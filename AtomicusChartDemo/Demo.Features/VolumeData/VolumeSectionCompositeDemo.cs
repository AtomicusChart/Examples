using System;
using System.Collections.Generic;
using System.Linq;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;
using AtomicusChart.Volumes.PresentationData.Widgets;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Section composite demo", GroupName = "Sections", Description = "Demo shows raycasting rendering technique. You can customize transfer function for ray casting and color legend.")]
	public class VolumeSectionCompositeDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Link to data. Several rendering techniques can use the same data. For reader we should specify link to binary data, slice size, and value axis bounds. 
			// For dynamic updates of data you can implement your own reader, basic reader interface provide neccessary methods for updating separate data regions.
			// Byte, Short, Float types are supported.
			var reader = new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(data.Min(), data.Max()));

			// Geometry specify bounding box to that volume data will be fitted. Geometry can be more complex than just box. 
			// Mostly it does nothave limits, you can specify even sphere.
			var geometry = new BoxVolumeGeometry
			{
				Origin = Vector3F.Zero,
				Size = new Vector3F(1f),
			};

			// Initialize ray-casting.
			var rayCasting = new VolumeRayCasting
			{
				// Setup it's reader. We'll reuse it for section.
				Reader = reader,
				// Setup ray-casting geometry.
				Geometry = geometry,
				// Interpolation type between voxels/
				InterpolationType = VolumeInterpolationType.Linear,
				// Parameter for ray casting technique that will specify how much steps will be on a each ray. 
				// Directly effects performance and render quality. By default it is calculated automatically.
				SamplingStepCount = size,
				// Threshold for transparent areas that will no be visible for hit testing. 
				// 0 will increase picking performance due to item will be picked by bounding box.
				HitTestThreshold = 0.25f,
				// The same as HitTestThreshold, but for highlighting.
				HighlightThreshold = 0.25f,
				// Global value transparency scale.
				ValueScale = 0.3f,
				// Set name.
				Name = "Volume",
				// Don't forget to enable depth-test since we want to visualize section in the ray-casting.
				IsDepthTestEnabled = true
			};

			List<RenderData> renderDatas = new List<RenderData>();
		
			void SubmitSection(float relativeLocation, int id)
			{
				// Initialize volume visual section that takes control over section presentation.
				var visualSection = new VolumeVisualPlaneSection
				{
					// Specify it's parent geometry as ray-casting geometry we want to cross.
					ParentGeometry = geometry,
					// Setup section plane origin.
					Origin = new Vector3F(relativeLocation),
					// Setup section plane normal.
					Normal = new Vector3F(1, 1, 1),
					OutlineThickness = 2.0f,
					OutlineColor = Colors.Black,
					IsOutlineVisible = true,
					FillColor = new Color4(Colors.Blue, 100),
					IsFillVisible = false,
					Name = $"Visual {id}"
				};
				renderDatas.Add(visualSection);

				// Initialize volume section render data.
				var section = new VolumeSection
				{
					// Setup reader.
					Reader = reader,
					// Link the section geometry to it's visual section geometry.
					Geometry = visualSection.SectionGeometry,
					Name = $"Section {id}"
				};
				renderDatas.Add(section);
			}

			// Bounding box.
			var boundingCube = new Cube
			{
				Size = new Vector3F(1f),
				Position = new Vector3F(0.5f),
				Color = Colors.Black,
				PresentationType = PrimitivePresentationType.Wireframe,
				Name = "Bounds"
			};
			renderDatas.Add(boundingCube);

			// Add sections.
			SubmitSection(0.25f, 0);
			SubmitSection(0.5f, 1);
			SubmitSection(0.75f, 2);

			// Place ray-casting as last item to enable depth-test.
			renderDatas.Add(rayCasting);

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;
			chartControl.Axes.IsAxes3DVisible = true;

			// Set chart data source. 
			chartControl.DataSource = renderDatas;
		}
	}
}
