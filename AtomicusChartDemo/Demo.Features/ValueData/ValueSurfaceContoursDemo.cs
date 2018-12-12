using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Contours", GroupName = "Contours", Description = "Demo shows example of using contours on surfaces.")]
	class ValueSurfaceContoursDemo : FeatureDemo
	{
		private const int MapSize = 200;

		private class CustomContoursOwner : IContoursOwner
		{
			public CustomContoursOwner(ObservableCollection<Contour> contours) => Contours = contours;

			public event PropertyChangedEventHandler PropertyChanged;

			public event PropertyChangingEventHandler PropertyChanging;

			public ObservableCollection<Contour> Contours { get; }
		}

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate the surface data.
			Vector3F[] positions = DemoHelper.GenerateSinPoints(MapSize);

			// For demo value is a copy of Z axis.
			float[] values = DemoHelper.ExtractZValues(positions, out OneAxisBounds valueRange);

			// Create contours collection.
			var contours = new ObservableCollection<Contour>(DemoHelper.GenerateContours(15, 1f, valueRange));

			// Create the surface data reader.
			var reader = new StructuredValueSurfaceDataReader(positions, values, MapSize, MapSize, valueRange);	

			// Create the surface.
			var surface = new ValueSurface
			{
				// Set surface data reader.
				Reader = reader,
				// Set name.
				Name = "Surface"
			};

			// Create the data that is responsible for contour visualization.
			var contoursData = new TriangleContoursRenderData
			{
				// Set contours computer source.
				DataSource = reader,
				// Set contours collection source.
				ContoursSource = new CustomContoursOwner(contours),
				// Set name.
				Name = "Contours",
			};

			// Setup chart options.
			chartControl.ViewResetOptions.ResetOnDataChanged = false;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.Axes.IsAxes3DVisible = true;

			// Set data source.
			chartControl.DataSource = new RenderData[] { surface, contoursData };			
		}
	}
}
