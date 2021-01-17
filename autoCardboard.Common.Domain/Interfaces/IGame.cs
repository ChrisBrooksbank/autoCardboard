using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGame<TGameState, TGameTurn> 
        where TGameState : IGameState 
        where TGameTurn : IGameTurn
    {
        /// <summary>
        /// A list of players who are playing this game.
        /// A player is some code which can take a turn, when asked, given a TGameTurn.
        /// </summary>
        IEnumerable<IPlayer<TGameTurn>> Players { get; set; }

        /// <summary>
        /// Plays one game
        /// Performs game setup
        /// Asks players to take turns in order, giving them information to make decision
        /// Verifies each turn is legal
        /// Detects when game is finished and which player won
        /// </summary>
        void Play();
    }
}
