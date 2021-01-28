using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace autoCardboard.Pandemic.State
{
    public class MapNodeFactory
    {
        // Top left is 0,0 - means row 0 , column 0, there were gaps - approximate
        private const string CityGridRowsAndColumns = @"
            Atlanta:2,1.
            Chicago:1,1.
            Essen:1,5.
            London:1,4.
            Madrid:2,4.
            Milan:1,6.
            Montreal:1,2.
            NewYork:1,3.
            Paris:2,5.
            SanFrancisco:1,0.
            StPetersburg:0,6.
            Washington:2,3.
            Bogota:4,3.
            BuenosAires:6,4.
            Johannesburg:6,6.
            Khartoum:4,6.
            Kinshasa:5,6.
            Lagos:4,5.
            Lima:5,3.
            LosAngeles:3,0.
            MexicoCity:3,1.
            Miami:3,3.
            Santiago:6,3.
            SaoPaulo:5,4.
            Algiers:3,5.
            Baghdad:3,7.
            Cairo:3,6.
            Chennai:4,9.
            Delhi:3,9.
            Istanbul:2,6.
            Karachi:3,8.
            Kolkata:3,10.
            Moscow:2,7.
            Mumbai:4,8.
            Riyadh:4,7.
            Tehran:2,8.
            Bangkok:4,10.
            Beijing:2,11.
            HoChiMinhCity:5,11.
            HongKong:4,11.
            Jakarta:6,10.
            Manila:5,12.
            Osaka:4,13.
            Seoul:2,12.
            Shanghai:3,11.
            Sydney:6,12.
            Taipei:4,12.
            Tokyo:3,13.";

        private const string CityConnectionsData = @"
            Atlanta:Chicago,Washington,Miami.
            Chicago:SanFrancisco,Montreal,Atlanta,LosAngeles,MexicoCity.
            Essen:London,Paris,Milan,StPetersburg.
            London:NewYork,Madrid,Paris,Essen.
            Madrid:NewYork,London,Paris,Algiers,SaoPaulo.
            Milan:Essen,Paris,Istanbul.
            Montreal:Chicago,NewYork,Washington.
            NewYork:Montreal,Washington,London,Madrid.
            Paris:London,Essen,Milan,Algiers,Madrid.
            SanFrancisco:Tokyo,Manila,Chicago,LosAngeles.
            StPetersburg:Essen,Istanbul,Moscow.
            Washington:Montreal,NewYork,Atlanta,Miami.
            Bogota:Miami,SaoPaulo,BuenosAires,Lima,MexicoCity.
            BuenosAires:Bogota,SaoPaulo.
            Johannesburg:Kinshasa,Khartoum.
            Khartoum:Cairo,Lagos,Kinshasa,Johannesburg.
            Kinshasa:Lagos,Khartoum,Johannesburg.
            Lagos:SaoPaulo,Khartoum,Kinshasa.
            Lima:MexicoCity,Bogota,Santiago.
            LosAngeles:SanFrancisco,Sydney,Chicago,MexicoCity.
            MexicoCity:LosAngeles,Chicago,Miami,Bogota,Lima.
            Miami:Atlanta,Washington,Bogota,MexicoCity.
            Santiago:Lima.
            SaoPaulo:Bogota,Lagos,BuenosAires.         
            Algiers:Madrid,Paris,Istanbul,Cairo.
            Baghdad:Istanbul,Tehran,Karachi,Riyadh,Cairo.
            Cairo:Algiers,Istanbul,Baghdad,Riyadh,Khartoum.
            Chennai:Mumbai,Delhi,Kolkata,Bangkok,Jakarta.
            Delhi:Tehran,Kolkata,Chennai,Mumbai,Karachi.
            Istanbul:Milan,StPetersburg,Moscow,Baghdad,Cairo,Algiers.
            Karachi:Tehran,Delhi,Mumbai,Riyadh,Baghdad.
            Kolkata:Delhi,HongKong,Bangkok,Chennai.
            Moscow:StPetersburg,Tehran,Istanbul.
            Mumbai:Karachi,Delhi,Chennai.
            Riyadh:Baghdad,Karachi,Cairo.
            Tehran:Moscow,Delhi,Karachi,Baghdad.
            Bangkok:Kolkata,HongKong,HoChiMinhCity,Jakarta,Chennai.
            Beijing:Seoul,Shanghai.
            HoChiMinhCity:Bangkok,HongKong,Manila,Jakarta.
            HongKong:Shanghai,Taipei,Manila,HoChiMinhCity,Bangkok.
            Jakarta:Chennai,Bangkok,HoChiMinhCity,Sydney.
            Manila:HongKong,Taipei,SanFrancisco,Sydney,HoChiMinhCity.
            Osaka:Tokyo,Taipei.
            Seoul:Beijing,Tokyo,Shanghai.
            Shanghai:Beijing,Seoul,Tokyo,Taipei,HongKong.
            Sydney:LosAngeles,Jakarta,Manila.
            Taipei:Shanghai,Osaka,Manila,HongKong.
            Tokyo:SanFrancisco,Osaka,Shanghai,Seoul.";

        public MapNode CreateMapNode(City city)
        {
            var gridPosition = GetGridPosition(city);

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
                ConnectedCities = GetConnectedCities(city),
                GridRow = gridPosition.row,
                GridColumn = gridPosition.column
            };

            return newNode;
        }

        private List<City> GetConnectedCities(City city)
        {
            var cities = new List<City>();

            var dataSignature = city.ToString() + ":";
            var indexOfStart = CityConnectionsData.IndexOf(dataSignature);
            var indexOfEnd = indexOfStart + CityConnectionsData.Substring(indexOfStart).IndexOf(".");
            var dataLength = indexOfEnd - indexOfStart;

            var data = CityConnectionsData.Substring(indexOfStart, dataLength);
            data = data.Substring(dataSignature.Length);

            var citiesCsv = data.Split(',');

            foreach (var cityField in citiesCsv)
            {
                cities.Add((City)Enum.Parse(typeof(City),cityField, true));
            }

            return cities;
        }

        private (int row, int column) GetGridPosition(City city)
        {
            var gridRow = -1;
            var gridColumn = -1;

            var dataSignature = city.ToString() + ":";
            var indexOfStart = CityGridRowsAndColumns.IndexOf(dataSignature);
            var indexOfEnd = indexOfStart + CityGridRowsAndColumns.Substring(indexOfStart).IndexOf(".");
            var dataLength = indexOfEnd - indexOfStart;

            var data = CityGridRowsAndColumns.Substring(indexOfStart, dataLength);
            data = data.Substring(dataSignature.Length);

            var rowAndColumnCsv = data.Split(',');

            gridRow = int.Parse(rowAndColumnCsv[0]);
            gridColumn = int.Parse(rowAndColumnCsv[1]);

            return (gridRow, gridColumn);
        }
    }
}
