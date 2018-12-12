using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Cone", GroupName = "Primitives", Description = "Demo shows example of using cone primitive.")]
	class ConeDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new Cone[]
			{
				new Cone
				{
					//Cone direction
					Direction = Vector3F.UnitZ,
					Color = Colors.Cyan,
					//Position relative to base center
					Position = new Vector3F(),
					//Resolution for radial part of item. Means number of generated points
					Resolution = 20,
					//Height
					Height = 0.5f,
					//Base radius
					Radius = 0.5f,
					//Primitive presentation type
					PresentationType = PrimitivePresentationType.Solid,
					Name = "Cone 1"
				},
				new Cone
				{
					//Cone direction
					Direction = -Vector3F.UnitZ,
					Color = Colors.Blue,
					//Position relative to base center
					Position = new Vector3F(0,0,1),
					//Resolution for radial part of item. Means number of generated points
					Resolution = 200,
					//Height
					Height = 0.5f,
					//Base radius
					Radius = 0.3f,
					//Primitive presentation type
					PresentationType = PrimitivePresentationType.Solid,
					Name = "Cone 2"
				},
				new Cone
				{
					//Cone direction
					Direction = new Vector3F(0,1,0),
					Color = Colors.DarkBlue,
					//Position relative to base center
					Position = new Vector3F(0,-0.5f, 0.5f),
					//Resolution for radial part of item. Means number of generated points
					Resolution = 30,
					//Height
					Height = 0.5f,
					//Base radius
					Radius = 0.1f,
					//Primitive presentation type
					PresentationType = PrimitivePresentationType.Wireframe,
					Name = "Cone 3"
				},
			};
		}
	}
}
