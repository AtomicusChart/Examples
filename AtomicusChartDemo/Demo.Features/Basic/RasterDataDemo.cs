using System;
using System.Windows.Media.Imaging;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Geometry;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.WpfControl;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Static logo image", GroupName = "Raster Data", Description = "Demo shows example of most common raster data usage.")]
	class RasterDataDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Load bitmap image.
			var bitmapSource =
				new BitmapImage(new Uri(@"pack://application:,,,/Demo.Features;component/Resources/Atomicus.jpg"));
			float ratio = (float)bitmapSource.PixelHeight / bitmapSource.PixelWidth;

			// Create default reader for Wpf bitmap sources.
			var reader = new BitmapSourceRasterImage2DReader(bitmapSource);

			// Create geometry.
			var geometry = new RectTextureGeometry
			{
				Origin = Vector3F.Zero,
				DirectionX = Vector3F.UnitX,
				DirectionY = Vector3F.UnitY,
				Size = new Vector2F(1f, ratio)
			};

			// Create raster data object.
			var rasterData = new RasterData
			{
				// Set image interpolation type.
				InterpolationType = RasterDataInterpolationType.Linear,
				// Set image reader.
				Reader = reader,
				// Set name.
				Name = "Image",
				// Set geometry.
				Geometry = geometry
			};

			// Setup some chart view settings.
			chartControl.ContextView.Mode2D = true;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;

			// Setup chart data source.
			chartControl.DataSource = rasterData;
		}
	}
}
