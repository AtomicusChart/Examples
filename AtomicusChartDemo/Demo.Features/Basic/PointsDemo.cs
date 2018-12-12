using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Points", GroupName = "Primitives", Description = "Demo shows example of using points.")]
	class PointsDemo : FeatureDemo
	{
		private static readonly Random Random = new Random(DateTime.Now.Millisecond);
		private const int PointsCount = 100000;
		private const float MaxRadius = 5;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate positions.
			var positions = new Vector3F[PointsCount];
			for (int i = 0; i < PointsCount; i++)
			{
				positions[i] = new Vector3F(
					(float)Random.NextDouble() * MaxRadius,
					(float)Random.NextDouble() * MaxRadius,
					(float)Random.NextDouble() * MaxRadius);
			}

			// Creation of data presentation.
			var points = new SingleColorPoints
			{
				// Reader approach is used to improve performance for big data sets and their updates.
				Reader = new DefaultPositionMaskDataReader(positions),
				// Set points color.
				Color = Colors.DarkBlue,
				// Set name.
				Name = "Points"
			};

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data source.
			chartControl.DataSource = points;
		}
	}
}
