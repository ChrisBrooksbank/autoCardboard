using System;
using System.Collections.Generic;
using System.Text;

namespace autoCardboard.Pandemic.State.Delta
{
    public interface IPlayerSetupDelta
    {
        int PlayerId { get; set; }
        PlayerRole PlayerRole { get; set; }
        City City{ get; set; }
    }
}
