using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Dynamic points with value", GroupName = "Points", Description = "Demo shows example of using points with value and its default implementation of dynamic data reader.")]
	class DynamicValuePointsDemo : FeatureDemo
	{
		private const int PointsCount = 100000;
		private const float MaxRadius = 5;
		private const float ValueBound = 5;
		private static readonly Random Random = new Random(DateTime.Now.Millisecond);
		private readonly AnimationHelper animationHelper = new AnimationHelper();

		private float[] RandomizeValues(Vector3F[] positions, float argument)
		{
			float[] values = new float[PointsCount];
			for (int i = 0; i < values.Length; i++)
			{
				float offset = Math.Min(ValueBound - positions[i].Z, positions[i].Z - ValueBound);
				values[i] = positions[i].Z + (offset / 2) * (float)Math.Sin(argument);
			}
			return values;
		}

		public override void Do(IDemoChartControl chartControl)
		{
			// Initialize positions.
			var positions = new Vector3F[PointsCount];
			for (int i = 0; i < PointsCount; i++)
			{
				positions[i] = new Vector3F(
					(float)Random.NextDouble() * MaxRadius,
					(float)Random.NextDouble() * MaxRadius,
					(float)Random.NextDouble() * MaxRadius);
			}

			// Initialize data reader.
			var reader = new DynamicPositionValueMaskDataReader(PointsCount, new OneAxisBounds(0, ValueBound));

			// Creation of data presentation.
			var points = new ValuePoints
			{
				// Reader can be reused and we can create several presentations for 1 reader.
				Reader = reader,
				// Set name.
				Name = "Points"
			};

			// Randomize initial data.
			reader.UpdatePositions(positions);
			reader.UpdateValues(RandomizeValues(positions, 0));

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data source.
			chartControl.DataSource = points;

			// Start animation.
			animationHelper.Start(
				(f) => f,
				rf => reader.UpdateValues(RandomizeValues(positions, rf)),
				0, 0.075f, 25);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}
}
