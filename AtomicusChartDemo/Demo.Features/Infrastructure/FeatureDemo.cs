using System;
using System.CodeDom.Compiler;
using System.Windows.Media.Imaging;
using AtomicusChart.Core.DirectX;
using AtomicusChart.Interface.AxesData;
using AtomicusChart.Interface.CameraView;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;

namespace AtomicusChart.Demo.Features.Infrastructure
{

	public class FeatureInfo
	{
		public string FeatureName { get; }

		public bool ShouldResetViewBeforeRun;

		public readonly Type FeatureType;

		public readonly string GroupName;

		public string FeatureDescription { get; }

		public readonly bool DeveloperOnly;

		public readonly bool RunOnSuspend;

		private readonly ICodeLoader codeLoader;

		public FeatureInfo(
			string featureName,
			bool shouldResetViewBeforeRun,
			Type featureType,
			string groupName,
			string featureDescription,
			bool developerOnly,
			bool runOnSuspend,
			ICodeLoader codeLoader)
		{
			FeatureName = featureName;
			ShouldResetViewBeforeRun = shouldResetViewBeforeRun;

			FeatureType = featureType;
			GroupName = groupName;
			FeatureDescription = featureDescription;
			DeveloperOnly = developerOnly;
			RunOnSuspend = runOnSuspend;
			this.codeLoader = codeLoader;
		}

		public string LoadCode() => codeLoader.GetCode(this);
	}

	public interface IDemoChartControl
	{
		object ChartInstance { get; }

		void Redraw();

		void Reset();

		ContextView ContextView { get; }

		Axes Axes { get; }

		ViewResetOptions ViewResetOptions { get; }

		Color4 Background { get; set; }

		object DataSource { get; set; }

		bool IsOitEnabled { get; set; }

		Multisampling Multisampling { get; set; }

		float ScalingCoefficient { get; set; }

		IActionController ActionController { get; }

		BitmapSource LitSphereBitmap { get; set; }
	}

	public interface IFeatureViewer
	{
		void SetNewFeatureAndShowChart(
			string featureName,
			string code,
			CompilerErrorCollection compilerErrorCollection);
	}

	public abstract class FeatureDemo
	{
		public abstract void Do(IDemoChartControl chartControl);

		public virtual void Stop(IDemoChartControl chartControl) { }
	}
}
