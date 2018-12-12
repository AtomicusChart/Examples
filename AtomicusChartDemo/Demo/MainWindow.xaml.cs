using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.AxesData;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.PresentationData.BaseTypes;
using AtomicusChart.WpfControl;
using Colors = AtomicusChart.Interface.Data.Colors;

namespace AtomicusChart.Demo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private ObservableCollection<RenderData> col;

		private readonly MainViewModel viewModel;

		public static readonly DependencyProperty SelectedFeatureProperty =
			DependencyProperty.Register(nameof(SelectedFeature), typeof(FeatureInfo), typeof(MainWindow),
				new FrameworkPropertyMetadata(SelectedFeatureChanged));


		public FeatureInfo SelectedFeature
		{
			get { return (FeatureInfo)GetValue(SelectedFeatureProperty); }
			set { SetValue(SelectedFeatureProperty, value); }
		}

		public MainWindow()
		{
			//TODO: белое мерцание на resize window
			InitializeComponent();
			viewModel = new MainViewModel(Chart);
			DataContext = viewModel;
			Binding binding = new Binding(nameof(MainViewModel.SelectedFeature))
			{
				Mode = BindingMode.TwoWay
			};
			SetBinding(SelectedFeatureProperty, binding);
			StateChanged += OnStateChanged;
		}

		private void OnStateChanged(object sender, EventArgs eventArgs)
		{
			//7 is a magic number.By default Windows fits a maximized window with Margin
			//7 7 7 7 to fill entire screen(WPF.Net 4.5). Larger numbers produce a gap
			//	between maximized window and screen edges; smaller numbers show parts of
			//	the window outside of the current monitor on multi-display computers.

			BorderThickness = WindowState == WindowState.Maximized ? new Thickness(7) : new Thickness(0);
		}

		private static void SelectedFeatureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var feature = (FeatureInfo)e.NewValue;

			var window = (MainWindow)d;

			if (feature == null)
			{
				window.SourceCodeGrid.Visibility = Visibility.Collapsed;
				return;
			}
			//((FeatureDemo)Activator.CreateInstance(feature.FeatureType)).Do(new DemoChartWrapper(window.Chart));

		}


		public class DemoChartWrapper : IDemoChartControl
		{
			public object ChartInstance => chart;

			public ViewResetOptions ViewResetOptions => chart.ViewResetOptions;

			public Color4 Background
			{
				get
				{
					var brush = chart.Background as SolidColorBrush;
					return new Color4(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);
				}
				set => chart.Background = new SolidColorBrush { Color = Color.FromArgb(value.Alpha, value.Red, value.Green, value.Blue) };
			}

			public ContextView ContextView => chart.ContextView;

			public Axes Axes => chart.Axes;

			public bool IsOitEnabled
			{
				get => chart.IsOitEnabled;
				set => chart.IsOitEnabled = value;
			}

			public Multisampling Multisampling
			{
				get => chart.Multisampling;
				set => chart.Multisampling = value;
			}

			public float ScalingCoefficient
			{
				get => chart.ScalingCoefficient;
				set => chart.ScalingCoefficient = value;
			}

			public IActionController ActionController => chart.ActionController;

			public BitmapSource LitSphereBitmap
			{
				get => chart.LitSphereBitmap;
				set => chart.LitSphereBitmap = value;
			}

			public object DataSource
			{
				get => chart.DataSource;
				set => chart.DataSource = value;
			}

			private readonly ChartControl chart;
			public DemoChartWrapper(ChartControl cc)
			{
				chart = cc;
			}


			public void Reset()
			{
				chart.Multisampling = Multisampling.High8X;
				chart.ScalingCoefficient = 1.0f;
				chart.IsOitEnabled = false;

				chart.Axes.IsAxes3DVisible = false;
				chart.ContextView.Camera3D.AspectRatio = null;
				chart.Axes.X.DataScale = DataScale.Linear;
				chart.Axes.Y.DataScale = DataScale.Linear;
				chart.Axes.Z.DataScale = DataScale.Linear;
				chart.Axes.X.AxisThickness = 2f;
				chart.Axes.Y.AxisThickness = 2f;
				chart.Axes.Z.AxisThickness = 2f;
				chart.Axes.R.AxisThickness = 2f;
				chart.Axes.Phi.AxisThickness = 2f;
				chart.Axes.Theta.AxisThickness = 2f;
				chart.Axes.X.AxisColor = Colors.Black;
				chart.Axes.Y.AxisColor = Colors.Black;
				chart.Axes.Z.AxisColor = Colors.Black;
				chart.Axes.R.AxisColor = Colors.Black;
				chart.Axes.Phi.AxisColor = Colors.Black;
				chart.Axes.Theta.AxisColor = Colors.Black;
				chart.Axes.CoordinateSystem = CoordinateSystem.Cartesian;

				chart.ViewResetOptions.ResetOnDataChanged = false;
				chart.ViewResetOptions.ResetOnCollectionChanged = false;
				chart.ViewResetOptions.ResetOnDataSourceChanged = true;
				ContextView.Mode2D = false;
				ContextView.SetDefaultView();
			}

			public void Redraw()
			{

			}
		}

		private void Maximize_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}

		private void Close_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			viewModel.StopAll();
			Close();
		}

		private void Minimize_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void Run_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SelectedFeature = (FeatureInfo)e.Parameter;
		}

		private void DimensionalMod_OnClicked(object sender, RoutedEventArgs e)
		{
		    Chart.ContextView.Mode2D = !Chart.ContextView.Mode2D;
		}

		private void SourceCodeVision_OnClicked(object sender, RoutedEventArgs e)
		{
			SourceCodeGrid.Visibility = SourceCodeGrid.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
		}

		private void CloseDescriptionButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (CloseDescriptionPanel.Visibility == Visibility.Visible)
			{
				CloseDescriptionPanel.Visibility = Visibility.Collapsed;
			}
			else
			{
				CloseDescriptionPanel.Visibility = Visibility.Visible;
			}

		}

	}

	class ObjectToVisibilityExtension : MarkupExtension, IValueConverter
	{
		public bool Invert { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new ObjectToVisibilityExtension { Invert = Invert };
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool isVisible = value != null;
			if (Invert)
			{
				isVisible = !isVisible;
			}
			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}

}
