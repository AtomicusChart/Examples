using System.Collections.Generic;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Interface.Processing;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Custom multicolor surface", GroupName = "Primitives", Description = "Demo shows example of using custom multicolor primitive.")]
	public class ColoredDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Enable 3D axes.
			chartControl.Axes.IsAxes3DVisible = true;

			// Setup data source.
			chartControl.DataSource = new MyMulticolorPrimitive
			{
				Name = "My object"
			};
		}

		class MyMulticolorPrimitive : MultiColorPrimitive
		{
			private const int Resolution = 25;

			public override void GetGeometryTransforms(out Matrix4F meshTransform) =>
				meshTransform = Matrix4F.Identity;

			public override ColoredMesh GetMesh()
			{
				// Here we gonna generate mesh with duplicated triangles to avoid vertex color interpolation.
				Vector3F[] initialPositions = DemoHelper.GenerateSinPoints(Resolution);
				int[] initialIndices = GridHelper.GetStructuredTriangleListIndices(0, Resolution, Resolution, 1);

				var vertices = new List<Vector3F>();
				var normals = new List<Vector3F>();
				var indices = new List<int>();
				var colors = new List<Vector3F>();

				int index = 0;
				void SubmitVertex(Vector3F position, Vector3F normal, Vector3F color)
				{
					vertices.Add(position);
					normals.Add(normal);
					colors.Add(color);
					indices.Add(index++);
				}

				int triangleCount = initialIndices.Length / 3;
				for (int i = 0; i < triangleCount; i++)
				{
					Vector3F color = DemoHelper.GetRandomColor();
					Vector3F
						p0 = initialPositions[initialIndices[i * 3]],
						p1 = initialPositions[initialIndices[i * 3 + 1]],
						p2 = initialPositions[initialIndices[i * 3 + 2]];
					Vector3F normal = NormalProcessor.GetNormal(p0, p1, p2);

					SubmitVertex(p0, normal, color);
					SubmitVertex(p1, normal, color);
					SubmitVertex(p2, normal, color);
				}

				int[] indicesArray = indices.ToArray();
				return new ColoredMesh(vertices.ToArray(), colors.ToArray(), normals.ToArray(),
					indicesArray, GridHelper.GetWireframeIndices(indicesArray));
			}
		}
	}
}