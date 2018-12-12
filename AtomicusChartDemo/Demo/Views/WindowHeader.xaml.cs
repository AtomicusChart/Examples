using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AtomicusChart.Demo.Views
{
    internal partial class WindowHeader 
    {
        public WindowHeader()
        {
            InitializeComponent();
        }
    }

	class ImageButton : Button
	{
		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(ImageButton),
				new FrameworkPropertyMetadata());

		public ImageSource Icon
		{
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		static ImageButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
		}
	}
}
