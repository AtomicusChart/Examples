using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.AxesData;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Spherical", GroupName = "Coordinate Systems", Description = "Demo shows example of presenting render data in spherical coordinate system.")]
	class SphericalCSDemo : FeatureDemo
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
				var radius = (float)Random.NextDouble() * MaxRadius;
				double angle1 = Random.NextDouble() * 2 * Math.PI;
				double angle2 = Random.NextDouble() * 2 * Math.PI;
				float projectedRadius = radius * (float)Math.Cos(angle2);
				var x = (float)(projectedRadius * Math.Cos(angle1));
				var y = (float)(projectedRadius * Math.Sin(angle1));
				var z = (float)(radius * Math.Sin(angle2));
				positions[i] = new Vector3F(x, y, z);
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
			chartControl.Axes.CoordinateSystem = CoordinateSystem.Spherical;

			// Setup several axis settings.
			chartControl.Axes.R.AxisThickness = 2.0f;
			chartControl.Axes.R.AxisColor = Colors.Red;
			chartControl.Axes.Phi.AxisThickness = 2.0f;
			chartControl.Axes.Phi.AxisColor = Colors.Green;
			chartControl.Axes.Theta.AxisThickness = 2.0f;
			chartControl.Axes.Theta.AxisColor = Colors.Blue;

			// Set chart data source.
			chartControl.DataSource = points;
		}
	}
}
