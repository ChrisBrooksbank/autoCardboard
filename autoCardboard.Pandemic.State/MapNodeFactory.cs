using System;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.State
{
    public class MapNodeFactory
    {
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
                ConnectedCities = GetConnectedCities(city)
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
    }
}
