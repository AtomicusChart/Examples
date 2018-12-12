using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Custom series data reader", GroupName = "Series", Description = "This example shows how to create custom series data reader and implement efficient dynamic update logic. " +
																					 "You can use DynamicPositionMaskDataReader for this purpose as implemented reader for most tasks.")]
	class AnimatedSeriesDemo : FeatureDemo
	{
		private const int PointCount = 50;
		private static readonly Random random = new Random();

		/// <summary>
		/// This class extends <see cref="PositionMaskDataReader"/> and implements custom logic of randomizing it's data content.
		/// </summary>
		private class CustomSeriesDataReader : PositionMaskDataReader
		{
			private int index;

			/// <summary>
			/// This writer is simply used to write a single point to the specified index.
			/// </summary>
			private class CustomPositionMaskWriter : IPositionMaskDataWriter
			{
				private readonly Vector3F value;
				private readonly int index;

				public CustomPositionMaskWriter(Vector3F value, int index)
				{
					this.value = value;
					this.index = index;
				}

				public void Write(IResourceWriter1D positionsWriter, IResourceWriter1D vertexMasksWriter) =>
					positionsWriter.UpdateResource(new[] { value }, 0, 1, index);
			}

			public CustomSeriesDataReader(int vertexCount) : base(vertexCount) { }

			public override void InitializeResources(IResourceWriter1D positionsWriter, IResourceWriter1D vertexMasksWriter) =>
				positionsWriter.UpdateResource(RandomizePoints(VertexCount, 5f, 25));

			private static Vector3F[] RandomizePoints(int count, float argumentStep, float valueMax)
			{
				Vector3F[] result = new Vector3F[count];
				for (int i = 0; i < count; i++)
				{
					result[i] = new Vector3F(argumentStep * i, (float)random.NextDouble() * valueMax, 0);
				}
				return result;
			}

			public void RandomizePoint(float argumentStep, float valueMax)
			{
				if (index >= VertexCount)
					index = 0;
				Vector3F point = new Vector3F(argumentStep * index, (float)random.NextDouble() * valueMax, 0);
				OnDataChanged(new CustomPositionMaskWriter(point, index));
				index++;
			}
		}

		private readonly AnimationHelper animationHelper = new AnimationHelper();

		public override void Do(IDemoChartControl chartControl)
		{
			// Create custom data reader.
			var dataReader = new CustomSeriesDataReader(PointCount);

			// Create series.
			var series = new Series
			{
				// Set data reader.
				Reader = dataReader,
				// Set series line color.
				Color = Colors.Blue,
				// Set series line thickness.
				Thickness = 2.0f,
				// Set series line pattern style.
				PatternStyle = PatternStyle.Solid,
				// Set series marker color.
				MarkerColor = Colors.Red,
				// Set series marker style.
				MarkerStyle = MarkerStyle.Cross,
				// Set name.
				Name = "Line"
			};

			// Setup chart view settings.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;
			chartControl.ViewResetOptions.ResetOnDataChanged = false;

			// Setup chart data source.
			chartControl.DataSource = series;

			// Start animation.
			animationHelper.Start(value => value, value => dataReader.RandomizePoint(5f, 25), 0f, 0f, 25);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}
}
