using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.WpfControl;

namespace AtomicusChart.Demo
{
	public class MainViewModel : IFeatureViewer, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly FeatureTracker featureTracker;

		private Group selectedGroup;
		private Category selectedCategory;
		private FeatureInfo selectedFeature;
		private string featureCode;
		private CompilerErrorCollection errors;

		public ObservableCollection<Group> Groups { get; } = new ObservableCollection<Group>();

		public FeatureInfo SelectedFeature
		{
			get => selectedFeature;
			set
			{
				if (selectedFeature == value) return;

				selectedFeature = value;
				RaisePropertyChanged();

				if (selectedFeature != null)
				{
					SelectedCategory = FindCategoryFromFeature(selectedFeature);
				}

				if (selectedFeature != null)
				{
					featureTracker.CreateAndRun(selectedFeature);
				}
				else
				{
					featureTracker.StopAll();
				}
			}
		}

		private Category FindCategoryFromFeature(FeatureInfo fi)
		{
			foreach (var group in Groups)
			{
				var category = group.Categories.FirstOrDefault(o => o.Features.Contains(fi));
				if (category != null)
					return category;
			}
			return null;
		}

		public Group SelectedGroup
		{
			get { return selectedGroup ?? (selectedGroup = Groups.First()); }
			set
			{
				if (selectedGroup == value) return;
				selectedGroup = value;
				if ((selectedCategory != null) && (IsInclude(selectedCategory, selectedGroup.Categories)))
				{
					SelectedCategory = null;
				}
				RaisePropertyChanged();
			}
		}
		public Category SelectedCategory
		{
			get { return selectedCategory; }
			set
			{
				if (selectedCategory == value) return;
				selectedCategory = value;
				RaisePropertyChanged();
			}
		}
		public string FeatureCode
		{
			get => featureCode;
			set
			{
				if (featureCode == value)
					return;
				featureCode = value;
				RaisePropertyChanged();
			}
		}
		public CompilerErrorCollection Errors
		{
			get => errors;
			set
			{
				if (errors == value)
					return;
				errors = value;
				RaisePropertyChanged();
			}
		}

		public ICommand BackCommand { get; }
		public ICommand HomeCommand { get; }
		public ICommand CompileAndRun { get; set; }
		public ICommand SetCodeToDefault { get; }

		public MainViewModel(ChartControl chart)
		{
			AddFeatureGroup("Basic", 
				new FeaturesFolder(typeof(FeatureDemo).Assembly, "AtomicusChart.Demo.Features.Basic", Demo.Features.Properties.Resources.ResourceManager)
				//new FeaturesFolder(typeof(FeatureDemo).Assembly, "AtomicusChart.Demo.Features.Primitives", Demo.Features.Properties.Resources.ResourceManager)
				);
			AddFeatureGroup("Value Data", 
				new FeaturesFolder(typeof(FeatureDemo).Assembly, "AtomicusChart.Demo.Features.ValueData", Demo.Features.Properties.Resources.ResourceManager)
				//new FeaturesFolder(typeof(FeatureDemo).Assembly, "AtomicusChart.Demo.Features.Contours", Demo.Features.Properties.Resources.ResourceManager)
				);
			AddFeatureGroup("Volumetric", new FeaturesFolder(typeof(FeatureDemo).Assembly, "AtomicusChart.Demo.Features.VolumeData", Demo.Features.Properties.Resources.ResourceManager));

			SelectedGroup = Groups.First();
			BackCommand = new SimpleCommand(() => SelectedFeature = null);
			HomeCommand = new SimpleCommand(() => SelectedCategory = null);

			CompileAndRun = new SimpleCommand(CompileAndRunHandler);
			SetCodeToDefault = new SimpleCommand(SetCodeToDefaultHandler);
			featureTracker = new FeatureTracker(new MainWindow.DemoChartWrapper(chart), this);
		}

		private void AddFeatureGroup(string groupName, params FeaturesFolder[] featureFolder)
		{
			IReadOnlyList<FeatureGroup> loadedFeatures = FeatureLoader.LoadAllFeatures(featureFolder);

			List<Category> realCategories = loadedFeatures.Select(o => new Category
			{
				Header = new Header { Name = o.Caption },
				Features = new FeaturesCollection(o.Items)
			}).ToList();

			realCategories.Sort((o1, o2) => Math.Sign(o2.Features.Count - o1.Features.Count));

			var realGroup = new Group
			{
				Header = new Header { Name = groupName },
				Categories = new CategoriesCollection(realCategories)
			};

			Groups.Add(realGroup);
		}



		public void SetNewFeatureAndShowChart(string featureName, string code, CompilerErrorCollection compilerErrorCollection)
		{
			FeatureCode = code;
			Errors = compilerErrorCollection;
		}

		public void StopAll() => featureTracker.StopAll();

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private bool IsInclude(Category category, ObservableCollection<Category> collection)
		{
			return (collection.IndexOf(category) != 0);
		}

		private void SetCodeToDefaultHandler() => featureTracker.SetCodeToDeafult();

		private void CompileAndRunHandler() => featureTracker.TryCreateAndRun(featureCode);

	}


	public class SimpleCommand : ICommand
	{
		private readonly Action action;

		public SimpleCommand(Action action)
		{
			this.action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter)
		{
			action?.Invoke();
		}

		public event EventHandler CanExecuteChanged;
	}
}