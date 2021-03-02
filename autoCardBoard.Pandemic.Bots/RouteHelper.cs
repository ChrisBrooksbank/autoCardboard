using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Pandemic.State;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using Microsoft.Extensions.Caching.Memory;

namespace autoCardBoard.Pandemic.Bots
{
    public class RouteHelper : IRouteHelper
    {
        private readonly IMemoryCache _memoryCache;

        public RouteHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        
        public City GetBestCityToTravelToWithoutDiscarding(IPandemicState state, City startingLocation)
        {
            var startingNode = state.Cities.Single(n => n.City == startingLocation);
            var threeCubeCities = state.Cities.Where(c => c.DiseaseCubeCount >= 3).ToList();

            var bestCandidate = 
                threeCubeCities.OrderBy(c => GetDistance(state,startingLocation, c.City)).FirstOrDefault();

            if (bestCandidate != null)
            {
                var connections = startingNode.ConnectedCities.ToList();
                if (startingNode.HasResearchStation)
                {
                    var researchStationNodes = state.Cities
                        .Where(n => n.HasResearchStation && n.City != startingLocation)
                        .Select(n => n.City);
                    connections.AddRange(researchStationNodes);
                }
                var bestDriveFerryOrShuttleFlightTo = connections.OrderBy(c => GetDistance(state,bestCandidate.City, c)).First();
                
                return bestDriveFerryOrShuttleFlightTo;
            }

            return bestCandidate == null ? GetRandomNeighbour(state, startingLocation) : bestCandidate.City;
        }
        
        public City GetRandomNeighbour(IPandemicState state, City startingLocation)
        {
            var connectionCount = state.Cities.Single(n => n.City ==  startingLocation).ConnectedCities.Count();
            var moveDie = new Die(connectionCount);
            var moveDieRoll = moveDie.Roll();
            var moveTo = state.Cities.Single(n => n.City ==  startingLocation).ConnectedCities.ToArray()[moveDieRoll - 1];

            return moveTo;
        }

        public City? GetNearestCitywithResearchStation(IPandemicState state, City city)
        {
            return state.Cities.Where(n => n.HasResearchStation == true)
                .OrderBy(n => GetDistance(state, city, n.City) )
                .FirstOrDefault()?.City;
        }

        public int GetDistance(IPandemicState state, City city1, City city2)
        {
            return GetShortestPath(state, city1, city2).Count - 1; // -2 because list includes start and destination
        }

        public List<City> GetShortestPath(IPandemicState state, City fromCity, City toCity)
        {
            var citiesWithResearchStations = state.Cities.Where( n => n.HasResearchStation).ToList();
            var cacheKey = citiesWithResearchStations.Count < 2 ? $"Pandemic.ShortestPath.{fromCity}-{toCity}" :  $"Pandemic.{state.Id}.ShortestPath.{fromCity}-{toCity}";
            var cacheExpires = citiesWithResearchStations.Count < 2 ? TimeSpan.FromMinutes(30) : TimeSpan.FromMilliseconds(2000);

            List<City> cacheEntry;
            if (_memoryCache.TryGetValue(cacheKey, out cacheEntry))
            {
                return cacheEntry;
            }

            var cityGraph = GetCityGraph(state);

            var route =  cityGraph.Dijkstra( (uint)fromCity+1, (uint)toCity+1);
            var path = route.GetPath();

            var citiesTravelledTo = new List<City>();
            foreach (var node in path)
            {
                citiesTravelledTo.Add((City)(node-1));
            }

            cacheEntry = citiesTravelledTo;
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(cacheExpires);
            _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);

            return citiesTravelledTo;
        }

        public Graph<City, string> GetCityGraph(IPandemicState state)
        {
            var citiesWithResearchStations = state.Cities.Where( n => n.HasResearchStation).ToList();
            var cacheKey = citiesWithResearchStations.Count < 2 ? "Pandemic.CityGraph" : $"Pandemic.{state.Id}.CityGraph.R{state.ResearchStationStock}";
            var cacheExpires = citiesWithResearchStations.Count < 2 ? TimeSpan.FromMinutes(30) : TimeSpan.FromMilliseconds(2000);

            Graph<City, string> cacheEntry;
            if (_memoryCache.TryGetValue(cacheKey, out cacheEntry))
            {
                return cacheEntry;
            }

            var graph = new Graph<City, string>();

            foreach(var city in Enum.GetValues(typeof(City)) )
            {
                graph.AddNode((City)city);
            }

            foreach (var cityNode in state.Cities)
            {
                foreach (var connectedCity in cityNode.ConnectedCities)
                {
                    graph.Connect((uint)cityNode.City + 1, (uint)connectedCity + 1, 1, $"{cityNode.City}-{connectedCity}");
                }

                // Connect all cities with research stations together
                if (cityNode.HasResearchStation)
                {
                    foreach (var researchStationCity in citiesWithResearchStations)
                    {
                        if (researchStationCity.City != cityNode.City)
                        {
                            graph.Connect((uint)cityNode.City + 1, (uint)researchStationCity.City + 1, 1, $"Shuttle:{cityNode.City}-{researchStationCity.City}");
                        }
                    }
                }
            }

            cacheEntry = graph;
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(cacheExpires);
            _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            
            return graph;
        }

        public int GetLocationValue(IPandemicState state, City city)
        {
            var locationValue = 0;
            var cityNode = state.Cities.Single( n => n.City == city);

            locationValue += cityNode.DiseaseCubeCount * 3;

            foreach (var neighbour in cityNode.ConnectedCities)
            {
                var neighbourNode =  state.Cities.Single( n => n.City == neighbour);
                locationValue += neighbourNode.DiseaseCubeCount * 2;
            }

            if (cityNode.HasResearchStation)
            {
                locationValue += 3;
            }
            
            return locationValue;
        }

        public City GetBestLocationOnBoard(List<MapNode> cities)
        {
            var cityWithMostDisease = cities.OrderByDescending(c => c.DiseaseCubeCount).First();
            return cityWithMostDisease.City;
        }

        public City? GetBestLocationForNewResearchStation(IPandemicState state)
        {
            var nodesWithoutResearchStation = state.Cities.Where( n => !n.HasResearchStation).ToList();

            City? isolatedCity = null;
            var furthestDistance = 0;
            foreach (var nodeWithoutResearchStation in nodesWithoutResearchStation)
            {
                var nearestResearchStation = GetNearestCitywithResearchStation(state, nodeWithoutResearchStation.City);
                if (nearestResearchStation.HasValue)
                {
                    var distanceToNearestResearchStation =
                        GetDistance(state, nodeWithoutResearchStation.City, nearestResearchStation.Value);
                    if (distanceToNearestResearchStation > furthestDistance)
                    {
                        isolatedCity = nodeWithoutResearchStation.City;
                        furthestDistance = distanceToNearestResearchStation;
                    }
                }
            }

            return isolatedCity;
        }
    }
}
