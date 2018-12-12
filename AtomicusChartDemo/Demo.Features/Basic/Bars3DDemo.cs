using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.GeometryFactory;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.Collections;
using AtomicusChart.Interface.UtilityTypes;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Bars 3D", GroupName = "Primitives sets", Description = "Demo shows example of primitive collection usage for memory efficient and high-performance primitive rendering." +
	                                                                        "This example is siuted for 3D bars rendering with the help of transformed cubes." +
	                                                                        "To use different colors for each cube use multi-color primitive collection.")]
	class Bars3DDemo : FeatureDemo
	{
		private static readonly Random random = new Random(DateTime.Now.Millisecond);
		private const int GridSize = 10;
		private const int TotalBarCount = GridSize * GridSize;
		private const float BlockSize = 1;
		private const float GridStep = 1.25f;
		private const float MaxHeight = 5;
		
		public override void Do(IDemoChartControl chartControl)
		{
			// Create mesh for rendering. We need a cube. 
			Mesh cubeMesh = CubeMeshFactory.GenerateCube();

			// Generates cube transformation matrixes.
			Matrix4F[] transformations = new Matrix4F[TotalBarCount];
			int index = 0;
			for (int x = 0; x < GridSize; x++)
			{
				for (int y = 0; y < GridSize; y++)
				{
					// Randomize block height.
					float height = (float)random.NextDouble() * MaxHeight;
					// Compute current bar transformation matrix. 
					// Scaling matrix is used for size scaling. Translation matrix is used for positioning.
					transformations[index++] = Matrix4F.Scaling(BlockSize, BlockSize, height) *
					                           Matrix4F.Translation(GridStep * x, GridStep * y, height / 2);
				}
			}

			// Create presentation object.
			var primitiveCollection = new SingleColorPrimitiveCollection
			{
				// Set mesh.
				Mesh = cubeMesh,
				// Set color.
				Color = Colors.Aqua,
				// Set name.
				Name = "Bars",
				// Set custom material.
				Material = new RenderMaterial(0.35f, 0.5f, 0.6f, 0.0f, 0.0f)
			};

			// Set transforms.
			primitiveCollection.SetTransforms(transformations);

			// Set chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set data source.
			chartControl.DataSource = primitiveCollection;
		}
	}
}
