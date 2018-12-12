using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Custom series with value", GroupName = "Series", Description = "Demo shows example of using series with value and implement partial data update logic.")]
	public class AnimatedValueSeriesDemo : FeatureDemo
	{
		/// <summary>
		/// Represents extension of <see cref="PositionValueMaskDataReader"/> that implements random value generation logic.
		/// </summary>
		class CustomPositionValueMaskReader : PositionValueMaskDataReader
		{
			class CustomWriter : IPositionValueMaskDataWriter
			{
				private readonly CustomPositionValueMaskReader reader;
				public CustomWriter(CustomPositionValueMaskReader reader) => this.reader = reader;

				public void Write(IResourceWriter1D positionsWriter, IResourceWriter1D valuesWriter,
					IResourceWriter1D masksWriter) =>
					valuesWriter.UpdateResource(reader.values);
			}

			private readonly CustomWriter writer;
			private readonly Vector3F[] positions;
			private readonly float[] initialValues;
			private readonly float[] values;

			public CustomPositionValueMaskReader(Vector3F[] positions, float[] initialValues, OneAxisBounds valueRange) : base(positions.Length)
			{
				this.positions = positions;
				this.initialValues = initialValues;
				ValueRange = valueRange;
				writer = new CustomWriter(this);

				values = new float[initialValues.Length];
				Array.Copy(initialValues, values, values.Length);
			}

			public void RandomizeValues(float argument)
			{
				for (int i = 0; i < values.Length; i++)
				{
					float offset = Math.Min(ValueRange.Max - initialValues[i], initialValues[i] - ValueRange.Min);
					values[i] = initialValues[i] + (float)Math.Cos(argument) * offset;
				}
				OnDataChanged(writer);
			}

			public override void InitializeResources(IResourceWriter1D positionsWriter, IResourceWriter1D valuesWriter,
				IResourceWriter1D masksWriter)
			{
				// We're not using vertex masks, so just ignore the writer.
				positionsWriter.UpdateResource(positions);
				valuesWriter.UpdateResource(values);
			}
		}

		private const int PointCount = 25;

		private static readonly Random random = new Random();

		private static Vector3F[] RandomizePoints(int count, float argumentStep, float valueMax)
		{
			Vector3F[] result = new Vector3F[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new Vector3F(argumentStep * i, (float)random.NextDouble() * valueMax, 0);
			}
			return result;
		}

		private static float[] RandomizeValues(int count)
		{
			float[] result = new float[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = (float)random.NextDouble();
			}
			return result;
		}

		private readonly AnimationHelper animationHelper = new AnimationHelper();

		public override void Do(IDemoChartControl chartControl)
		{
			// Create series custom data reader.
			var reader = new CustomPositionValueMaskReader(
				RandomizePoints(PointCount, 0.5f, 25f), // Randomize points.
				RandomizeValues(PointCount), // Randomize values.
				new OneAxisBounds(0, 1)); // Setup value range.

			// Create value series render data.
			var series = new ValueSeries
			{
				// Set series data reader.
				Reader = reader,
				// Set series marker style.
				MarkerStyle = MarkerStyle.Circle,
				// Set series pattern style.
				PatternStyle = PatternStyle.Solid,
				// Set series marker size.
				MarkerSize = 15,
				// Set series line thickness.
				Thickness = 4f,
				// Set name.
				Name = "Series"
			};

			// Setup chart view options.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;

			// Set chart data source.
			chartControl.DataSource = series;

			// Start animation.
			animationHelper.Start(value => value, value => reader.RandomizeValues(value), 0f, 0.025f, 16);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}
}
