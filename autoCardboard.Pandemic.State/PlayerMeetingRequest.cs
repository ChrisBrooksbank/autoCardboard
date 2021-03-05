using System;
using System.Collections.Generic;
using System.Text;

namespace autoCardboard.Pandemic.State
{
    /// <summary>
    /// Knowledge sharing requires two players to be in same city
    /// If a bot wants to meet another bot in a particular city
    /// They can create a PlayerMeetingRequest
    /// </summary>
    [Serializable]
    public class PlayerMeetingRequest
    {
        public int RequestedByPlayerId { get; set; }
        public int RequestForPlayerId { get; set; }
        public City MeetingLocation { get; set; }
    }
}
