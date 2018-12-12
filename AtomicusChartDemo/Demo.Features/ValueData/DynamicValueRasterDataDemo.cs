using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.Geometry;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Raster data dynamic updates", GroupName = "Raster", Description = "Demo shows example of dynamic raster data update. Custom reader is implemented to provide random values.")]
	public class DynamicValueRasterDataDemo : FeatureDemo
	{
		/// <summary>
		/// Custom reader for value generating.
		/// </summary>
		public class MyRasterDataReader : IntensityImage2DReader
		{
			// Value data array.
			private readonly float[] data;
			// Data stride/width. Used to convert one dimensional array to two dimensional.
			private readonly int stride;

			public MyRasterDataReader(int width, int height, OneAxisBounds valueRange) :
				base(width, height, Intensity2DFormat.Float)
			{
				data = new float[width * height];
				ValueRange = valueRange;
				stride = sizeof(float) * width;
			}
			
			// First data initialization.
			public override void InitializeResource(IResourceWriter2D resourceWriter) =>
				UpdateResource(data, resourceWriter);

			public void RandomizeValues(float argument)
			{
				float[] RandomFloatData()
				{
					var result = new float[Description.Width * Description.Height];
					for (int i = 0; i < data.Length; i++)
					{
						int x = i % Description.Width, y = i / Description.Height;
						result[i] = (float)(ValueRange.Min + Math.Cos(argument * x) * Math.Sin(y * argument));
					}
					return result;
				}
				OnDataChanged(new DelegateWriter2D(writer => { UpdateResource(RandomFloatData(), writer); }));
			}

			private void UpdateResource(float[] newData, IResourceWriter2D resourceWriter) =>
				resourceWriter.UpdateResource(newData, stride);
		}

		private readonly int Width = 25, Height = 25;
		private readonly AnimationHelper animation = new AnimationHelper();

		public override void Do(IDemoChartControl chartControl)
		{
			// Create custom data reader.
			var floatReader = new MyRasterDataReader(Width, Height, new OneAxisBounds(0, 1, 0, 0.1f));

			// Create raster data object.
			var floatRasterData = new ValueRasterData
			{
				// Enable value linear interpolation.
				InterpolationType = RasterDataInterpolationType.Linear,
				// Set data reader.
				Reader = floatReader,
				// Set geometry.
				Geometry = new RectTextureGeometry
				{
					Origin = Vector3F.Zero,
					Size = new Vector2F(1f, 1f)
				},
				// Set name.
				Name = "Image"
			};

			// Setup view settings.
			chartControl.ContextView.Mode2D = true;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;

			// Set data source.
			chartControl.DataSource = new RenderData[] { floatRasterData };

			// Start animation.
			animation.Start(
				(f) => f > 1 ? 0 : f,
				rf => floatReader.RandomizeValues(rf),
				0, 0.00025f, 16);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animation.Stop();
		}
	}
}
