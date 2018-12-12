using System;
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
	[Feature(Name = "Points snapping demo", GroupName = "Snapping", Description = "Demo shows example of chart using custom event listener implementation to enable cursor snapping.")]
	class PointsSnappingDemo : FeatureDemo
	{
		private IEventListener eventListener;

		public override void Do(IDemoChartControl chartControl)
		{
			// Create check marker.
			var crossMarker = new CrossMarker
			{
				Name = "Marker",
				IsLegendVisible = false
			};

			// Randomize points.
			Vector3F[] points = DemoHelper.GenerateRandomPoints(100, new Vector3F(0f, 0f, 0f), new Vector3F(5f, 5f, 0f));

			// Create series markers.
			var series = new Series
			{
				Reader = new DefaultPositionMaskDataReader(points),
				Thickness = 0,
				MarkerColor = Colors.DarkRed,
				MarkerStyle = MarkerStyle.Circle,
				MarkerSize = 8,
				Name = "Points"
			};

			// Create snap target.
			var snapTarget = new PointCollectionSnapTarget(points);

			// Regsiter event listener.
			chartControl.ActionController.RegisterHandler(1, eventListener = new ChartEventListener(crossMarker, snapTarget));

			// Setup chart view options.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;
			chartControl.ViewResetOptions.ResetOnDataChanged = false;

			// Setup chart data source.
			chartControl.DataSource = new RenderData[] { crossMarker, series };
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
					SnapMode = SnapMode.SnapOnly
				})
				{ Active = true };
				snapManager.PropertyChanged += SnapManager_PropertyChanged;
			}

			private void SnapManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				// We're gonna track CurrentSnapData property to be notified about snap result changes.
				if (e.PropertyName == nameof(SnapManager.CurrentSnapData))
				{
					if (snapManager.CurrentSnapData.HasValue)
					{
						snapPosition = snapManager.CurrentSnapData.Value.Point;
						marker.Position = snapPosition;
					}
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
		}
	}
}
