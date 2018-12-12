using System;
using System.Linq;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.Processing;
using AtomicusChart.Volumes.Data;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Spherical section", GroupName = "Sections", Description = "Demo shows volume sections technique. " +
																			   "Currently use this spherical geometry that presents a border presentation of volumetric data.")]
	class VolumeSectionDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Generates surface positions.
			const int resolution = 50;
			const float r = 0.5f;
			var center = new Vector3F(0.5f);
			var vertex = new Vector3F[resolution * resolution];
			var index = 0;
			for (var i = 0; i < resolution; i++)
			{
				for (var k = 0; k < resolution; k++)
				{
					double t = Math.PI * i / (resolution-1);
					double f = Math.PI * 2 * k / (resolution-1);
					vertex[index++] = new Vector3F((float)(r * Math.Sin(t) * Math.Cos(f)), (float)(r * Math.Sin(t) * Math.Sin(f)), (float)(r * Math.Cos(t))) + center;
				}
			}

			// Section geometry.
			var complexGeometry = new CustomVolumeGeometry(new VolumeMesh(vertex, vertex, GridHelper.GetStructuredTriangleListIndices(0, resolution, resolution, 1)));

			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Byte, Short, Float types are supported.
			var reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max()));

			// Create volume section render data.
			var section = new VolumeSection
			{
				// Set section data reader.
				Reader = reader,
				// Set section geometry.
				Geometry = complexGeometry,
				// Set section interpolation type.
				InterpolationType = VolumeInterpolationType.Linear,
				// Set name.
				Name = "Section"
			};

			// Enable 3D axis.
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data source.
			chartControl.DataSource = new RenderData[] { section };
		}
	}
}
