using System;
using System.Collections.Generic;
using System.Text;

namespace autoCardboard.Pandemic.Domain
{
    public enum PlayerStandardAction
    {
        /// <summary>
        /// Move to a city connected by a white line.
        /// </summary>
        DriveOrFerry,
        /// <summary>
        /// Discard a City card to move to the city named on the card.
        /// </summary>
        DirectFlight,
        /// <summary>
        /// Discard the City card that matches the city that you are in to move to any city.
        /// </summary>
        CharterFlight,
        /// <summary>
        /// Move from a city with a research station to any other city that has a research station.
        /// </summary>
        ShuttleFlight,
        /// <summary>
        /// Discard the city card that matches the city you are in to place a reseach station there.
        /// </summary>
        BuildResearchStation,
        /// <summary>
        /// Remove 1 disease cube from the city you are in. If this color is cured, remove all cubes of that color from the city.
        /// </summary>
        TreatDisease,
        /// <summary>
        /// Either: give the card that matches the city you are in to another player, or take that card from another player.
        /// The other player must also be in the city with you.
        /// </summary>
        ShareKnowledge,
        /// <summary>
        /// At any research station, discard 5 city cards of the same disease color to cure that disease.
        /// </summary>
        DiscoverCure
    }
}
