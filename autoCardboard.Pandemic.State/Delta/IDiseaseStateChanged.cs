using System;
using System.Collections.Generic;
using System.Text;

namespace autoCardboard.Pandemic.State.Delta
{
    public interface IDiseaseStateChanged: IDelta
    {
        Disease Disease { get; set; }
        DiseaseState DiseaseState{ get; set; }
    }
}
