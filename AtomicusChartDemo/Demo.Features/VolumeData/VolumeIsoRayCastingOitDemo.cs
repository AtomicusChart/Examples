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
	[Feature(Name = "Iso-surface transparency (OIT)", GroupName = "Iso-Surfacing", Description = "Demo shows order-independent transparency support for isosurface rendering technique.")]
	public class VolumeIsoRayCastingOitDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Order-Independent transparency requres additional GPU resources, it should be used only when neccessary. 
			// Multisample antialiasing and control resolution have significant effect to OIT performance.
			chartControl.IsOitEnabled = true;

			// Demo volumetric binary data from resources
			var data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds. 
			// For dynamic updates of data you can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
			// Byte, Short, Float types are supported. The reader will be used for both ray-casting for efficient memory usage.
			var reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max()));

			// Create volume geometry object (we'll reuse it for both ray-castings for efficient memory usage).
			var geometry = new BoxVolumeGeometry
			{
				Origin = Vector3F.Zero,
				Size = new Vector3F(1f)
			};

			float ComputeRelativeIsoValue(float relativeValue) =>
				reader.ValueRange.Min + (reader.ValueRange.Max - reader.ValueRange.Min) * relativeValue;

			VolumeIsoRayCasting CreateRayCasting(float relativeLevel, Color4 color, string name)
			{
				return new VolumeIsoRayCasting
				{
					// Set ray-casting data reader.
					Reader = reader,
					// Set ray-casting border geometry.
					Geometry = geometry,
					// Set value interpolation type.
					InterpolationType = VolumeInterpolationType.Linear,
					// Set iso-value.
					IsoValue = ComputeRelativeIsoValue(relativeLevel),
					// Set surface color.
					Color = color,
					// Set hit-test threshold value.
					HitTestThreshold = 0.25f,
					// Set sampling step count.
					SamplingStepCount = 500,
					// Set name.
					Name = name
				};
			}

			// Create first ray-casting.
			VolumeIsoRayCasting rayCasting = CreateRayCasting(0.4f, new Color4(Colors.Red, 125), "Volume 1");

			// Create second ray-casting.
			VolumeIsoRayCasting rayCasting2 = CreateRayCasting(0.15f, new Color4(Colors.White, 125), "Volume 2");

			// Create internal cube.
			var cube = new Cube
			{
				Size = new Vector3F(0.5f),
				Position = new Vector3F(0.5f),
				Color = new Color4(Colors.Green, 150),
				Name = "Cube"
			};

			// Create border cube.
			var borderCube = new Cube
			{
				Size = new Vector3F(1f),
				Position = new Vector3F(0.5f),
				Color = Colors.Red,
				PresentationType = PrimitivePresentationType.Wireframe,
				Name = "Bounds"
			};

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;

			// Set chart data source.
			chartControl.DataSource = new RenderData[] { rayCasting, cube, borderCube, rayCasting2 };
		}
	}
}
