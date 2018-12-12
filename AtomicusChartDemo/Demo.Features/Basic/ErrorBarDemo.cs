using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Default error bars", GroupName = "Error Bars", Description = "Demo shows example of using error bars with Bars type.")]
	public class ErrorBarDemo : FeatureDemo
	{
		private const int PointCount = 110;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate error bar data as a set of error points.
			ErrorPoint[] data = DemoDataHelper.GetStripData(-5, 5, PointCount,
				arg => new ErrorPoint(
					new Vector3F(arg, 0, 2 + (float)Math.Sin(arg) * (float)Math.Sin(arg)),
					(float)Math.Sin(arg * 21) / 15 - 0.1f,
					(float)Math.Sin(arg * 29) / 15 + 0.1f)
			);

			// Create error bar render data.
			var errorBars = new ErrorBars
			{
				// Set data.
				Data = data,
				// Set color.
				Color = Colors.BlueViolet,
				// Set bar types.
				ErrorBarType = ErrorBarType.Bars,
				// Set type.
				Name = "Errors",
			};

			chartControl.ContextView.Mode2D = true;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosZPos;
			chartControl.DataSource = errorBars;
		}
	}
}
