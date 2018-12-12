using System;
using System.Linq;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Average Intensity", GroupName = "Intensity projections", Description = "Demo shows average intensity projection via raycasting rendering technique. Maximal, minimal and average projection types can be changed in legend.")]
	class VolumeIPRayCastingDemoAverage : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Initialization of rendering technique.
			var rayCasting = new VolumeIPRayCasting
			{
				// Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds.
				// For dynamic updates of datayou can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
				Reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max())),
				// Geometry specify bounding box to that volume data will be fitted. 
				// Geometry can be more complex than just box. Mostly it does nothave limits, you can specify even sphere.
				Geometry = new BoxVolumeGeometry
				{
					Origin = Vector3F.Zero,
					Size = new Vector3F(1f),
				},
				// Parameter for ray casting technique that will specify how much steps will be on a each ray. 
				// Directly effects performance and render quality. By default it is calculated automatically.
				SamplingStepCount = size,
				// Intensity projection type.
				ProjectionType = VolumeIPType.Average,
				// Interpolation type between voxels.
				InterpolationType = VolumeInterpolationType.Linear,
				// Set name.
				Name = "Volume"
			};

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;

			// Set chart data source.
			chartControl.DataSource = rayCasting;
		}
	}
}