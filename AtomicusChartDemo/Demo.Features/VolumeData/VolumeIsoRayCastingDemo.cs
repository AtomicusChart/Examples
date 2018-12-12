using System;
using System.Linq;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Iso-surface", GroupName = "Iso-Surfacing", Description = "Demo shows isosurfacing via raycasting rendering technique. Non-transparent isu-surfaces support depth output by default. Iso value and color can be changed in legend.")]
	public class VolumeIsoRayCastingDemo : FeatureDemo
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
				IsoValue = reader.ValueRange.Min + (reader.ValueRange.Max - reader.ValueRange.Min) * 0.25f,
				// Specify iso-surface color.
				Color = Colors.Red,
				// For isosurface there is no difference which value is setted. Only 0 or another from 0 have sence.
				// 0 will increase picking performance due to item will be picked by bounding box.
				HitTestThreshold = 0.25f,
				// Set name.
				Name = "Volume"
			};

			// Setup highlight interactor.
			rayCasting.Interactor = new HighlightInteractor(rayCasting);

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
		}

		/// <summary>
		/// This interactor is used for iso-surface highlighting.
		/// </summary>
		private class HighlightInteractor : IInteractor
		{
			private static readonly Color4 HighlightColor = new Color4(Colors.Blue, 75);
			private readonly VolumeRayCastingBase rayCasting;

			public HighlightInteractor(VolumeRayCastingBase rayCasting)
			{
				this.rayCasting = rayCasting;
			}

			public void MouseDown(PickData pickData, IChartEventArg arg)
			{

			}

			public void MouseUp(PickData pickData, IChartEventArg arg)
			{

			}

			public void MouseMove(PickData pickData, IChartEventArgCapturable arg)
			{

			}


			public void MouseLeave(IChartEventArg arg)
			{
				arg.InteractorEventArg.CursorType = CursorType.Arrow;
				rayCasting.HighlightColor = null;
			}

			public void MouseEnter(IChartEventArg arg)
			{
				arg.InteractorEventArg.CursorType = CursorType.HandPointer;
				rayCasting.HighlightColor = HighlightColor;
			}

			public void DoubleClick(PickData pickData, IChartEventArg args)
			{

			}
		}

	}
}
