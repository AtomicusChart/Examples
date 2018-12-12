using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Lines", GroupName = "Primitives", Description = "Demo shows example of using line primitive.")]
	class LineDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			const int pointsCount = 10_000;
			const float amplitude = 0.6f;


			chartControl.ContextView.Mode2D = true;

			// Generate points for line.
			var points = new Vector3F[pointsCount];
			for (var i = 0; i < pointsCount; i++)
			{
				float scaledCoord = i / (float)pointsCount;
				points[i] = new Vector3F(
					scaledCoord,
					amplitude * (float)(Math.Cos(5 * 2 * Math.PI * scaledCoord) + 0.5f),
					amplitude * (float)(Math.Sin(5 * Math.PI * scaledCoord) + 0.5f));
			}
			// Create line.
			chartControl.DataSource = new Line
			{
				Points = points,
				//If strip is true - than points are connected one by one, otherwise line list is used which means that pair of points is used to create one line segment.
				Strip = true,
				Thickness = 2f,
				Color = Colors.DarkBlue,
				Name = "Line"
			};
		}
	}
}
