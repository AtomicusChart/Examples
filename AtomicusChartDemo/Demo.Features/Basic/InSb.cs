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
	[Feature(Name = "InSb", GroupName = "Primitives sets", Description = "Demo shows example of combination different primitives into one set.")]
	public class InSb : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			var inElementColor = Colors.Yellow;
			var sbElementColor = Colors.Blue;
			var borderColor = Colors.White;
			const float atomR = 0.15f;
			const float linkR = 0.02f;
			const float borderLinkR = 0.06f;

			// Top base.
			var point1 = new Vector3F(-1f, 1f, -1f);
			var point2 = new Vector3F(-1f, 1f, 1f);
			var point3 = new Vector3F(1f, 1f, -1f);
			var point4 = new Vector3F(1f, 1f, 1f);

			// Bottom base.
			var point5 = new Vector3F(-1f, -1f, -1f);
			var point6 = new Vector3F(-1f, -1f, 1f);
			var point7 = new Vector3F(1f, -1f, -1f);
			var point8 = new Vector3F(1f, -1f, 1f);

			// Centers of the faces.
			var point9 = new Vector3F((point1.X + point3.X) / 2f, point1.Y, (point1.Z + point2.Z) / 2f);
			var point10 = new Vector3F(point1.X, (point1.Y + point5.Y) / 2f, (point1.Z + point2.Z) / 2f);
			var point11 = new Vector3F(point3.X, (point3.Y + point6.Y) / 2f, (point3.Z + point4.Z) / 2f);
			var point12 = new Vector3F((point5.X + point7.X) / 2f, point5.Y, (point5.Z + point6.Z) / 2f);
			var point13 = new Vector3F((point1.X + point3.X) / 2f, (point1.Y + point5.Y) / 2f, point1.Z);
			var point14 = new Vector3F((point2.X + point4.X) / 2f, (point2.Y + point7.Y) / 2f, point2.Z);

			// Blue points.
			var point15 = new Vector3F((point1.X + point9.X) / 2f, (point1.Y + point13.Y) / 2f, (point1.Z + point9.Z) / 2f);
			var point16 = new Vector3F((point4.X + point9.X) / 2f, (point4.Y + point14.Y) / 2f, (point4.Z + point9.Z) / 2f);
			var point17 = new Vector3F((point6.X + point12.X) / 2f, (point6.Y + point14.Y) / 2f, (point6.Z + point12.Z) / 2f);
			var point18 = new Vector3F((point7.X + point12.X) / 2f, (point7.Y + point13.Y) / 2f, (point7.Z + point12.Z) / 2f);

			var atomCollection1 = new[]
			{
				point1,point2,point3,point4,point5,point6,point7,point8,point9,point10,point11,point12,point13,point14
			};

			var atomCollection2 = new[]
			{
				point15,point16,point17,point18
			};

			//// Links.
			//// Top base.
			var whiteLinks = new Tuple<Vector3F, Vector3F>[12]
			{
				new Tuple<Vector3F, Vector3F>(point1, point2),
				new Tuple<Vector3F, Vector3F > (point1, point3),
				new Tuple<Vector3F, Vector3F > (point3, point4),
				new Tuple<Vector3F, Vector3F > (point2, point4),

				// Bottom base.
				new Tuple<Vector3F, Vector3F > (point5, point6),
				new Tuple<Vector3F, Vector3F > (point5, point7),
				new Tuple<Vector3F, Vector3F > (point6, point8),
				new Tuple<Vector3F, Vector3F > (point7, point8),

				// Links from top to bottom.
				new Tuple<Vector3F, Vector3F > (point1, point5),
				new Tuple<Vector3F, Vector3F > (point2, point6),
				new Tuple<Vector3F, Vector3F > (point3, point7),
				new Tuple<Vector3F, Vector3F > (point4, point8)
			};

			//// Internal links.
			var links = new Tuple<Vector3F, Vector3F>[16]
			{
				new Tuple<Vector3F, Vector3F>(point1, point15),
				new Tuple<Vector3F, Vector3F > (point9, point15),
				new Tuple<Vector3F, Vector3F > (point4, point16),
				new Tuple<Vector3F, Vector3F > (point9, point16),

				new Tuple<Vector3F, Vector3F > (point10, point15),
				new Tuple<Vector3F, Vector3F > (point13, point15),
				new Tuple<Vector3F, Vector3F > (point14, point16),
				new Tuple<Vector3F, Vector3F > (point11, point16),

				new Tuple<Vector3F, Vector3F > (point10, point17),
				new Tuple<Vector3F, Vector3F > (point6, point17),
				new Tuple<Vector3F, Vector3F > (point12, point17),
				new Tuple<Vector3F, Vector3F > (point14, point17),

				new Tuple<Vector3F, Vector3F > (point13, point18),
				new Tuple<Vector3F, Vector3F > (point11, point18),
				new Tuple<Vector3F, Vector3F > (point12, point18),
				new Tuple<Vector3F, Vector3F > (point7, point18)
			};

			var list = new List<RenderData>();
			AddAtoms(list,
				new[]
				{
					new AtomsDescription{ Col = inElementColor, R = atomR, Positions = atomCollection1 },
					new AtomsDescription{ Col = sbElementColor, R =atomR, Positions = atomCollection2 }
				});
			AddTubes(list, new[] {
				new TubesDescription{ Links  = whiteLinks, R = linkR, Col = borderColor}
			});
			AddTubes(list, new[] {
				new LinksDescription{ Links  = links, R = borderLinkR, Col1 = inElementColor, Col2 = sbElementColor, Ratio = 0.5f}
			});
	
			// Show result.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = list;
		}

		//We compose several items into collection for improved performance
		protected void AddAtoms(IList<RenderData> list, AtomsDescription[] atoms)
		{
			const int resolution = 50;
			foreach (var atom in atoms)
			{
				list.Add(new SphereCollection(atom.Positions.Select(Matrix4F.Translation).ToArray(), atom.R, resolution) { Color = atom.Col, IsLegendVisible = false});
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
	}

	public struct AtomsDescription
	{
		public float R;
		public Color4 Col;
		public Vector3F[] Positions;
	}

	public struct LinksDescription
	{
		public float R;
		public Color4 Col1;
		public Color4 Col2;
		public float Ratio;
		public Tuple<Vector3F, Vector3F>[] Links;
	}

	public struct TubesDescription
	{
		public float R;
		public Color4 Col;
		public Tuple<Vector3F, Vector3F>[] Links;
	}
}
