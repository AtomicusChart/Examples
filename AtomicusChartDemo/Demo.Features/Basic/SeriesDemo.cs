using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.AxesData;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Static series", GroupName = "Series", Description = "This example shows how to create series render data object with default static data reader" +
	                                                                                    " and setup it's settings. This series contains 100.000 points. Also you can see how to use series to" +
	                                                                                    " visualize a row of pixel-dependent point markers. Sqrt scaling is enabled for horizontal axis.")]
	class SeriesDemo : FeatureDemo
	{
		private const int PointsCount = 100000;
		private const int FilteredCount = 100;
		private const float Amplitude = 0.6f;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate points for series.
			var points = new Vector3F[PointsCount];
			for (var i = 0; i < PointsCount; i++)
			{
				float scaledCoord = i / (float)PointsCount;
				points[i] = new Vector3F(
					scaledCoord,
					Amplitude * (float)(Math.Cos(5 * 2 * Math.PI * scaledCoord) + 0.5f),
					Amplitude * (float)(Math.Sin(5 * Math.PI * scaledCoord) + 0.5f));
			}

			// Filter some points for markers.
			var markerPoints = new Vector3F[FilteredCount];
			int stepRate = PointsCount / FilteredCount;
			for (var i = 0; i < FilteredCount; i++)
			{
				markerPoints[i] = points[i * stepRate];
			}

			// Setup chart control view settinsg.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;

			// Create series data reader.
			var reader = new DefaultPositionMaskDataReader(points);

			// Create series render data.
			var series = new Series
			{
				// Set name.
				Name = "Series",
				// Set color.
				Color = Colors.DarkBlue,
				// Set thickness.
				Thickness = 3,
				// Set pattern style.
				PatternStyle = PatternStyle.Solid,
				// Set data reader.
				Reader = reader,
				// Disable marker.
				MarkerStyle = MarkerStyle.None,
			};

			// Create marker series data reader.
			var markerReader = new DefaultPositionMaskDataReader(markerPoints);

			// Create marker series render data.
			var markerSeries = new Series
			{
				// Set name.
				Name = "Markers",
				// Set zero thickness to disable series line.
				Thickness = 0,
				// Set data reader.
				Reader = markerReader,
				// Set marker style as cross.
				MarkerStyle = MarkerStyle.Cross,
				// Set marker size (in pixels).
				MarkerSize = 10,
				// Set marker color.
				MarkerColor = Colors.Blue
			};

			// Setup data scaling for X axis.
			chartControl.Axes.X.DataScale = DataScale.Sqrt;

			// Set chart data source.
			chartControl.DataSource = new RenderData[] { series, markerSeries };
		}
	}
}
