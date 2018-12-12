using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Cube", GroupName = "Primitives", Description = "Demo shows example of using cube primitive.")]
	class CubeDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new []
			{
				new Cube
				{
					Color = Colors.Cyan,
					//Size of cube sizes
					Size = new Vector3F(1, 1, 0.5f),
					//Position relative to cube center
					Position = new Vector3F(1, 1, -1),
					//Cube direction (Also transform matrix can be used for more accurate rotations)
					Direction = new Vector3F(1, 1, 1),
					//Primitive presentation type
					PresentationType = PrimitivePresentationType.Solid,
					Name = "Cube 1"
				},
				new Cube
				{
					Color = Colors.Blue,
					//Size of cube sizes
					Size = new Vector3F(0.2f, 0.2f, 0.5f),
					//Position relative to cube center
					Position = new Vector3F(0, 0, 0),
					//Transformation matrix example
					Transform = Matrix4F.RotationAxis(Vector3F.UnitX, Math.PI/3)*Matrix4F.RotationAxis(Vector3F.UnitZ, Math.PI/3),
					//Primitive presentation type
					PresentationType = PrimitivePresentationType.Solid,
					Name = "Cube 2"
				},
				new Cube
				{
					Color = Colors.DarkBlue,
					//Size of cube sizes
					Size = new Vector3F(1.2f, 1.2f, 0.7f),
					//Position relative to cube center
					Position = new Vector3F(1, 1, -1),
					//Cube direction (Also transform matrix can be used for more accurate rotations)
					Direction = new Vector3F(1, 1, 1),
					//Primitive presentation type
					PresentationType = PrimitivePresentationType.Wireframe,
					Name = "Cube 3"
				}
			};
		}
	}
}