using System;
using System.Linq;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Ray casting with depth", GroupName = "Ray casting", Description = "Demo shows raycasting rendering technique with enabled depth-test flag. You can customize transfer function for ray casting and color legend.")]
	public class VolumeRayCastingDepthDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;
			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Initialization of rendering technique.
			var rayCasting = new VolumeRayCasting
			{
				// Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds. 
				// For dynamic updates of data you can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
				// Byte, Short, Float types are supported.
				Reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max())),
				// Geometry specify bounding box to that volume data will be fitted. Geometry can be more complex than just box. 
				// Mostly it does nothave limits, you can specify even sphere.
				Geometry = new BoxVolumeGeometry
				{
					Origin = Vector3F.Zero,
					Size = new Vector3F(1f),
				},
				// Interpolation type between voxels/
				InterpolationType = VolumeInterpolationType.Linear,
				// Parameter for ray casting technique that will specify how much steps will be on a each ray. 
				// Directly effects performance and render quality. By default it is calculated automatically.
				SamplingStepCount = size,
				// Threshold for transparent areas that will no be visible for hit testing. 
				// 0 will increase picking performance due to item will be picked by bounding box.
				HitTestThreshold = 0.25f,
				// Global value transparency scale.
				ValueScale = 0.5f,
				// Setup depth enabled flag.
				IsDepthTestEnabled = true,
				// Set name.
				Name = "Volume"
			};

			// Inscribed opaque objects.
			var cube1 = new Cube
			{
				Color = Colors.DarkBlue,
				Size = new Vector3F(0.4f),
				Position = new Vector3F(0.25f),
				Name = "Cube 1"
			};
			var cube2 = new Cube
			{
				Color = Colors.DarkGreen,
				Size = new Vector3F(0.4f),
				Position = new Vector3F(0.75f),
				Name = "Cube 2"
			};
			var sphere = new Sphere
			{
				Color = Colors.DarkRed,
				Radius = 0.2f,
				Position = new Vector3F(0.5f),
				Name = "Sphere"
			};

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;

			// Set chart data source. Note: ray-casting with enabled depth-test must be last in the list.
			chartControl.DataSource = new RenderData[] { cube1, cube2, sphere, rayCasting };
		}
	}
}
