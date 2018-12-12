using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Structured grid surface", GroupName = "Surfaces", Description = "Demo shows example of using structured grid surface primitive. ")]
	class StructuredValueSurface : FeatureDemo
	{
		private const int MapSize = 200;

		public override void Do(IDemoChartControl chartControl)
		{		
			// Generate positions.
			Vector3F[] positions = DemoHelper.GenerateSinPoints(MapSize);

			// Creation of surface data presentation.
			var surface = new ValueSurface
			{
				// Data reader approach is used to improve performance for big data sets and their updates.
				Reader = new StructuredValueSurfaceDataReader(
					positions, // Surface positions.
					DemoHelper.ExtractZValues(positions, out OneAxisBounds valueBounds),// Surface values, for demo purposes values are extracted from Z position component, but in real world they are independent, size of array should be the same as for positions.
					MapSize, // Width and height are required for triangulation of structured grid.
					MapSize,
					valueBounds), //Bounds of value axes.
				// Set presentation option.
				PresentationType = ValueSurfacePresentationType.SolidAndWireframe,
				// Set name.
				Name = "Surface"
			};

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;
			
			// Setup chart data source.
			chartControl.DataSource = surface;
		}
	}
}
