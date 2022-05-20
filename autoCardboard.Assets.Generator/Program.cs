using autoCardboard.Assets.Generator.Pandemic.MapBuilder;

var pandemicMapBuilder = new SvgBuilder();
pandemicMapBuilder.Clear();
pandemicMapBuilder.AddAllCities();
Console.WriteLine(pandemicMapBuilder.ToHtml());