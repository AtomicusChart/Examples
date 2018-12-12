using System;
using System.Windows.Media.Imaging;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Interface.Processing;
using AtomicusChart.Interface.UtilityTypes;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Lit sphere", GroupName = "Custom Features", Description =
			"Demo shows example of usage lit sphere bitmap to emulate material behaviour.")]
	class LitSphereDemo : FeatureDemo
	{
		private const int SurfaceResolution = 200;

		public override void Do(IDemoChartControl chartControl)
		{
			// Load bitmap image.
			var bitmapSource = new BitmapImage(new Uri(@"pack://application:,,,/Demo.Features;component/Resources/OldBronze.jpg"));
			
			// Setup the bitmap as enviroment image of the chart.
			chartControl.LitSphereBitmap = bitmapSource;

			// Generate surface mesh.
			Mesh mesh = GridHelper.GetStructuredParametricGridMesh((x, y) => new Vector3F(x, y, (float)(Math.Sin(x) * Math.Sin(x * x + y * y))), 
				new Vector2F(-3f), new Vector2F(3f), SurfaceResolution, SurfaceResolution);

			// Create testing surface.
			var sphere = new Surface
			{
				// Set the sphere color.
				Color = Colors.DarkBlue,
				// Setup the surface mesh.
				SurfaceMesh = mesh,
				// Setup material that will reflect environment image.
				Material = new RenderMaterial(0.75f, 0.6f, 0.3f, 0f, 1f),
				// Set name.
				Name = "Surface"
			};

			// Setup chart control options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Setup chart control data source.
			chartControl.DataSource = sphere;
		}
	}
}
