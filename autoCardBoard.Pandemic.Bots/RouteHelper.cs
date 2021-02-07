﻿using System;
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
        private readonly IMapNodeFactory _mapNodeFactory;

        public RouteHelper(IMemoryCache memoryCache, IMapNodeFactory mapNodeFactory)
        {
            _mapNodeFactory = mapNodeFactory;
            _memoryCache = memoryCache;
        }

        public City GetBestCityToDriveOrFerryTo(IPandemicState state, City startingLocation)
        {
            var threeCubeCities = state.Cities.Where(c => c.DiseaseCubeCount >= 3).ToList();

            var bestCandidate = 
                threeCubeCities.OrderBy(c => GetDistance(state.Cities,startingLocation, c.City)).FirstOrDefault();

            if (bestCandidate != null)
            {
                var startingNode = _mapNodeFactory.CreateMapNode(startingLocation);
                var bestMoveTo = startingNode.ConnectedCities.OrderBy(c => GetDistance(state.Cities,bestCandidate.City, c)).First();
                return bestMoveTo;
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

        public City? GetNearestCitywithResearchStation(List<MapNode> cities, City city)
        {
            return cities.Where(n => n.HasResearchStation == true)
                .OrderBy(n => GetDistance(cities, city, n.City) )
                .FirstOrDefault()?.City;
        }

        public int GetDistance(List<MapNode> cities, City city1, City city2)
        {
            return GetShortestPath(cities, city1, city2).Count - 1; // -2 because list includes start and destination
        }

        public List<City> GetShortestPath(List<MapNode> cities, City fromCity, City toCity)
        {
            var cityGraph = GetCityGraph(cities);

            var route =  cityGraph.Dijkstra( (uint)fromCity+1, (uint)toCity+1);
            var path = route.GetPath();

            var citiesTravelledTo = new List<City>();
            foreach (var node in path)
            {
                citiesTravelledTo.Add((City)(node-1));
            }

            return citiesTravelledTo;
        }

        public Graph<City, string> GetCityGraph(List<MapNode> cities)
        {
            Graph<City, string> cacheEntry;
            if (_memoryCache.TryGetValue("Pandemic.CityGraph", out cacheEntry))
            {
                return cacheEntry;
            }

            var graph = new Graph<City, string>();

            foreach(var city in Enum.GetValues(typeof(City)) )
            {
                graph.AddNode((City)city);
            }

            foreach (var cityNode in cities)
            {
                foreach (var connectedCity in cityNode.ConnectedCities)
                {
                    graph.Connect((uint)cityNode.City + 1, (uint)connectedCity + 1, 1, $"{cityNode.City}-{connectedCity}");
                }
            }

            cacheEntry = graph;
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(90));

            // Save data in cache.
            _memoryCache.Set("Pandemic.CityGraph", cacheEntry, cacheEntryOptions);
            
            return graph;
        }
    }
}
