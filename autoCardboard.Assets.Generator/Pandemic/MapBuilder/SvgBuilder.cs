using System.Reflection;
using System.Text;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Assets.Generator.Pandemic.MapBuilder
{
    public class SvgBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        
        public SvgBuilder()
        {
        }

        public void Clear()
        {
            _stringBuilder.Clear();
        }


        public string ToHtml()
        {
            var template = GetTemplate();
            var cities = _stringBuilder.ToString();
            var html = template.Replace("<cities/>", cities);
            return html;
        }

        public void AddAllCities()
        {
            var allCities = Enum.GetValues(typeof(City));
            foreach (City city in allCities)
            {
                AddCity(city);
            }
        }

        public void AddCity(City city)
        {
            var cityName = city.ToString();
            var defaultDiseaseName = city.GetAttributeOfType<DiseaseAttribute>().Disease.ToString().ToLower();
            var gridPosition = city.GetAttributeOfType<GridPositionAttribute>();
            
            var data = $"<text id=\"{cityName}\" class=\"{defaultDiseaseName}city city\" data-gridrow=\"{gridPosition.Row}\" data-gridcolumn=\"{gridPosition.Column}\"></text>";
            _stringBuilder.AppendLine(data);
        }

        private string GetTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "autoCardboard.Assets.Generator.Pandemic.MapBuilder.SvgTemplate.html";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
