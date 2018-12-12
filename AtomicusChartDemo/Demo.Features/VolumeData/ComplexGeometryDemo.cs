using System;
using System.Linq;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Processing;
using AtomicusChart.Volumes.Data;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Complex volume geometry", GroupName = "Custom geometies", Description = "Demo shows vareity of geometries for volume rendering. Sphere geometry is easily created and applied to ray casting technique. The same geometry aproach can be used for all volume rendering techniques.")]
	public class ComplexGeometryDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;
			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Very simple sphere generation algorithm.
			const int resolution = 50;
			const float r = 0.5f;
			var center = new Vector3F(0.5f);
			var vertex = new Vector3F[resolution * resolution];
			int index = 0;
			for (var i = 0; i < resolution; i++)
			{
				// We use here inversed order due to triangle list indicies generation algorythm. 
				// It is very important to use correct counterclockwise triangle indices generation for raycasting.
				for (int k = resolution - 1; k >= 0; k--)
				{
					var t = Math.PI * i / (resolution - 1);
					var f = Math.PI * 2 * k / (resolution - 1);
					vertex[index++] = new Vector3F((float)(r * Math.Sin(t) * Math.Cos(f)), (float)(r * Math.Sin(t) * Math.Sin(f)), (float)(r * Math.Cos(t))) + center;
				}
			}

			// Section geometry.
			var complexGeometry = new CustomVolumeGeometry(new VolumeMesh(vertex, vertex, GridHelper.GetStructuredTriangleListIndices(0, resolution, resolution, 1)));

			// Initialization of rendering technique.
			var rayCasting = new VolumeRayCasting
			{
				//Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds. 
				//For dynamic updates of data you can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
				//Byte, Short, Float types are supported.
				Reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max())),
				Geometry = complexGeometry,
				//Interpolation type between voxels
				InterpolationType = VolumeInterpolationType.Linear,
				//Parameter for ray casting technique that will specify how much steps will be on a each ray. Directly effects performance and render quality. By default it is calculated automatically.
				SamplingStepCount = size,
				//Threshold for transparent areas that will no be visible for hit testing. 0 will increase picking performance due to item will be picked by bounding box.
				HitTestThreshold = 0.25f,
				//Global value transparency scale
				ValueScale = 0.5f
			};

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Setup chart data source.
			chartControl.DataSource = rayCasting;
		}
	}
}
