using System;
using System.Windows.Media.Imaging;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Geometry;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.UtilityTypes;
using AtomicusChart.WpfControl;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Bump image mapping", GroupName = "Raster Data", Description = "Demo shows example of raster data with attached bump mapping technique visualization.")]
	class BumpRasterDataDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Load bitmap image.
			var colorTextureSource = new BitmapImage(new Uri(@"pack://application:,,,/Demo.Features;component/Resources/stone01.bmp"));

			// Load bump map image.
			var bumpTextureSource = new BitmapImage(new Uri(@"pack://application:,,,/Demo.Features;component/Resources/bump01.bmp"));
			float ratio = (float)colorTextureSource.PixelHeight / colorTextureSource.PixelWidth;

			// Create default reader for Wpf bitmap sources.
			var reader = new BitmapSourceRasterImage2DReader(colorTextureSource);

			// Create bump reader.
			var bumpReader = new BitmapSourceRasterImage2DReader(bumpTextureSource);

			// Create geometry for both raster data-s.
			var geometry = new RectTextureGeometry
			{
				Origin = Vector3F.Zero,
				DirectionX = Vector3F.UnitX,
				DirectionY = Vector3F.UnitY,
				Size = new Vector2F(1f, ratio)
			};

			// Create non-bump raster data object.
			var rasterData = new RasterData
			{
				// Set image interpolation type.
				InterpolationType = RasterDataInterpolationType.Linear,
				// Set image reader.
				Reader = reader,
				// Set name.
				Name = "Default",
				// Set geometry
				Geometry = geometry
			};

			// Create bump render data object.
			var bumpRasterData = new RasterData
			{
				// Set image interpolation type.
				InterpolationType = RasterDataInterpolationType.Linear,
				// Set image reader.
				Reader = reader,
				// Set bump image reader.
				BumpReader = bumpReader,
				// Set name.
				Name = "Bump",
				// Specify some material custom settings.
				Material = new RenderMaterial(0.25f, 0.6f, 0.6f, 0f, 0f),
				// Set geometry
				Geometry = geometry
			};

			// Setup bump raster data offset matrix.
			bumpRasterData.Transform = Matrix4F.Translation(1.25f, 0, 0);

			// Setup chart data source.
			chartControl.DataSource = new RenderData[] { rasterData, bumpRasterData };
		}
	}
}
