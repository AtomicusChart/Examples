using System;
using System.Collections.Generic;
using System.Linq;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Benzene", GroupName = "Primitives sets", Description = "Demo shows example of combination different primitives into one set.")]
	class Benzene : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			const float distToLarge = 0.5f;
			const float distToSmall = 1.0f;

			var largeCenters = new Vector3F[6];
			var smallCenters = new Vector3F[6];
			// Create positions.
			for (var i = 0; i < 6; i++)
			{
				var cos = (float)Math.Cos(Math.PI / 3 * i);
				var sin = (float)Math.Sin(Math.PI / 3 * i);
				largeCenters[i] = new Vector3F(cos * distToLarge, sin * distToLarge, 0);
				smallCenters[i] = new Vector3F(cos * distToSmall, sin * distToSmall, 0);
			}

			var list = new List<RenderData>();
			AddAtoms(list,
				new[]
				{
					new AtomsDescription{ Col = Colors.Purple, R = 0.57f, Positions = largeCenters},
					new AtomsDescription{ Col = Colors.Gold, R = 0.3f, Positions = smallCenters}
				});

			// Show result.
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = list;
		}

		protected void AddAtoms(IList<RenderData> list, AtomsDescription[] atoms)
		{
			const int resolution = 50;
			foreach (var atom in atoms)
			{
				list.Add(new SphereCollection(atom.Positions.Select(Matrix4F.Translation).ToArray(), atom.R, resolution)
				{
					Color = atom.Col,
					IsLegendVisible = false
				});
			}
		}

		public struct AtomsDescription
		{
			public float R;
			public Color4 Col;
			public Vector3F[] Positions;
		}
	}


	

}
