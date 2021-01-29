using autoCardboard.Pandemic.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardBoard.Pandemic.Bots
{
    public class PlayerDeckHelper : IPlayerDeckHelper
    {
        public Dictionary<Disease, List<PandemicPlayerCard>> GetCityCardsByColour(IEnumerable<PandemicPlayerCard> cards)
        {
            var cityCardsByColour = new Dictionary<Disease, List<PandemicPlayerCard>>();
            if (cards == null)
            {
                return cityCardsByColour;
            }

            cityCardsByColour[Disease.Blue] = new List<PandemicPlayerCard>();
            cityCardsByColour[Disease.Red] = new List<PandemicPlayerCard>();
            cityCardsByColour[Disease.Black] = new List<PandemicPlayerCard>();
            cityCardsByColour[Disease.Yellow] = new List<PandemicPlayerCard>();

            foreach (var cityCard in cards.Where(c => c.PlayerCardType == PlayerCardType.City))
            {
                var city = (City) cityCard.Value;
                cityCardsByColour[city.GetDefaultDisease()].Add(cityCard);
            }

            return cityCardsByColour;
        }
    }
}
