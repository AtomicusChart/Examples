using System;
using System.Collections.Generic;
using System.Linq;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Volumes.Data;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Default section", GroupName = "Sections", Description = "Demo shows volume sections technique in it's simplest form.")]
	class RectVolumeSectionDemo : FeatureDemo
	{
		private const int SliceCount = 5;

		public override void Do(IDemoChartControl chartControl)
		{

			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Byte, Short, Float types are supported.
			var reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max()));

			// This function is used for volume section geometry creation.
			// We just create rectangular geometry moving it along Z axis.
			VolumeGeometry GetGeometry(float relativePosition)
			{
				// Generates surface positions.
				Vector3F[] positions =
				{
					new Vector3F(0.0f, 0.0f, relativePosition),
					new Vector3F(1f, 0.0f, relativePosition),
					new Vector3F(0.0f, 1f, relativePosition),
					new Vector3F(1f, 1f, relativePosition),
				};
				int[] indices = { 0, 1, 2, 2, 3, 1 };
				return new CustomVolumeGeometry(new VolumeMesh(positions, positions, indices));
			}

			VolumeSection GetSection(float relativePosition, float opacity, string name)
			{
				// Create volume section render data.
				return new VolumeSection
				{
					// Set section data reader.
					Reader = reader,
					// Set section geometry.
					Geometry = GetGeometry(relativePosition),
					// Set section interpolation type.
					InterpolationType = VolumeInterpolationType.Linear,
					// Set opacity,
					Opacity = opacity,
					// Set name.
					Name = name
				};
			}

			// Bounding box.
			var cube = new Cube
			{
				Size = new Vector3F(1f),
				Position = new Vector3F(0.5f),
				Color = Colors.Black,
				PresentationType = PrimitivePresentationType.Wireframe,
				Name = "Bounds"
			};

			// Enable 3D axis.
			chartControl.Axes.IsAxes3DVisible = true;

			// Submit render data-s.
			List<RenderData> renderDatas = new List<RenderData>();
			float sliceStep = 1.0f / (SliceCount + 1);
			for (int i = 0; i < SliceCount; i++)
			{
				float currentPosition = sliceStep * (i + 1);
				renderDatas.Add(GetSection(currentPosition, 1.0f - currentPosition, $"Section {i}"));
			}
			renderDatas.Add(cube);

			// Set chart data source.
			chartControl.DataSource = renderDatas;
		}
	}
}
