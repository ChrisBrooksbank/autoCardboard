namespace autoCardboard.Pandemic
{
    public enum EventCard
    {
        /// <summary>
        /// Move any 1 pawn to any city.
        /// Get permission before moving another players pawn.
        /// </summary>
        Airlift,
        /// <summary>
        /// Remove any 1 card in the infection Discard Pile from the game.
        /// You may play this between the infect and intensify steps of an epidemic.
        /// </summary>
        ResilientPopulation,
        /// <summary>
        /// Add 1 research station to any city (no city card needed)
        /// </summary>
        GovernmentGrant,
        /// <summary>
        /// Skip the next infect cities step ( Do not flip over any infection cards )
        /// </summary>
        OneQuietNight,
        /// <summary>
        /// Draw, look at, and rearrange the top 6 cards of the infection deck. Put them back on top.
        /// </summary>
        Forecast
    }
}
