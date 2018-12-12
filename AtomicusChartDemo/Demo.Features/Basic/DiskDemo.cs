using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Disk", GroupName = "Primitives", Description = "Demo shows example of using disk primitive.")]
	class DiskDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			chartControl.Axes.IsAxes3DVisible = true;
			chartControl.DataSource = new Disk[]
			{
				new Disk
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 200,
					//Item position relative to center
					Position = new Vector3F(),
					Color = Colors.Cyan,
					//Item direction vector
					Direction = Vector3F.UnitZ,
					//Item radius
					Radius = 0.5f,
					//Item total height
					Height = 0.4f,
					Name = "Disk 1"
				},
				new Disk
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 20,
					//Item position relative to center
					Position = new Vector3F(0f, 0f, 0.3f),
					Color = Colors.Blue,
					//Item direction vector
					Direction = Vector3F.UnitX,
					//Item radius
					Radius = 0.1f,
					//Item height
					Height = 0.8f,
					Name = "Disk 2"
				},
				new Disk
				{
					//Resolution for radial part of item. Means number of generated points
					Resolution = 100,
					//Item position relative to center
					Position = new Vector3F(0.2f, 0.2f, 0.5f),
					Color = Colors.DarkBlue,
					//Item direction vector
					Direction = new Vector3F(0.5f, 0.5f, 0.5f),
					//Item radius
					Radius = 0.2f,
					//Item height
					Height = 0.04f,
					PresentationType = PrimitivePresentationType.Wireframe,
					Name = "Disk 3"

				},


			};
		}
	}
}
