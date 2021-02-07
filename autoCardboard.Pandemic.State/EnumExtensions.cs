using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.State
{
    public static class EnumExtensions
    {
        public static Disease GetDefaultDisease(this City city)
        {
            var enumType = typeof(City);
            var memberInfos = enumType.GetMember(city.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DiseaseAttribute), false);
            var diseaseAttribute = ((DiseaseAttribute) valueAttributes[0]);
            return diseaseAttribute.Disease;
        }

        public static IEnumerable<City> GetConnections(this City city)
        {
            var enumType = typeof(City);
            var memberInfos = enumType.GetMember(city.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(ConnectionsAttribute), false);
            return ((ConnectionsAttribute) valueAttributes[0]).Connections;
        }

        public static (int row, int column) GetGridPosition(this City city)
        {
            var enumType = typeof(City);
            var memberInfos = enumType.GetMember(city.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(GridPositionAttribute), false);
            var attribute = (GridPositionAttribute) valueAttributes[0];
            return (attribute.Row, attribute.Column);
        }
    }
}
