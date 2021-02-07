using System.Collections.Generic;

namespace autoCardboard.Pandemic.State
{
    public class MapNodeFactory: IMapNodeFactory
    {
        public MapNode CreateMapNode(City city)
        {
            var newNode = new MapNode
            {
                City = city,
                DiseaseCubes = new Dictionary<Disease, int>()
                {
                    {Disease.Blue,0},
                    {Disease.Red,0},
                    {Disease.Yellow,0},
                    {Disease.Black,0}
                },
                ConnectedCities = city.GetConnections(),
                GridRow = city.GetGridPosition().row,
                GridColumn = city.GetGridPosition().column,
                DefaultDisease = city.GetDefaultDisease()
            };

            return newNode;
        }
    }
}
