using System;
using System.CodeDom.Compiler;

namespace AtomicusChart.Demo.Features.Infrastructure
{
	public class FeatureTracker
	{
		private static readonly FeatureCompiler FeatureCompiler = new FeatureCompiler();
		private readonly IDemoChartControl chartControl;
		private readonly IFeatureViewer featureViewer;
		private static readonly CompilerErrorCollection EmptyErrorCollection = new CompilerErrorCollection();
		private FeatureDemo executingFeatureDemo;
		private FeatureInfo lastFeatureFromAssebly;

		public FeatureTracker(IDemoChartControl chartControl, IFeatureViewer featureViewer)
		{
			this.chartControl = chartControl;
			this.featureViewer = featureViewer;
		}

		public bool TryCreateAndRun(string featureCode)
		{
			FeatureDemo featureDemo;
			FeatureInfo featureInfo;
			CompilerErrorCollection errorCollection = CreateFeature(featureCode, out featureDemo, out featureInfo);

			if (errorCollection.HasErrors)
			{
				featureViewer.SetNewFeatureAndShowChart(
					"Custom feature",
					featureCode,
					errorCollection);
				return false;
			}
			else
			{
				featureViewer.SetNewFeatureAndShowChart(
					featureInfo.FeatureName,
					featureInfo.LoadCode(),
					errorCollection);
				RunFeature(featureDemo, featureInfo);
				return true;
			}
		}

		public void CreateAndRun(FeatureInfo featureInfo)
		{
			FeatureDemo featureDemo = CreateFeature(featureInfo);
			featureViewer.SetNewFeatureAndShowChart(
				featureInfo.FeatureName,
				featureInfo.LoadCode(),
				EmptyErrorCollection);
			RunFeature(featureDemo, featureInfo);
			lastFeatureFromAssebly = featureInfo;
		}

		public void SetCodeToDeafult()
		{
			if (lastFeatureFromAssebly == null)
				featureViewer.SetNewFeatureAndShowChart(
					string.Empty,
					string.Empty,
					EmptyErrorCollection);
			else
				featureViewer.SetNewFeatureAndShowChart(
					lastFeatureFromAssebly.FeatureName,
					lastFeatureFromAssebly.LoadCode(),
					EmptyErrorCollection);

		}

		private void RunFeature(FeatureDemo featureDemo, FeatureInfo featureInfo)
		{
			if (executingFeatureDemo != null)
			{
				executingFeatureDemo.Stop(chartControl);
				executingFeatureDemo = null;
			}

			if (featureInfo.ShouldResetViewBeforeRun)
				chartControl.Reset();
			if (featureInfo.RunOnSuspend)
			{
				PrimarySettings.Setup(chartControl);
				featureDemo.Do(chartControl);
			}
			else
				featureDemo.Do(chartControl);

			executingFeatureDemo = featureDemo;
		}

		public void StopAll()
		{
			executingFeatureDemo?.Stop(chartControl);
			executingFeatureDemo = null;
		}

		private static FeatureDemo CreateFeature(FeatureInfo featureInfo) 
			=> (FeatureDemo) Activator.CreateInstance(featureInfo.FeatureType);

		private static CompilerErrorCollection CreateFeature(
			string code, out FeatureDemo featureDemo, out FeatureInfo featureInfo)
		{ 
			Type featureDemoType;
			CompilerErrorCollection compilerErrorCollection = FeatureCompiler.TryCompile(code, out featureDemoType);
			if (compilerErrorCollection.HasErrors)
			{
				featureInfo = null;
				featureDemo = null;
				return compilerErrorCollection;
			}
			else
			{
				string errorMessage;
				if (FeatureLoader.TryGetFeatureInfo(
					featureDemoType,
					new CompilationCodeLoader(code),
					out featureInfo,
					out errorMessage))
				{
					featureDemo = CreateFeature(featureInfo);
					return EmptyErrorCollection;
				}
				else
				{
					featureDemo = null;
					featureInfo = null;
					return FeatureCompiler.GetCustomCompilerError("!!!", errorMessage);
				}
			}
		}
	}
}
