
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Geometry;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Pixelated raster data", GroupName = "Raster", Description = "Demo shows example of using raster data with value. " +
	                                                                   "Raster data is represented by rectangular grid and it's performance is much higher rather than surfaces. " +
	                                                                   "That type is implemented using texturing techniques. Interpolation type may be changed with the help of data editor.")]
	public class PixelatedValueRasterDemo : FeatureDemo
	{
		private const int Resolution = 10;

		public override void Do(IDemoChartControl chartControl)
		{
			// We take the same data as for surfaces for demo purposes, in current case it contains redundant x, y values.
			Vector3F[] xyzPoints = DemoHelper.GenerateSinPoints(Resolution);

			// Extract only values from z array.
			float[] values = DemoHelper.ExtractZValues(xyzPoints);

			// Create geometry.
			var geometry = new RectTextureGeometry
			{
				Origin = Vector3F.Zero,
				DirectionX = Vector3F.UnitX,
				DirectionY = Vector3F.UnitY,
				Size = new Vector2F(1f, 1f)
			};

			var rasterData = new ValueRasterData
			{
				// Disable interpolation to see blocks.
				InterpolationType = RasterDataInterpolationType.None,
				// Float data reader specification (values, data stride/width and value axis bounds).
				Reader = new FloatIntensityImage2DReader(values, Resolution, new OneAxisBounds(-1f, 1f)),
				// Set geometry.
				Geometry = geometry,
				// Set name.
				Name = "Surface"
			};

			// Setup view settings.
			chartControl.ContextView.Mode2D = true;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;

			// Set chart data source.
			chartControl.DataSource = rasterData;
		}
	}
}
