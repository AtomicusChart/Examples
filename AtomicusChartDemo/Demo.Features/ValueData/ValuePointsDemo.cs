using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Points with value", GroupName = "Points", Description = "Demo shows example of using points with value.")]
	class ValuePointsDemo : FeatureDemo
	{
		private const int PointsCount = 100000;
		private const float MaxRadius = 5;
		private static readonly Random Random = new Random(DateTime.Now.Millisecond);

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
			var points = new ValuePoints
			{
				// Reader can be reused and we can create several presentations for 1 reader.
				Reader = new DefaultPositionValueMaskDataReader(
					positions, // Points positions.
					DemoHelper.ExtractZValues(positions, out OneAxisBounds valueBounds), // Points values, for demo purposes values are extracted from Z position component, but in real world they are independent.
					valueBounds), // We must specify value axis bounds.
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
