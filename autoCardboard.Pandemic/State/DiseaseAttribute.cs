using System;

namespace autoCardboard.Pandemic
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
