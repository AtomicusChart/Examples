using System;
using System.Collections.Generic;
using System.Linq;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.GeometryFactory;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Nanotube", GroupName = "Primitives sets", Description = "Demo shows example of combination different primitives into one set.")]
	class Nanotube : FeatureDemo
	{
		private readonly Color4 linkColor = Colors.LightBlue;
		private readonly Color4 sphereColor = Colors.Gold;

		public override void Do(IDemoChartControl chartControl)
		{
			// Create geometry of nanotube.
			var center = new Vector3F(0f, 0f, 0f);
			const float r = 1f;
			const int length = 20;
			const int width = 10;
			const int pairsCount = width / 2;

			double angleStep = 2 * Math.PI / pairsCount;
			var aAngle = (float)(angleStep / (2 + Math.Sqrt(3)));

			var atomPositions = new List<Vector3F>();
			var zDelta = aAngle * r;
			for (var i = 0; i < length; i++)
			{
				var z = i * zDelta;
				float offset;
				if (i % 2 == 0)
				{
					offset = 0f;
				}
				else
				{
					offset = (float)(aAngle * Math.Sqrt(3) / 2);
				}
				GetRowSpheres(atomPositions, aAngle, new Vector3F(center.X, center.Y, z), width, offset, r);
			}

			var links = new List<Tuple<Vector3F, Vector3F>>();
			for (var i = 0; i < length; i += 2)
			{
				for (var j = 0; j < width - 2; j += 2)
				{
					links.Add(new Tuple<Vector3F, Vector3F>(atomPositions[i * 10 + j + 1], atomPositions[i * 10 + j + 2]));
				}
				links.Add(new Tuple<Vector3F, Vector3F>(atomPositions[i * 10], atomPositions[i * 10 + width - 1]));
			}

			for (var i = 1; i < length; i += 2)
			{
				for (var j = 0; j < width; j += 2)
				{
					links.Add(new Tuple<Vector3F, Vector3F>(atomPositions[i * 10 + j], atomPositions[i * 10 + j + 1]));
				}
			}

			for (var i = 0; i < length - 1; i++)
			{
				for (var j = 0; j < width; j++)
				{
					links.Add(new Tuple<Vector3F, Vector3F>(atomPositions[i * 10 + j], atomPositions[(i + 1) * 10 + j]));
				}
			}

			var list = new List<RenderData>();
			AddAtoms(list,
				new[]
				{
					new Benzene.AtomsDescription{ Col = sphereColor, R = 0.1f, Positions = atomPositions.ToArray() },
				});
			AddTubes(list, new[] {
				new TubesDescription{ Links  = links.ToArray(), R = 0.03f, Col = linkColor}
			});

			// Show result.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = list;
		}

		//We compose several items into collection for improved performance
		protected void AddAtoms(IList<RenderData> list, Benzene.AtomsDescription[] atoms)
		{
			const int resolution = 50;
			foreach (var atom in atoms)
			{
				list.Add(new SphereCollection(atom.Positions.Select(Matrix4F.Translation).ToArray(), atom.R, resolution) { Color = atom.Col, IsLegendVisible = false });
			}
		}

		//We compose several items into collection for improved performance
		protected void AddTubes(IList<RenderData> list, TubesDescription[] tubes)
		{
			const int resolution = 50;
			foreach (var tube in tubes)
			{
				list.Add(new SingleColorLinkCollection(
					tube.Links.Select(x => TubeMeshFactory.GetTubeTransform(x.Item1, x.Item2)).ToArray(), resolution, tube.R)
				{
					Color = tube.Col,
					IsLegendVisible = false
				});
			}
		}

		//No collection for such type, so items are displayed independently
		protected void AddTubes(IList<RenderData> list, LinksDescription[] tubes)
		{
			const int resolution = 50;
			foreach (var tube in tubes)
			{
				foreach (var link in tube.Links)
				{
					list.Add(new Link
					{
						Point1 = link.Item1,
						Point2 = link.Item2,
						Radius = tube.R,
						Resolution = resolution,
						Color1 = tube.Col1,
						Color2 = tube.Col2,
						Ratio = tube.Ratio,
						IsLegendVisible = false
					});
				}
			}
		}


		public struct AtomsDescription
		{
			public float R;
			public Color4 Col;
			public Vector3F[] Positions;
		}


		public struct TubesDescription
		{
			public float R;
			public Color4 Col;
			public Tuple<Vector3F, Vector3F>[] Links;
		}

		public void GetRowSpheres(List<Vector3F> list, float angle, Vector3F center, int width, float offset, float r)
		{
			var position = offset;
			for (var j = 0; j < width / 2; j++)
			{
				if (offset == 0f)
				{
					GetPairSpheres(list, ref position, angle, center, r);
				}
				else
				{
					GetPairSpheresOffset(list, ref position, angle, center, r);
				}
			}
		}


		public void GetPairSpheresOffset(List<Vector3F> list, ref float position, float angle, Vector3F center, float r)
		{
			list.Add(new Vector3F((float)(r * Math.Cos(position)), (float)(r * Math.Sin(position)), center.Z));
			position += angle;
			list.Add(new Vector3F((float)(r * Math.Cos(position)), (float)(r * Math.Sin(position)), center.Z));
			position += (float)(angle * Math.Sqrt(3) + angle);
		}

		public void GetPairSpheres(List<Vector3F> list, ref float position, float a, Vector3F center, float r)
		{
			list.Add(new Vector3F((float)(r * Math.Cos(position)), (float)(r * Math.Sin(position)), center.Z));
			position += (float)(a * Math.Sqrt(3) + a);
			list.Add(new Vector3F((float)(r * Math.Cos(position)), (float)(r * Math.Sin(position)), center.Z));
			position += a;
		}
	}

}
