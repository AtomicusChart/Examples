using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Value surface dynamic updates", GroupName = "Surfaces",
		Description =
			"Demo shows example of dynamic value surface data update. Default implementation of data reader is used for dynamic updates, " +
			"but feel free to use your own implementation to gain the best performance for your specific case.")]
	public class DynamicStructuredValueSufraceDemo : FeatureDemo
	{
		private const int Resolution = 100;
		private const int UpdateInterval = 50;
		private readonly AnimationHelper animationHelper = new AnimationHelper();
		private float[] initialValues;

		public float[] GenerateValues(float argument, OneAxisBounds valueRange)
		{
			float[] result = new float[initialValues.Length];
			for (int i = 0; i < initialValues.Length; i++)
			{
				float offset = Math.Min(valueRange.Max - initialValues[i], initialValues[i] - valueRange.Min);
				result[i] = initialValues[i] + (float)Math.Cos(argument) * offset;
			}
			return result;
		}

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate vertex positions.
			Vector3F[] positions = DemoHelper.GenerateSinPoints(Resolution);

			// Generates initial values (for this case extract it as Z coordinates of positions).
			initialValues = DemoHelper.ExtractZValues(positions, out OneAxisBounds valueRange);

			// Current values storage.
			var currentValues = new float[initialValues.Length];
			Array.Copy(initialValues, currentValues, initialValues.Length);

			// Create value surface data reader.
			var reader = new StructuredValueSurfaceDataReader(positions, currentValues, Resolution, Resolution, valueRange);

			// Create value surface presentation data.
			var valueSurface = new ValueSurface
			{
				// Set data reader.
				Reader = reader,
				// Set presentation type.
				PresentationType = ValueSurfacePresentationType.Solid,
				// Set name.
				Name = "Surface"
			};

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.ContextView.Mode2D = false;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;

			// Set chart data source.
			chartControl.DataSource = valueSurface;

			// Start animation.
			animationHelper.Start(
				argument => argument,
				argument =>
				{
					// Generates new values and update reader with it.
					float[] randomizedValues = GenerateValues(argument, reader.ValueRange);
					reader.UpdateValues(randomizedValues, 0, reader.VertexCount, 0);
				},
				0, 0.025f * ((float)UpdateInterval / 16), UpdateInterval);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}

}
