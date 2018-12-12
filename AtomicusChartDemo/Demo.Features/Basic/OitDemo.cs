using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Order-independent transparency", GroupName = "Custom Features", Description = "Demo shows example of usage of order-independent transparency technique which allows " +
	                                                                                               "to achieve absolutely pixel perfect result of semi-transparent object blending. The " +
	                                                                                               "bad side of the mechanics is that it requires way more graphics resources especially with " +
	                                                                                               "enabled anti-aliasing methods like multisampling or supersampling.")]
	class OitDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Adding some intersecting semi-transparent objects.
			var cube = new Cube
			{
				Position = new Vector3F(0.5f),
				Size = new Vector3F(1f),
				Color = new Color4(Colors.DarkBlue, 150),
				Name = "Cube"
			};

			var sphere = new Sphere
			{
				Position = new Vector3F(0.5f),
				Radius = 0.6f,
				Color = new Color4(Colors.Green, 175),
				Name = "Sphere"
			};

			var cone = new Cone
			{
				Color = new Color4(Colors.DarkRed, 225),
				Direction = new Vector3F(0, 0, 1),
				Height = 1.25f,
				Position = new Vector3F(0.5f),
				Name = "Cone"
			};

			var disk = new Disk
			{
				Color = new Color4(Colors.Yellow, 125),
				Direction = new Vector3F(0, 0, 1),
				Height = 0.25f,
				Radius = 0.8f,
				Position = new Vector3F(0.5f, 0.5f, 1.15f),
				Name = "Disk"
			};

			// Enable order-independent transparency technique.
			chartControl.IsOitEnabled = true;

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data source.
			chartControl.DataSource = new RenderData[] { cube, sphere, cone, disk };
		}
	}
}
