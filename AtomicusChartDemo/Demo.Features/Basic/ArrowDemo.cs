using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Arrow", GroupName = "Primitives", Description = "Demo shows example of using arrow primitive.")]
	class ArrowDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new[]
			{
				new Arrow
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 200,
					//Item direction vector
					Direction = Vector3F.UnitX,
					//Item position relatively base center
					Position = new Vector3F(0f, 0f, 0f),
					//Item total height
					Height = 2f,
					Color = Colors.Red,
					//Base height / cone height ratio
					BaseToConeHeightRatio = 1.5f,
					//Base radius / cone radius ratio
					BaseToConeRadiusRatio = 0.5f,
					//Name of the arrow
					Name = "Arrow 1"
				},
				new Arrow
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 200,
					//Item direction vector
					Direction = Vector3F.UnitY,
					//Item position relatively base center
					Position = new Vector3F(0f, 0f, 0f),
					//Item total height
					Height = 2f,
					Color = Colors.Green,
					//Base height / cone height ratio
					BaseToConeHeightRatio = 1f,
					//Base radius / cone radius ratio
					BaseToConeRadiusRatio = 0.2f,
					//Name of the arrow
					Name = "Arrow 2"
				},
				new Arrow
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 200,
					//Item direction vector
					Direction = Vector3F.UnitZ,
					//Item position relatively base center
					Position = new Vector3F(0f, 0f, 0f),
					//Item total height
					Height = 2f,
					//Item radius of maximal part (cone or base) depending on BaseToConeRadiusRatio property
					Radius = 0.1f,
					Color = Colors.Blue,
					//Base height / cone height ratio
					BaseToConeHeightRatio = 0.5f,
					//Base radius / cone radius ratio
					BaseToConeRadiusRatio = 0.5f,
					//Name of the arrow
					Name = "Arrow 3"
				},
				new Arrow
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 200,
					//Item direction vector
					Direction = Vector3F.UnitZ,
					//Item position relatively base center
					Position = new Vector3F(0.5f),
					//Item total height
					Height = 0.5f,
					Radius = 0.4f,
					Color = Colors.Cyan,
					//Base height / cone height ratio
					BaseToConeHeightRatio = 0.3f,
					//Base radius / cone radius ratio
					BaseToConeRadiusRatio = 2f,
					//Name of the arrow
					Name = "Arrow 4"
				},
				new Arrow
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 20,
					//Item direction vector
					Direction = Vector3F.UnitZ,
					//Item position relatively base center
					Position = new Vector3F(1.5f, 1.5f, 0f),
					//Item total height
					Height = 2f,
					//Item radius of maximal part (cone or base) depending on BaseToConeRadiusRatio property
					Radius = 0.1f,
					PresentationType = PrimitivePresentationType.Wireframe,
					Color = Colors.DarkBlue,
					//Base height / cone height ratio
					BaseToConeHeightRatio = 0.5f,
					//Base radius / cone radius ratio
					BaseToConeRadiusRatio = 0.5f,
					//Name of the arrow
					Name = "Arrow 5"
				},
			};
		}
	}
}
