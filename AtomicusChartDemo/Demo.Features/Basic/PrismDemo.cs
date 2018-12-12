using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Prism", GroupName = "Primitives", Description = "Demo shows example of using prism primitive.")]
	class PrismDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new Prism[]
			{
				new Prism
				{
					//Prism side should be defined in 2d space by convex polygon
					Side = new[] {new Vector2F(-0.5f, 0), new Vector2F(-0.5f, 1), new Vector2F(1, 1)},
					//Vector that define translate between top and bottom sides
					BottomToTopVector = Vector3F.UnitZ,
					Color = Colors.Cyan,
					Transform = Matrix4F.RotationAxis(Vector3F.UnitY, -Math.PI/4),
					Name = "Prism 1"
				},
				new Prism
				{
					//Prism side should be defined in 2d space by convex polygon
					Side = new[] {new Vector2F(1,0), new Vector2F(0.4f, 0.5f), new Vector2F(1)},
					//Vector that define translate between top and bottom sides
					BottomToTopVector = new Vector3F(0.5f, 0.5f, 0.5f),
					Color = Colors.Blue,
					Name = "Prism 2"
				}
			};
		}
	}
}
