using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.ValueData.DataReaders;
using AtomicusChart.ValueData.PresentationData;

namespace AtomicusChart.Demo.Features.ValueData
{
	[Feature(Name = "Contours picking", GroupName = "Contours", Description = "Demo shows example of using contours on surfaces and enable it's picking.")]
	class ValueSurfaceContourPickingDemo : FeatureDemo
	{
		private const int MapSize = 200;

		private class CustomContoursOwner : IContoursOwner
		{
			public CustomContoursOwner(ObservableCollection<Contour> contours) => Contours = contours;

			public event PropertyChangedEventHandler PropertyChanged;

			public event PropertyChangingEventHandler PropertyChanging;

			public ObservableCollection<Contour> Contours { get; }
		}

		private class ContoursRenderDataInteractor : IInteractor
		{
			private readonly Label infoLabel;
			private readonly Func<int, Contour> contourGetter;

			public ContoursRenderDataInteractor(Label infoLabel, Func<int, Contour> contourGetter)
			{
				this.infoLabel = infoLabel;
				this.contourGetter = contourGetter;
			}

			public void MouseDown(PickData pickData, IChartEventArg arg)
			{

			}

			public void MouseUp(PickData pickData, IChartEventArg arg)
			{
				// We're getting contour index as subindex of picked data.
				int contourIndex = pickData.GetRoundedIndex();
				Contour contour = contourGetter(contourIndex);

				infoLabel.IsVisible = true;
				infoLabel.Text = $"Contour: {pickData.GetRoundedIndex()}; Level: {contour.Level:G4}";
			}

			public void MouseMove(PickData pickData, IChartEventArgCapturable arg)
			{
				
			}

			public void MouseLeave(IChartEventArg arg)
			{
				arg.InteractorEventArg.CursorType = CursorType.Arrow;
			}

			public void MouseEnter(IChartEventArg arg)
			{
				arg.InteractorEventArg.CursorType = CursorType.HandPointer;
			}

			public void DoubleClick(PickData pickData, IChartEventArg args)
			{
				
			}
		}

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate the surface data.
			Vector3F[] positions = DemoHelper.GenerateSinPoints(MapSize);

			// For demo value is a copy of Z axis.
			float[] values = DemoHelper.ExtractZValues(positions, out OneAxisBounds valueRange);

			// Create contours collection.
			var contours = new ObservableCollection<Contour>(DemoHelper.GenerateContours(15, 1f, valueRange));

			// Create the surface data reader.
			var reader = new StructuredValueSurfaceDataReader(positions, values, MapSize, MapSize, valueRange);

			// Create the surface.
			var surface = new ValueSurface
			{
				// Set surface data reader.
				Reader = reader,
				// Set name.
				Name = "Surface"
			};

			// Create info label. This label we'll be used to show currently picked contour information.
			var label = new Label
			{
				Position = new Vector3F(0, 0, valueRange.Max + 0.25f),
				Offset = new Vector2F(-0.5f, -0.5f),
				IsVisible = false,
				BorderThickness = 1,
				BorderColor = Colors.Black,
				IsLegendVisible = false,
				FontSize = 18,
				FontFamily = "Tahoma",
				MarkerRadius = 0
			};

			// Create the data that is responsible for contour visualization.
			var contoursData = new TriangleContoursRenderData
			{
				// Set contours computer source.
				DataSource = reader,
				// Set contours collection source.
				ContoursSource = new CustomContoursOwner(contours),
				// Set name.
				Name = "Contours",
				// Since we're using low thickness contours setup additional picking thickness.
				PickingAdditionalThickness = 5,
				// Setup interactor.
				Interactor = new ContoursRenderDataInteractor(label, (contourIndex) => contours[contourIndex])
			};

			// Setup chart options.
			chartControl.ViewResetOptions.ResetOnDataChanged = false;
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.Axes.IsAxes3DVisible = true;

			// Set data source.
			chartControl.DataSource = new RenderData[] { surface, contoursData, label };
		}
	}
}
