using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Links & Tube & Cyllinder", GroupName = "Primitives", Description = "Demo shows example of using different link and tube primitives.")]
	class LinkDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;

			var p1 = new Vector3F(-1f, -1f, -1f);
			var p2 = new Vector3F(0f, 0f, 0f);
			var p3 = new Vector3F(1f, 1f, 1f);
			var p4 = new Vector3F(-1f, -1f, 1f);
			chartControl.DataSource = new RenderData[]
			{
				new Sphere
				{
					Position = p1,
					Color = Colors.Red,
					Name = "Sphere 1"
				},
				new Link
				{
					//Point 1 that should be connected via link
					Point1 = p1,
					//Point 2 that should be connected via link
					Point2 = p2,
					//Primitive radius
					Radius = 0.15f,
					Color1 = Colors.Red,
					Color2 = Colors.Purple,
					Name = "Link 1"
				},
				new Sphere
				{
					Position = p2,
					Color = Colors.Purple,
					Name = "Sphere 2"
				},
				new SingleColorLink
				{
					//Point 1 that should be connected via link
					Point1 = p2,
					//Point 2 that should be connected via link
					Point2 = p3,
					Radius = 0.3f,
					Color = Colors.Purple,
					Name = "Link 2"
				},
				new Sphere
				{
					Position = p3,
					Color = Colors.Purple,
					Name = "Sphere 3"
				},
				//Tube has another approach in properties. It is defined by position of center and directon, while link is defined by two points. That is the only difference between Tube and SingleColorLink
				new Tube
				{
					//Primitive radius
					Radius = 1,
					//Position of primitive center.
					Position = p2,
					//Tube height.
					Height = 1,
					//Resolution for radial part of item. Means number of generated points.
					Direction = new Vector3F(0.5f),
					Resolution = 200,
					Color = new Color4(100, 100, 255),
					Name = "Tube"

				},
				new Link
				{
					Resolution = 20,
					//Point 1 that should be connected via link
					Point1 = p1,
					//Point 2 that should be connected via link
					Point2 = p4,
					Radius = 0.15f,
					Color1 = Colors.Red,
					Color2 = Colors.BurlyWood,
					PresentationType = PrimitivePresentationType.Wireframe,
					Name = "Link 3"
				},
				new Sphere
				{
					Position = p4,
					Resolution = 20,
					PresentationType = PrimitivePresentationType.Wireframe,
					Color = Colors.BurlyWood,
					Name = "Sphere 4"
				},
				//Unlike tube cylinder has enclosed sides
				new Cylinder
				{
					Color = Colors.DarkBlue,
					Resolution = 20,
					//Position of primitive center.
					Position = new Vector3F(0.9f, 0.9f, -0.7f),
					//Tube height.
					Height = 1,
					//Resolution for radial part of item. Means number of generated points.
					Direction = Vector3F.UnitZ,
					//Primitive radius
					Radius = 0.2f,
					Name = "Cylinder 1"
				},
				new Cylinder
				{
					Color = Colors.Cyan,
					Resolution = 20,
					//Position of primitive center.
					Position = new Vector3F(0.9f, -0.9f, -0.7f),
					//Tube height.
					Height = 1,
					//Resolution for radial part of item. Means number of generated points.
					Direction = Vector3F.UnitZ,
					//Primitive radius
					Radius = 0.2f,
					PresentationType = PrimitivePresentationType.Wireframe,
					Name = "Cylinder 2"
				},
			};
		}

	}


}
