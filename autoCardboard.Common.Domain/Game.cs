using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Common.Domain
{
    /// <summary>
    /// Responsible for implementing a game e.g. :
    /// allowing players to take turns ( providing them with a TGameTurn allow them to decide what move to make ),
    /// validating a legal turn is played which follows the rules of the game.
    /// Determining when the game is over.
    /// </summary>
    /// <typeparam name="TGameState">Type responsible for storing game state, including player state.</typeparam>
    /// <typeparam name="TGameTurn">Type passed to a player when they are being asked to take a turn</typeparam>
    public abstract class Game<TGameState, TGameTurn>
        where TGameState : IGameState
        where TGameTurn : IGameTurn
    {
        /// <summary>
        /// Responsible for storing all game state ( including state for all players )
        /// </summary>
        public TGameState State { get; set; }

        /// <summary>
        /// A list of players who are playing this game.
        /// A player is some code which can take a turn, when asked, given a TGameTurn.
        /// </summary>
        public IEnumerable<IPlayer<TGameTurn>> Players { get; set; }
               
        /// <summary>
        /// Plays one game
        /// Performs game setup
        /// Asks players to take turns in order, giving them information to make decision
        /// Verifies each turn is legal
        /// Detects when game is finished and which player won
        /// </summary>
        abstract public void Play(IEnumerable<IPlayer<TGameTurn>> players);
    }
}
