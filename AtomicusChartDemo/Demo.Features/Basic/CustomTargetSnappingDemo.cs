
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

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Custom target dynamic snapping demo", GroupName = "Snapping", Description = 
		"Demo shows example of chart using custom event listener implementation to enable cursor snapping to user defined snap target.")]
	class CustomTargetSnappingDemo : FeatureDemo
	{
		private const int PointCount = 50;
		private const float AnimationRadius = 0.25f;

		private readonly AnimationHelper animationHelper = new AnimationHelper();

		private Vector3F[] initialPoints;
		private Vector3F[] currentPoints;
		private MySnapTarget snapTarget;
		private IEventListener eventListener;
		private DynamicPositionMaskDataReader dataReader;

		public override void Do(IDemoChartControl chartControl)
		{
			// Generate initial points.
			initialPoints = DemoHelper.GenerateRandomPoints(PointCount, new Vector3F(0f, 0f, 0f), new Vector3F(5f, 5f, 0f));
			currentPoints = new Vector3F[initialPoints.Length];
			Array.Copy(initialPoints, currentPoints, currentPoints.Length);

			// Create check marker.
			var crossMarker = new CrossMarker
			{
				Name = "Marker",
				IsLegendVisible = false
			};

			// Create series dynamic data reader.
			dataReader = new DynamicPositionMaskDataReader(currentPoints.Length);
			dataReader.UpdatePositions(currentPoints);

			// Create series markers.
			var series = new Series
			{
				Reader = dataReader,
				Thickness = 0,
				MarkerColor = Colors.DarkRed,
				MarkerStyle = MarkerStyle.Circle,
				MarkerSize = 8,
				Name = "Points"
			};

			// Create our snap target.
			snapTarget = new MySnapTarget { Points = currentPoints };

			// Regsiter event listener.
			chartControl.ActionController.RegisterHandler(1, eventListener = new ChartEventListener(crossMarker, snapTarget));

			// Setup chart view options.
			chartControl.ContextView.Camera2D.Projection = Projection2DTypes.XPosYPos;
			chartControl.ContextView.Mode2D = true;
			chartControl.ViewResetOptions.ResetOnDataChanged = false;

			// Setup chart data source.
			chartControl.DataSource = new RenderData[] { crossMarker, series };

			// Start animation.
			animationHelper.Start(
				argument =>
				{
					for (int i = 0; i < currentPoints.Length; i++)
					{
						float currentAngle = argument;
						currentPoints[i] = initialPoints[i] +
						                   AnimationRadius * new Vector3F((float) Math.Cos(currentAngle), (float) Math.Sin(currentAngle), 0);
					}
					return argument;
				}, 
				argument =>
				{
					dataReader.UpdatePositions(currentPoints);
					snapTarget.Points = currentPoints;
				}, 0f, 0.025f, 16);
		}

		public override void Stop(IDemoChartControl chartControl)
		{
			// Don't forget to remove event listener when it's not required.
			chartControl.ActionController.RemoveHandler(eventListener);

			// Stop the animation.
			animationHelper.Stop();
		}

		/// <summary>
		/// Simple custom implementation of <see cref="ISnapTarget"/> interface.
		/// </summary>
		private class MySnapTarget: ISnapTarget
		{
			public event EventHandler SnapChange;

			private Vector3F[] points;
			public Vector3F[] Points
			{
				get => points;
				set
				{
					points = value;
					SnapChange?.Invoke(this, EventArgs.Empty);
				}
			}

			public void ExtractSnapPoints(Vector3F point, ISnapContext context)
			{
				if (points == null || points.Length == 0)
					return;
				for(int i = 0; i < points.Length; i++)
					context.SubmitPoint(new SnapData(points[i], this, i));
			}
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
