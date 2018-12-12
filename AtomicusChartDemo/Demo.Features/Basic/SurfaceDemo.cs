using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Interface.Processing;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Custom surface", GroupName = "Primitives", Description = "Demo shows example of using custom primitive.")]
	class SurfaceDemo : FeatureDemo
	{
		private const int SurfaceResolution = 200;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate surface mesh.
			Mesh mesh = GridHelper.GetStructuredParametricGridMesh((x, y) => new Vector3F(x, y, (float)(Math.Sin(x) * Math.Sin(x * x + y * y))),
				new Vector2F(-3f), new Vector2F(3f), SurfaceResolution, SurfaceResolution);

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data soruce.
			chartControl.DataSource = new Surface
			{
				SurfaceMesh = mesh,
				Name ="Surface"
			};
		}
	}
}
