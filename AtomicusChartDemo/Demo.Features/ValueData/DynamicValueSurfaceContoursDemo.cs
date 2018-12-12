using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Contours performance", GroupName = "Contours", Description = "Demo shows example a performance of using contours on surfaces. " +
	                                                                              "As you can see contours levels and surface values are dynamically changed during the animation.")]
	class DynamicValueSurfaceContoursDemo : FeatureDemo
	{
		private const int Size = 100;
		private const int UpdateInterval = 50;
		private readonly AnimationHelper animationHelper = new AnimationHelper();
		private float[] initialValues;

		private class CustomContoursOwner : IContoursOwner
		{
			public CustomContoursOwner(ObservableCollection<Contour> contours) => Contours = contours;

			public event PropertyChangedEventHandler PropertyChanged;

			public event PropertyChangingEventHandler PropertyChanging;

			public ObservableCollection<Contour> Contours { get; }
		}

		public void RandomizeValues(float[] output, float argument, OneAxisBounds valueRange)
		{
			for (int i = 0; i < initialValues.Length; i++)
			{
				float offset = Math.Min(valueRange.Max - initialValues[i], initialValues[i] - valueRange.Min);
				output[i] = initialValues[i] + (float)Math.Cos(argument) * offset;
			}
		}

		public override void Do(IDemoChartControl chartControl)
		{
			Vector3F[] positions = DemoHelper.GenerateSinPoints(Size);
			initialValues = DemoHelper.ExtractZValues(positions, out OneAxisBounds valueRange);

			// Create contours collection.
			var contours = new ObservableCollection<Contour>(DemoHelper.GenerateContours(10, 1f, valueRange));
			float[] levels = contours.Select(contour => contour.Level).ToArray();

			float[] currentValues = new float[initialValues.Length];
			Array.Copy(initialValues, currentValues, initialValues.Length);

			// Create value surface data reader.
			var reader = new StructuredValueSurfaceDataReader(positions, currentValues, Size, Size, valueRange);

			// Create value surface.
			var surface = new ValueSurface
			{
				// Set data reader.
				Reader = reader,
				// Set name.
				Name = "Surface"
			};

			// Create the data that is pesponsible for contour visualization.
			TriangleContoursRenderData contoursData = new TriangleContoursRenderData(new CustomContoursOwner(contours), reader)
			{
				Name = "Contours"
			};

			// Setup chart options.
			chartControl.ViewResetOptions.ResetOnDataChanged = false;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.Axes.IsAxes3DVisible = true;

			// Set data source.
			chartControl.DataSource = new RenderData[] { surface, contoursData }; 

			// Start animation.
			animationHelper.Start(
				argument =>
				{
					RandomizeValues(currentValues, argument, reader.ValueRange);
					return argument;
				},
				argument =>
				{				
					// Update reader values.
					reader.UpdateValues(currentValues, 0, reader.VertexCount, 0);

					// Update contours levels.
					for(int i = 0; i < contours.Count; i++)
					{
						//contours[i].Level = levels[i] + 0.25f * (float)Math.Cos(argument);
					}
				},
				0, 0.025f * ((float)UpdateInterval / 16), UpdateInterval);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}
}
