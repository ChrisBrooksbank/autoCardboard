using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace autoCardboard.Pandemic.Domain
{
    public enum PlayerRole
    {
        /// <summary>
        /// Brown Pawn
        /// You may give any 1 of your City cards when you Share Knowledge. It need not match your city. 
        /// A player who Shares Knowledge with you on their turn can take any 1 of your City cards.
        /// </summary>
        Researcher,
        /// <summary>
        /// Dark Green Pawn
        /// Prevent disease cube placements (and outbreaks) in the city you are in and all the cities connected to it.
        /// </summary>
        QuarantineSpecialist,
        /// <summary>
        /// Teal Pawn
        /// As an action, take any discarded event card and store it on this card.
        /// When you play the stored Event card, remove it from the game.
        /// Limit : 1 Event card on this card at a time, which is not part of your hand.
        /// </summary>
        ContingencyPlanner,
        /// <summary>
        /// Lime Pawn
        /// As an action build a research station in the city you are in ( no city card needed )
        /// Once per turn as an action, move from a reseach station to any city by discarding any City card.
        /// </summary>
        OperationsExpert,
        /// <summary>
        /// Orange Pawn
        /// Remove all cubes of one color when doing Treat Disease.
        /// Automatically remove cubes of cured diseases from the city you are in ( and prevent them from being placed there )
        /// </summary>
        Medic,
        /// <summary>
        /// White Pawn
        /// You only need 4 cards of the same color to do the Discover a Cure action.
        /// </summary>
        Scientist,
        /// <summary>
        /// Lilac Pawn
        /// Move another players pawn as if it were yours.
        /// As an action, move any pawn to a city with another pawn.
        /// Get permission before moving another players pawn.
        /// </summary>
        Dispatcher
    }
}
