using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AtomicusChart.Demo
{
    class TabItemBackgroundConverter : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TabItem tabItem = (TabItem)value ;
			int index= ItemsControl.ItemsControlFromItemContainer(tabItem)
						.ItemContainerGenerator.IndexFromContainer(tabItem);
			return index % 2 == 0 ? Brushes.DarkGray : Brushes.LightGray;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new TabItemBackgroundConverter();
		}
	}
}
