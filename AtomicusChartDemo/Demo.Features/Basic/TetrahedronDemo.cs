using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Tetrahedron & Pyramid", GroupName = "Primitives", Description = "Demo shows example of using tetrahedron and pyramid primitive.")]
	class TetrahedronDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new RenderData[]
			{

				new Tetrahedron
				{
					Color = Colors.Cyan, 
					//Relative to item center position
					Position = new Vector3F(0, 0, 0.5f),
					//Item direction
					Direction = Vector3F.UnitZ, 
					Name = "Tetrahedron"
				},
				//Pyramid with height = 0.5 and orientation via axis Z. This primitive is fully defined via transformation matrices
				new Pyramid
				{
					Color = Colors.Blue,
					Transform = Matrix4F.Scaling(0.5f, 0.5f, 1f) * Matrix4F.RotationFromDirection(-Vector3F.UnitZ),
					Name = "Pyramid"
				}
			};
		}

	}
}
