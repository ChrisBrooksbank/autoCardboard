using System.Linq;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic
{
    public static class EnumExtensions
    {
        public static Disease GetDefaultDisease(this City city)
        {
            var enumType = typeof(City);
            var memberInfos = enumType.GetMember(city.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DiseaseAttribute), false);
            var diseaseAttribute = ((DiseaseAttribute)valueAttributes[0]);
            return diseaseAttribute.Disease;
        }
    }
}
