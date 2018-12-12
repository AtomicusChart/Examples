using System;
using System.Linq;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Irregular surface", GroupName = "Surfaces", Description = "Demo shows example of using irregular in 2D space surface primitive. " +
	                                                                           "Effective Delaunay2D triangulation algorithm is used.")]
	class IrregularValueSurface : FeatureDemo
	{
		private const int size = 100;

		public override void Do(IDemoChartControl chartControl)
		{	
			var random = new Random(DateTime.Now.Millisecond);

			// Generate demo structured surface.
			Vector3F[] positions = DemoHelper.GenerateSinPoints(size);

			// Randomly remove points from that grid for irregular grid, only 30% will be taken from original grid.
			Vector3F[] irregularPositions = positions.Where(t => random.NextDouble() > 0.7).ToArray();
			// Surface data presentation creation.
			var surface = new ValueSurface
			{
				// Reader can be reused and we can create several presentations for 1 reader.
				// Note: this reader provider default implementation, so feel free to implement your own logic.
				Reader = new IrregularValueSurfaceDataReader(
					irregularPositions, // Define surface points.
					DemoHelper.ExtractZValues(irregularPositions, out OneAxisBounds valueBounds), // Surface values, here they are equal to Z values .
					2, // As data is irregular in 2D space we should specify exclude axis index for triangulation.
					valueBounds), //Value axis bounds.
				// Setup presentation option.
				PresentationType = ValueSurfacePresentationType.SolidAndWireframe,
				// Set name.
				Name = "Surface"
			};

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;

			// Set data source.
			chartControl.DataSource = surface;
		}
	}
}
