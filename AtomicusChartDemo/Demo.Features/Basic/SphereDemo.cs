using System;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Spheres", GroupName = "Primitives", Description = "Demo shows example of using sphere primitive.")]
	public class SphereDemo : FeatureDemo
    {
	    public override void Do(IDemoChartControl chartControl)
	    {
			chartControl.Axes.IsAxes3DVisible = true;
		    chartControl.DataSource = new RenderData[]
		    {
				new Sphere
				{
					//Resolution for radial part of item. Means number of generated points. For sphere it is approximate number due to mesh generation algorithm.
					Resolution = 100,
					Color = Colors.DarkBlue,
					//Item radius
					Radius = 0.5f,
					//Item position relative to center
					Position = new Vector3F(),
					Name = "Sphere 1"
				},
			    new Sphere
			    {
				    //Resolution for radial part of item. Means number of generated points. For sphere it is approximate number due to mesh generation algorithm.
				    Resolution = 100,
				    Color = Colors.Cyan,
				    //Item radius
				    Radius = 0.5f,
				    //Ietm position relative to center
				    Position = new Vector3F(0.5f),
					//We can create ellipse using transform matrices
					Transform = Matrix4F.Scaling(1f, 0.5f, 0.5f),
				    Name = "Sphere 2"
				},
			    new Sphere
			    {
				    //Resolution for radial part of item. Means number of generated points. For sphere it is approximate number due to mesh generation algorithm.
				    Resolution = 100,
				    Color = Colors.Blue,
				    //Item radius
				    Radius = 0.5f,
				    //Item position relative to center
				    Position = new Vector3F(0.5f,-0.5f, 0.5f),
				    //We can create ellipse using transform matrices
				    Transform = Matrix4F.Scaling(1f, 1f, 0.5f)*Matrix4F.RotationAxis(Vector3F.UnitX, Math.PI/4),
				    Name = "Sphere 3"
				},
			    new SemiSphere
			    {
				    //Resolution for radial part of item. Means number of generated points. For sphere it is approximate number due to mesh generation algorithm.
				    Resolution = 100,
				    Color = Colors.DarkCyan,
				    //Item radius
				    Radius = 0.55f,
				    //Item position relative to center
				    Position = new Vector3F(),
					//For semi sphere direction can be defined
					Direction = -Vector3F.UnitX,
					//Define if semi sphere base is required
					DrawBottomBase = false,
					//Item presentation type
				    PresentationType = PrimitivePresentationType.Wireframe,
				    Name = "Sphere 4"
				},
			};
		}
	
    }
}
