using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.AxesData;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Cartesian", GroupName = "Coordinate Systems", Description = "Demo shows example of presenting render data in cartesian coordinate system.")]
	class CartesianCSDemo : FeatureDemo
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

			var points = new SingleColorPoints
			{
				// Reader approach is used to improve performance for big data sets and their updates.
				Reader = new DefaultPositionMaskDataReader(positions),
				// Set points color.
				Color = Colors.DarkBlue,
				// Set name.
				Name = "Points"
			};

			// Setup chart settings.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.Axes.CoordinateSystem = CoordinateSystem.Cartesian;

			// Setup several axis settings.
			chartControl.Axes.X.AxisThickness = 2.0f;
			chartControl.Axes.X.AxisColor = Colors.Red;
			chartControl.Axes.Y.AxisThickness = 2.0f;
			chartControl.Axes.Y.AxisColor = Colors.Green;
			chartControl.Axes.Z.AxisThickness = 2.0f;
			chartControl.Axes.Z.AxisColor = Colors.Blue;

			// Set chart data source.
			chartControl.DataSource = points;
		}
	}
}
