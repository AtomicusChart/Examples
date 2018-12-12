using System;
using System.Collections.Generic;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Item interaction", GroupName = "Interaction")]
	class DataInteraction : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			var data = new List<Cube>();
			for (int x = 0; x < 3; x++)
				for (int y = 0; y < 3; y++)
					for (int z = 0; z < 3; z++)
					{
						var cube = new Cube
						{
							Position = new Vector3F(x, y, z),
							Size = new Vector3F(0.9f),
							Color = Colors.DarkBlue,
							Interactor = new MyDataInteractor(),
							IsLegendVisible = false,
							Name = "Cube " + z * 9 + y * 3 + x
						};
						data.Add(cube);
					}
			chartControl.Axes.IsAxes3DVisible = false;
			chartControl.DataSource = data;
		}
	}


	/// <inheritdoc />
	/// <summary>
	/// Class for base functionality of motion.
	/// </summary>
	public class MyDataInteractor : IInteractor
	{
		private const CursorType InsideCursorType = CursorType.HandPointer;

		private Vector3F prevCrossPoint;
		private Vector2F prevRelativeLocation;	

		public void MouseLeave(IChartEventArg arg)
		{
			arg.InteractorEventArg.CursorType = CursorType.Arrow;
		}

		public void MouseEnter(IChartEventArg arg)
		{
			arg.InteractorEventArg.CursorType = InsideCursorType;
		}

		public void DoubleClick(PickData pickData, IChartEventArg args)
		{
		}


		public void MouseUp(PickData pickData, IChartEventArg arg)
		{
		}

		public void MouseMove(PickData pickData, IChartEventArgCapturable chartEventArg)
		{
			chartEventArg.Capture();
			Vector3F currentLocation = pickData.RenderData.Transform.GetTranslation();
			Vector3F crossPoint = chartEventArg.CrossWithLookAtPlane(chartEventArg.InteractorEventArg.RelativeLocation);
			
			Vector3F diff = crossPoint - prevCrossPoint;
			pickData.RenderData.Transform = pickData.RenderData.Transform.WithNewTranslation(currentLocation + diff);
			prevCrossPoint = crossPoint;
		}

		/// <inheritdoc />
		public void MouseDown(PickData pickData, IChartEventArg chartEventArg)
		{
			prevRelativeLocation = chartEventArg.InteractorEventArg.RelativeLocation;
			Vector3F clickDataPosition = pickData.RenderData.GetTransformRef().GetTranslation();
			prevCrossPoint = chartEventArg.CrossWithLookAtPlane(prevRelativeLocation);
		}
	}
}
