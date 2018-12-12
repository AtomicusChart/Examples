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
	[Feature(Name = "Iso-surface animated", GroupName = "Iso-Surfacing", Description = "Demo shows minimal intensity projection via raycasting rendering technique. Maximal, minimal and average projection types can be changed in legend.")]
	public class VolumeIsoRayCastingAnimatedDemo : FeatureDemo
	{
		private readonly AnimationHelper animationHelper = new AnimationHelper();
		private IntensityImage3DReader reader;

		public override void Do(IDemoChartControl chartControl)
		{
			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds. 
			// For dynamic updates of data you can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
			// Byte, Short, Float types are supported.
			reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max()));

			// Initialization of rendering technique.
			var rayCasting = new VolumeIsoRayCasting
			{
				// Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds. 
				// For dynamic updates of datayou can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
				Reader = reader,
				// Geometry specify bounding box to that volume data will be fitted. Geometry can be more complex than just box. 
				// Mostly it does nothave limits, you can specify even sphere.
				Geometry = new BoxVolumeGeometry
				{
					Origin = Vector3F.Zero,
					Size = new Vector3F(1f),
				},
				// Parameter for ray casting technique that will specify how much steps will be on a each ray. 
				// Directly effects performance and render quality. By default it is calculated automatically.
				SamplingStepCount = size,
				// Interpolation type between voxels.
				InterpolationType = VolumeInterpolationType.Linear,
				// Property that define value level for isosurface. 
				IsoValue = reader.ValueRange.Min + (reader.ValueRange.Max - reader.ValueRange.Min) * 0.5f,
				// Specify iso-surface color.
				Color = new Color4(Colors.Red, 125),
				// For isosurface there is no difference which value is setted. Only 0 or another from 0 have sence.
				// 0 will increase picking performance due to item will be picked by bounding box.
				HitTestThreshold = 0.25f,
				// Set name.
				Name = "Volume"
			};

			// Bounding box.
			var cube = new Cube
			{
				Size = new Vector3F(1f),
				Position = new Vector3F(0.5f),
				Color = Colors.Red,
				PresentationType = PrimitivePresentationType.Wireframe,
				Name = "Bounds"
			};

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;

			// Setup chart data source.
			chartControl.DataSource = new RenderData[] { rayCasting, cube };

			// Start animation.
			animationHelper.Start(
				(argument) => 
				{
					if (argument > reader.ValueRange.Max)
						argument = reader.ValueRange.Min;
					return argument;
				},
				(argument) => { rayCasting.IsoValue = argument; }, reader.ValueRange.Min, 2f, 40);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			animationHelper.Stop();
		}
	}
}
