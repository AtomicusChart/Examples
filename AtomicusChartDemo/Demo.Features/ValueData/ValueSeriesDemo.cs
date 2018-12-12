using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Series with value", GroupName = "Series", Description = "Demo shows example of using series with value.")]
	public class ValueSeriesDemo : FeatureDemo
	{
		private const int Resolution = 100;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate simple sin curve. Axis Z as value copy and then we select XZ-projection.
			var points = new Vector3F[Resolution];
			var values = new float[Resolution];
			for (int i = 0; i < Resolution; i++)
			{
				values[i] = (float)Math.Sin((float)i/Resolution * Math.PI*4);
				points[i] = new Vector3F(i, 0, values[i]);
			}

			// Data reader for series.
			var reader = new DefaultPositionValueMaskDataReader(points, values, new OneAxisBounds(-1, 1));

			// Value series data presentation.
			var series = new ValueSeries
			{
				// Set data reader.
				Reader = reader,
				// Marker style.
				MarkerStyle = MarkerStyle.Circle,
				// Line pattern.
				PatternStyle = PatternStyle.Solid,
				// Marker size in pixels, it will be same size in pixels not depending on zoom.
				MarkerSize = 10,
				// Line thickness.
				Thickness = 2f,
				// Set name.
				Name = "Series"
			};

			// We set XZ projection for 2D mode as we use Z axis same as value, and Y axis does not has sense
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosZPos;
			chartControl.ContextView.Mode2D = true;

			// Set chart data source.
			chartControl.DataSource = series;
		}
		
	}
}
