using System.Collections.Generic;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.PresentationData.Primitives;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Pixel-dependent marker 3D", GroupName = "Interaction", Description = "The feature shows usage of pixel-dependent marker presentation data to visualize any data " +
																					   "that initially exists in normalized [0; 1] coordinates as pixel-dependent object. Here you can see " +
																					   "trivial implementation of distance measurement tool using composite render data.")]
	class MarkerDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			// Create our widget.
			var distanceWidget = new DistanceWidget
			{
				Name = "Widget"
			};
			
			// Bounding box.
			var boundingCube = new Cube
			{
				Size = new Vector3F(1f),
				Position = new Vector3F(0.5f),
				Color = Colors.Red,
				PresentationType = PrimitivePresentationType.Wireframe,
				Name = "Bounds"
			};

			// Setup chart options.
			chartControl.Axes.IsAxes3DVisible = true;

			// Setup chart data source.
			chartControl.DataSource = new RenderData[]
			{
				boundingCube, distanceWidget
			};
		}

		/// <summary>
		/// Custom example widget for distance measurement.
		/// </summary>
		private class DistanceWidget : CompositeRenderData
		{
			public DistanceWidget()
			{
				Marker CreateMarker(Vector3F position, Color4 color, int pixelSize, string name)
				{
					// Create marker internal data.
					var markerData = new Sphere
					{
						Color = color,
						// Assign interactor to the render data.
						Interactor = new MarkerInteractor(),
					};

					// Create marker object.
					var marker = new Marker(markerData)
					{
						// Set it's pixel size.
						PixelSize = pixelSize,
						// Set it's translation.
						Transform = Matrix4F.Translation(position),
						// Set it's name.
						Name = name
					};

					// Set render data parent as tag because we'll use it in it's interactor.
					markerData.Tag = marker;

					return marker;
				}

				// Create markers.
				Marker firstMarker = CreateMarker(new Vector3F(0.2f), Colors.Blue, 50, "Marker 1");
				Marker secondMarker = CreateMarker(new Vector3F(0.8f), Colors.Green, 50, "Marker 2");

				// Create distance line.
				var line = new Line
				{
					Color = Colors.Black,
					Thickness = 3.0f,
					Name = "Line",
				};

				Line CreateLabelOffsetLine() =>
					new Line
					{
						Color = Colors.DarkRed,
						Thickness = 1.0f,
						IsLegendVisible = false
					};

				Label CreateInfoLabel() =>
					new Label
					{
						IsLegendVisible = false,
						FontColor = Colors.Black,
						Background = Colors.White,
						BorderColor = Colors.Black,
						MarkerColor = Colors.DarkRed,
						MarkerRadius = 5,
						BorderThickness = 1,
						FontFamily = "Tahoma",
						FontSize = 12
					};

				string ToString(Vector3F position) => $"{position.X:G3}; {position.Y:G3}; {position.Z:G3}";

				// Create label offset lines.
				Line distanceLabelOffsetLine = CreateLabelOffsetLine();
				Line firstLabelOffsetLine = CreateLabelOffsetLine();
				Line secondLabelOffsetLine = CreateLabelOffsetLine();

				// Create info labels.
				Label distanceLabel = CreateInfoLabel();
				Label firstLabel = CreateInfoLabel();
				Label secondLabel = CreateInfoLabel();

				void ResetLinePoints()
				{
					const float labelOffsetLength = 0.4f;

					// Reset line points according to markers positions.
					line.Points = new[] { firstMarker.Transform.GetTranslation(), secondMarker.Transform.GetTranslation() };

					// Reset label position as center of distance line.
					Vector3F lineCenter = (line.Points[0] + line.Points[1]) / 2;
					Vector3F lineDirectionVector = (line.Points[0] - line.Points[1]).GetNormalized();
					Vector3F labelOffsetVector = lineDirectionVector.Cross(Vector3F.UnitZ);
					Vector3F totalOffset = labelOffsetLength * labelOffsetVector;
					distanceLabel.Position = lineCenter + labelOffsetLength * labelOffsetVector;
					firstLabel.Position = firstMarker.Transform.GetTranslation() + totalOffset;
					secondLabel.Position = secondMarker.Transform.GetTranslation() + totalOffset;
					distanceLabelOffsetLine.Points = new[] { lineCenter, lineCenter + totalOffset };
					firstLabelOffsetLine.Points = new[] { line.Points[0], line.Points[0] + totalOffset };
					secondLabelOffsetLine.Points = new[] { line.Points[1], line.Points[1] + totalOffset };

					// Reset label text as distance.
					distanceLabel.Text = (line.Points[0] - line.Points[1]).GetLength().ToString("G4");
					firstLabel.Text = ToString(line.Points[0]);
					secondLabel.Text = ToString(line.Points[1]);
				}

				// Attach property handlers.
				firstMarker.PropertyChanged += (sender, e) =>
				{
					if (e.PropertyName == nameof(RenderData.Transform))
						ResetLinePoints();
					;
				};
				secondMarker.PropertyChanged += (sender, e) =>
				{
					if (e.PropertyName == nameof(RenderData.Transform))
						ResetLinePoints();
				};
				ResetLinePoints();

				// Add elements to the container collection.
				Collection.Add(firstMarker);
				Collection.Add(secondMarker);
				Collection.Add(line);
				Collection.Add(firstLabelOffsetLine);
				Collection.Add(secondLabelOffsetLine);
				Collection.Add(distanceLabelOffsetLine);
				Collection.Add(firstLabel);
				Collection.Add(secondLabel);
				Collection.Add(distanceLabel);
			}

			/// <summary>
			/// Class for base functionality of motion.
			/// </summary>
			private class MarkerInteractor : IInteractor
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
					Vector3F currentLocation = GetPosition(pickData.RenderData);
					Vector3F crossPoint = chartEventArg.CrossWithLookAtPlane(chartEventArg.InteractorEventArg.RelativeLocation);
					Vector3F diff = crossPoint - prevCrossPoint;
					SetPosition(pickData.RenderData, currentLocation + diff);
					prevCrossPoint = crossPoint;
				}

				public void MouseDown(PickData pickData, IChartEventArg chartEventArg)
				{
					prevRelativeLocation = chartEventArg.InteractorEventArg.RelativeLocation;
					prevCrossPoint = chartEventArg.CrossWithLookAtPlane(prevRelativeLocation);
				}

				private static Vector3F GetPosition(RenderData renderData) =>
					((RenderData)renderData.Tag).GetTransformRef().GetTranslation();

				private static void SetPosition(RenderData renderData, Vector3F position)
				{
					var marker = ((RenderData)renderData.Tag);
					marker.Transform = marker.Transform.WithNewTranslation(position);
				}
			}
		}

	}
}
