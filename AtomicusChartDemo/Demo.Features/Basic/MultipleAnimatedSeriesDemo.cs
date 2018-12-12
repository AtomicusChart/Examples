using System;
using System.Collections.Generic;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Multiple custom series data reader", GroupName = "Series", Description = "This example shows how to create custom parallel collection of independly updated series data reader and implement efficient dynamic update logic.")]
	class MultipleAnimatedSeriesDemo : FeatureDemo
	{
		private const int SeriesCount = 5;
		private const int PointCount = 50;
		private static readonly Random random = new Random();

		private static readonly MarkerStyle[] markers = new[]
			{MarkerStyle.Circle, MarkerStyle.Cross, MarkerStyle.Rhombus, MarkerStyle.Triangle, MarkerStyle.Square};

		/// <summary>
		/// This class extends <see cref="PositionMaskDataReader"/> and implements custom logic of randomizing it's data content.
		/// </summary>
		private class CustomSeriesDataReader : PositionMaskDataReader
		{
			private readonly Vector3F[] positions;
			private readonly float offset;
			private readonly float argumentStep;
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

			public CustomSeriesDataReader(int vertexCount, float argumentStep, float offset) : base(vertexCount)
			{
				this.offset = offset;
				this.argumentStep = argumentStep;
				positions = new Vector3F[vertexCount];
				for(int i = 0; i < positions.Length; i++)
					positions[i] = new Vector3F(argumentStep * i, offset, 0);
			}

			public override void InitializeResources(IResourceWriter1D positionsWriter, IResourceWriter1D vertexMasksWriter) =>
				positionsWriter.UpdateResource(positions);

			public void RandomizePoint()
			{
				if (index >= VertexCount)
					index = 0;
				Vector3F point = new Vector3F(argumentStep * index, offset + (float)random.NextDouble(), 0);
				positions[index] = point;
				OnDataChanged(new CustomPositionMaskWriter(point, index));
				index++;
			}
		}

		private readonly AnimationHelper animationHelper = new AnimationHelper();
		private readonly List<CustomSeriesDataReader> dataReaders = new List<CustomSeriesDataReader>();

		public override void Do(IDemoChartControl chartControl)
		{
			List<RenderData> renderDatas = new List<RenderData>();
			for (int i = 0; i < SeriesCount; i++)
			{
				// Create custom data reader.
				var dataReader = new CustomSeriesDataReader(PointCount, 1f, i);
				dataReaders.Add(dataReader);

				// Create series.
				Color4 color = DemoHelper.RandomizeColor();
				var series = new Series
				{
					// Set data reader.
					Reader = dataReader,
					// Set series line color.
					Color = color,
					// Set series line thickness.
					Thickness = 2f,
					// Set series line pattern style.
					PatternStyle = PatternStyle.Solid,
					// Set series marker style.
					MarkerStyle = markers[i % markers.Length],
					// Set marker size.
					MarkerSize = 12,
					// Set marker color.
					MarkerColor = color,
					// Set name.
					Name = $"Line {i}"
				};
				renderDatas.Add(series);
			}

			// Setup chart view settings.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;

			// Tell the chart that we wanna update view on bounds change.
			chartControl.ViewResetOptions.ResetOnDataChanged = true;

			// Setup chart data source.
			chartControl.DataSource = renderDatas;

			// Start animation.
			animationHelper.Start(value => value, value =>
			{
				foreach (CustomSeriesDataReader dataReader in dataReaders)
				{
					dataReader.RandomizePoint();
				}
			}, 0f, 0f, 25);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}
}
