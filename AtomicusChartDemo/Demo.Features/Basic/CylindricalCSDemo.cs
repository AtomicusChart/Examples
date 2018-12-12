using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.AxesData;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Cylindrical", GroupName = "Coordinate Systems", Description = "Demo shows example of presenting render data in cylindrical coordinates system.")]
	class CylindricalCSDemo : FeatureDemo
	{
		private static readonly Random Random = new Random(DateTime.Now.Millisecond);
		private const int PointsCount = 100000;
		private const double MaxRadius = 5;

		private static Vector3F[] GetPoints()
		{
			var points = new Vector3F[PointsCount];
			for (int i = 0; i < PointsCount; i++)
			{
				var radius = (float)Math.Sqrt(Random.NextDouble() * MaxRadius);
				double angle = Random.NextDouble() * 2 * Math.PI;
				var x = (float)(radius * Math.Cos(angle));
				var y = (float)(radius * Math.Sin(angle));
				var z = (float)(Random.NextDouble() * MaxRadius);
				points[i] = new Vector3F(x, y, z);
			}
			return points;
		}

		public override void Do(IDemoChartControl chartControl)
		{
			var points = new SingleColorPoints
			{
				// Reader approach is used to improve performance for big data sets and their updates.
				Reader = new DefaultPositionMaskDataReader(GetPoints()),
				// Set points color.
				Color = Colors.DarkBlue,
				// Set name.
				Name = "Points"
			};

			// Setup chart settings. Note: for cylindrical system feel free to specify any axis as height axis. 
			// Currently we're using Z as height axis.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.Axes.CylindricalSettings.ZAxisAxisIndex = 2;
			chartControl.Axes.CoordinateSystem = CoordinateSystem.Cylindrical;

			// Setup several axis settings.
			chartControl.Axes.R.AxisThickness = 2.0f;
			chartControl.Axes.R.AxisColor = Colors.Red;
			chartControl.Axes.Phi.AxisThickness = 2.0f;
			chartControl.Axes.Phi.AxisColor = Colors.Green;
			chartControl.Axes.Z.AxisThickness = 2.0f;
			chartControl.Axes.Z.AxisColor = Colors.Blue;

			// Set chart data source.
			chartControl.DataSource = points;
		}
	}
}
