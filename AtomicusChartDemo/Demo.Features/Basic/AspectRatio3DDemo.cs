using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.PresentationData;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Aspect Ratio 3D", GroupName = "Custom Features", Description = "Demo shows example of usage 3D aspect ratio feature that may be used for data scaling to fit it according to specific rules.")]
	class AspectRatio3DDemo : FeatureDemo
	{
		private static readonly Random Random = new Random(DateTime.Now.Millisecond);
		private const int PointsCount = 100000;
		private const float MaxRadius = 5;
		private const float ScalingY = 3;
		private const float ScalingZ = 10;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate positions. Note: making Z and Y positions scaled.
			var positions = new Vector3F[PointsCount];
			for (int i = 0; i < PointsCount; i++)
			{
				positions[i] = new Vector3F(
					(float)Random.NextDouble() * MaxRadius,
					(float)Random.NextDouble() * MaxRadius * ScalingY,
					(float)Random.NextDouble() * MaxRadius * ScalingZ);
			}

			// Creation of data presentation.
			var points = new SingleColorPoints
			{
				// Reader approach is used to improve performance for big data sets and their updates.
				Reader = new DefaultPositionMaskDataReader(positions),
				// Set points color.
				Color = Colors.DarkBlue,
				// Set name.
				Name = "Points"
			};

			// Setup camera 3D aspect ratio.
			// Here we specify that we want to fit our data into [1; 1; 1] cube.
			chartControl.ContextView.Camera3D.AspectRatio = new AspectRatio(PreferableAxis.Z, new Vector3<float?>(1, 1, 1));

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data source.
			chartControl.DataSource = points;
		}
	}
}
