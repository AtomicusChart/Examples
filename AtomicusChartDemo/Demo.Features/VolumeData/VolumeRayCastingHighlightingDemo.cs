using System;
using System.Linq;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Volumes.DataReaders;
using AtomicusChart.Volumes.Geometry;
using AtomicusChart.Volumes.PresentationData;

namespace AtomicusChart.Demo.Features.VolumeData
{
	[Feature(Name = "Ray casting highlight", GroupName = "Ray casting", Description = "Demo shows raycasting rendering technique. You can customize transfer function for ray casting and color legend.")]
	public class VolumeRayCastingHighlightingDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Demo volumetric binary data from resources.
			byte[] data = Properties.Resources.skull;

			// Size of data is the same in all 3 dimensions.
			var size = (int)Math.Round(Math.Pow(data.Length, 1d / 3d));

			// Create default transfer function and customize it.
			var transferFunction = new DefaultTransferFunction1D(
				new []
				{
					new Vector2DRef(0.2, 0.0),
					new Vector2DRef(0.2, 1.0), 
				});

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
				// The same as HitTestThreshold, but for highlighting. This parameter was set just for demo purposes.
				HighlightThreshold = 0.25f,
				// Global value transparency scale.
				ValueScale = 0.25f,
				// Setup custom transfer function.
				TransferFunction1D = transferFunction,
				// Set name.
				Name = "Volume"
			};

			// Setup highlight interactor.
			rayCasting.Interactor = new HighlightInteractor(rayCasting);

			// Decrease multisampling level to improve interaction experience.
			chartControl.Multisampling = Multisampling.Low2X;

			// Set chart data source.
			chartControl.DataSource = rayCasting;
		}

		/// <summary>
		/// This interactor is used for iso-surface highlighting.
		/// </summary>
		private class HighlightInteractor : IInteractor
		{
			private static readonly Color4 HighlightColor = new Color4(Colors.Blue, 100);
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
