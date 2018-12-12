using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Torus", GroupName = "Primitives", Description = "Demo shows example of using torus primitive.")]
	class TorusDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new[]
			{
				new Torus
				{
					//Primitive radius of bigger part
					Radius = 10,
					//Primitive radius of small part
					RadiusSmall = 2,
					//Resolution of torus (number or generated circles)
					Resolution = 20,
					//Resolution of generated circle
					ResolutionSmall = 10,
					//Item direction
					Direction = new Vector3F(-1, -1, 0),
					//Item position relative to center
					Position = new Vector3F(1, 1, 0),
					PresentationType = PrimitivePresentationType.Wireframe,
					Name = "Torus big"

				},
				new Torus
				{
					//Primitive radius of bigger part
					Radius = 5,
					//Primitive radius of small part
					RadiusSmall = 1,
					//Resolution of torus (number or generated circles)
					Resolution = 200,
					//Resolution of generated circle
					ResolutionSmall = 20,
					//Item direction
					Direction = Vector3F.UnitX,
					Color = Colors.Red,
					//Item position relative to center
					Position = new Vector3F(1, 1, 0),
					Name = "Torus X"

				},
				new Torus
				{
					//Primitive radius of bigger part
					Radius = 5,
					//Primitive radius of small part
					RadiusSmall = 1,
					//Resolution of torus (number or generated circles)
					Resolution = 200,
					//Resolution of generated circle
					ResolutionSmall = 20,
					//Item direction
					Direction = Vector3F.UnitY,
					Color = Colors.Green,
					//Item position relative to center
					Position = new Vector3F(1, 1, 0),
					Name = "Torus Y"
				},
				new Torus
				{
					//Primitive radius of bigger part
					Radius = 5,
					//Primitive radius of small part
					RadiusSmall = 1,
					//Resolution of torus (number or generated circles)
					Resolution = 200,
					//Resolution of generated circle
					ResolutionSmall = 20,
					//Item direction
					Direction = Vector3F.UnitZ,
					Color = Colors.Blue,
					//Item position relative to center
					Position = new Vector3F(1, 1, 0),
					Name = "Torus Z"
				},

			};
		}
	}
}
