
using System.Collections.Generic;
using System.ComponentModel;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.DataReaders;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.Interface.Processing.Snapping;
using AtomicusChart.Interface.Processing.Snapping.Contexts;
using AtomicusChart.Interface.Processing.Snapping.Targets;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Line snapping demo", GroupName = "Snapping", Description = "Demo shows example of chart using custom event listener implementation to enable cursor-to-line snapping.")]
	class LineSnappingDemo : FeatureDemo
	{
		private IEventListener eventListener;
		private static readonly float[] Sizes = { 0.4f, 0.6f, 0.8f, 1.0f };
		private static readonly Vector3F ShapeCenter = new Vector3F(0.5f, 0.5f, 0);
		private static readonly Color4[] ShapeColors = {Colors.Red, Colors.Green, Colors.Blue, Colors.Black};

		private static Vector3F[] GetPositions(float size)
		{
			float halfSize = size / 2;
			Vector3F center = ShapeCenter - new Vector3F(halfSize, halfSize, 0);
			return new[]
			{
				center + new Vector3F(0f, halfSize, 0),
				center + new Vector3F(halfSize, size, 0),
				center + new Vector3F(size, halfSize, 0),
				center + new Vector3F(halfSize, 0, 0),
				center + new Vector3F(0f, halfSize, 0),
			};
		}

		private static Series GetSeries(Vector3F[] positions, Color4 color)
		{
			return new Series
			{
				Reader = new DefaultPositionMaskDataReader(positions),
				Thickness = 2.0f,
				Color = color,
				MarkerStyle = MarkerStyle.Circle,
				MarkerSize = 8,
				MarkerColor = color
			};
		}

		public override void Do(IDemoChartControl chartControl)
		{
			// Create check marker.
			var crossMarker = new CrossMarker
			{
				Name = "Marker",
				IsLegendVisible = false
			};

			// Here we'll store our render datas and its snap targets.
			var snapTargets = new List<ISnapTarget>();
			var renderDatas = new List<RenderData> {crossMarker};
;
			// Create some visual data.
			for (int i = 0; i < Sizes.Length; i++)
			{
				// Generate vertices.
				Vector3F[] vertices = GetPositions(Sizes[i]);

				// Create series.
				Series series = GetSeries(vertices, ShapeColors[i]);
				series.Name = $"Series {i}";
				renderDatas.Add(series);

				// Create line snap target.
				var lineSnapTarget = new LineSnapTarget(vertices, true)
				{
					// Specify vertex snap distance (measured in snap context distance units).
					VertexSnapDistance = 0.025f
				};
				snapTargets.Add(lineSnapTarget);
			}

			// Create colletion snap target since we want to snap to several lines.
			var snapTarget = new CollectionSnapTarget(snapTargets);

			// Regsiter event listener.
			chartControl.ActionController.RegisterHandler(1, eventListener = new ChartEventListener(crossMarker, snapTarget));

			// Setup chart view options.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;
			chartControl.ViewResetOptions.ResetOnDataChanged = false;

			// Setup chart data source.
			chartControl.DataSource = renderDatas;
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			// Don't forget to remove event listener when it's not required.
			chartControl.ActionController.RemoveHandler(eventListener);
		}

		/// <summary>
		/// The class is used to describe a single billboarding marker.
		/// </summary>
		public class CrossMarker : Series
		{
			private readonly DynamicPositionMaskDataReader dataReader;

			public CrossMarker()
			{
				Reader = dataReader = new DynamicPositionMaskDataReader(1);
				// Set zero thickness to disable line.
				Thickness = 0.0f;
				// Set marker style.
				MarkerStyle = MarkerStyle.Cross;
				// Set marker size.
				MarkerSize = 20;
				// Set marker color.
				MarkerColor = Colors.DarkBlue;
			}

			private Vector3F position;
			public Vector3F Position
			{
				get => position;
				set
				{
					if (position == value)
						return;
					position = value;
					dataReader.UpdatePositions(value, 0);
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// This event listener implement snapping logic.
		/// </summary>
		private class ChartEventListener : IEventListener
		{
			private readonly CrossMarker marker;
			private readonly SnapManager snapManager;
			private Vector3F snapPosition;
			private Vector3F currentPosition;

			public ChartEventListener(CrossMarker marker, ISnapTarget snapTarget)
			{
				this.marker = marker;

				// Create snap manager - object responsible for snap result tracking.
				snapManager = new SnapManager(snapTarget, new DefaultSnapContext
				{
					// Specify hybrid snap mode.
					SnapMode = SnapMode.Hybrid,
					// Set snap distance (measured in snap context distance units).
					SnapDistance = 0.075f
				})
				{ Active = true };
				snapManager.PropertyChanged += SnapManager_PropertyChanged;
			}

			private void SnapManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				// We're gonna track CurrentSnapData property to be notified about snap result changes.
				if (e.PropertyName == nameof(SnapManager.CurrentSnapData))
				{
					UpdateSnap(); // TODO: update.
				}
			}

			public void MouseDown(IChartEventArg arg)
			{

			}

			public void MouseUp(IChartEventArg arg)
			{

			}

			public void MouseMove(IChartEventArgCapturable arg)
			{
				marker.IsVisible = true;
				currentPosition = arg.CrossWithLookAtPlane();

				// Update snap position.
				snapManager.UpdateSnap(currentPosition);
				UpdateSnap();
			}

			public void MouseEnter(IChartEventArg arg)
			{
				// Activate snap manager.
				snapManager.Active = true;
			}

			public void MouseLeave(IChartEventArg arg)
			{
				marker.IsVisible = false;

				// Deactivate snap manager.
				snapManager.Active = false;
			}

			public void MouseDoubleClick(IChartEventArg arg)
			{

			}

			public void MouseWheel(IChartEventArg arg)
			{

			}

			private void UpdateSnap()
			{
				if (snapManager.CurrentSnapData.HasValue)
				{
					snapPosition = snapManager.CurrentSnapData.Value.Point;
					marker.Position = snapPosition;
				}
				else
				{
					snapPosition = currentPosition;
					marker.Position = currentPosition;
				}
			}
		}
	}
}
