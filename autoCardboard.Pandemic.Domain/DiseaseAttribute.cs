using System;
using System.Collections.Generic;
using System.Text;

namespace autoCardboard.Pandemic.Domain
{
    public class DiseaseAttribute:Attribute
    {
        private Disease _disease;
        public Disease Disease => _disease;

        public DiseaseAttribute(Disease disease)
        {
            _disease = disease;
        }
    }
}
