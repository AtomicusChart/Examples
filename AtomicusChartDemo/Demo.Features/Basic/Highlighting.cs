using System.Collections.Generic;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Highlighting", GroupName = "Interaction")]
	class Highlighting : FeatureDemo
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
							IsLegendVisible = false,
							Name = "Cube " + z * 9 + y * 3 + x
						};
						cube.Interactor = new HighlightInteractor(cube);
						data.Add(cube);
					}
			chartControl.DataSource = data;
		}

		private class HighlightInteractor : IInteractor
		{
			private static readonly Color4 HighlightColor = Colors.Cyan;

			private Color4 colorBeforeHightlight;

			private readonly SingleColorPrimitive singleColorData;

			public HighlightInteractor(SingleColorPrimitive singleColorData)
			{
				this.singleColorData = singleColorData;
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
				singleColorData.Color = colorBeforeHightlight;
			}

			public void MouseEnter(IChartEventArg arg)
			{
				arg.InteractorEventArg.CursorType = CursorType.HandPointer;
				colorBeforeHightlight = singleColorData.Color;
				singleColorData.Color = HighlightColor;
			}

			public void DoubleClick(PickData pickData, IChartEventArg args)
			{

			}
		}
	}
}
