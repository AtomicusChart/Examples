using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using AtomicusChart.Demo.Features.Infrastructure;

namespace AtomicusChart.Demo
{
	public class Header
	{
		public string Name { get; set; }
		public ImageSource Icon { get; set; }
	}

	public class Group
	{
		public Header Header { get; set; }
		public CategoriesCollection Categories { get; set; } = new CategoriesCollection();
	}

	public class CategoriesCollection : ObservableCollection<Category>
	{
		public CategoriesCollection() { }
		public CategoriesCollection(IEnumerable<Category> categories) : base(categories) { }

	}
	public class Category
	{
		public Header Header { get; set; }
		public FeaturesCollection Features { get; set; } = new FeaturesCollection();
		
	}

	public class FeaturesCollection : ObservableCollection<FeatureInfo>
	{
		public FeaturesCollection() { }
		public FeaturesCollection(IEnumerable<FeatureInfo> features) : base(features) { }
	}
}
