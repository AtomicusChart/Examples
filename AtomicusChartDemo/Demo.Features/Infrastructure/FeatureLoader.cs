using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Resources;

namespace AtomicusChart.Demo.Features.Infrastructure
{

	public class FeatureCategory
	{
		public string Caption { get; set; }

		public IReadOnlyList<FeatureGroup> Items { get; set; }
		
		public int FeatureCount { get; set; }

		public FeatureCategory(string caption, IReadOnlyList<FeatureGroup> items, int featureCount)
		{
			Items = items;
			Caption = caption;
			FeatureCount = featureCount;
		}
	}

	public class FeatureGroup
	{
		public string Caption { get; set; }

		public IReadOnlyList<FeatureInfo> Items { get; set; }

		public FeatureGroup(string caption, IReadOnlyList<FeatureInfo> items)
		{
			Items = items;
			Caption = caption;
		}
	}

	public struct FeaturesFolder
	{
		public readonly Assembly Assembly;
		public readonly string FolderNamespace;
		public readonly ResourceManager ResourceManager;

		public FeaturesFolder(Assembly assembly, string folderNamespace, ResourceManager resourceManager)
		{
			Assembly = assembly;
			FolderNamespace = folderNamespace;
			ResourceManager = resourceManager;
		}

	}

	public interface ICodeLoader
	{
		string GetCode(FeatureInfo featureInfo);
	}

	class ResourcesCodeLoader : ICodeLoader
	{
		private readonly ResourceManager resourceManager;

		public ResourcesCodeLoader(ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
		}

		public string GetCode(FeatureInfo featureInfo) => resourceManager?.GetString(featureInfo.FeatureType.Name);
	}

	class CompilationCodeLoader : ICodeLoader
	{
		private readonly string code;

		public CompilationCodeLoader(string code)
		{
			this.code = code;
		}

		public string GetCode(FeatureInfo featureInfo) => code;
	}

	public static class FeatureLoader
	{
		private static readonly Type FeatureDemoType = typeof(FeatureDemo);

		public static IReadOnlyList<FeatureGroup> LoadAllFeatures(params FeaturesFolder[] featuresFolders)
		{
			var groupsToFeatures = new Dictionary<string, List<FeatureInfo>>();
			foreach (FeaturesFolder featuresFolder in featuresFolders)
			{
				var codeLoader = new ResourcesCodeLoader(featuresFolder.ResourceManager);
				Type[] assembleyTypes = featuresFolder.Assembly.GetTypes();
				foreach (Type typeForCheck in assembleyTypes)
				{
					FeatureInfo featureInfo;
					string errorMessage;
					if (TryGetFeatureInfo(typeForCheck, codeLoader, out featureInfo, out errorMessage) &&
						typeForCheck.Namespace.StartsWith(featuresFolder.FolderNamespace))
					{
						List<FeatureInfo> featureGroup;
						if (!groupsToFeatures.TryGetValue(featureInfo.GroupName, out featureGroup))
						{
							featureGroup = new List<FeatureInfo>();
							groupsToFeatures.Add(featureInfo.GroupName, featureGroup);
						}
						featureGroup.Add(featureInfo);
					}
				}
			}

			var groups = new List<FeatureGroup>();
			foreach (KeyValuePair<string, List<FeatureInfo>> featureGroups in groupsToFeatures)
			{
				groups.Add(new FeatureGroup(
					featureGroups.Key,
					new ReadOnlyCollection<FeatureInfo>(featureGroups.Value)));
			}
			groups.Sort((g1, g2) => string.Compare(g1.Caption, g2.Caption, StringComparison.Ordinal));

			return groups;
		}

		internal static bool TryGetFeatureInfo(
			Type featureType,
			ICodeLoader codeLoader,
			out FeatureInfo featureInfo,
			out string errorMessage)
		{
			if (!featureType.IsSubclassOf(FeatureDemoType))
			{
				errorMessage = $"{nameof(featureType)} should be derived from {nameof(FeatureDemo)}.";
				featureInfo = null;
				return false;
			}
			if (featureType.IsAbstract)
			{
				errorMessage = "Type is abstract.";
				featureInfo = null;
				return false;
			}

			object[] featureAttributes = featureType.GetCustomAttributes(typeof(FeatureAttribute), false);

			string groupName;
			string featureName;
			bool shouldResetViewBeforeRun;
			bool runOnSuspend;
			string featureDescription = null;
			bool developerOnly = true;

			if (featureAttributes.Length > 0)
			{
				var featureAttribute = (FeatureAttribute)featureAttributes[0];
				groupName = string.IsNullOrEmpty(featureAttribute.GroupName)
					? GetLastFolderNameFromNamespace(featureType)
					: featureAttribute.GroupName;
				featureName = string.IsNullOrEmpty(featureAttribute.Name)
					? featureType.Name
					: featureAttribute.Name;
				shouldResetViewBeforeRun = featureAttribute.ResetViewBeforeRun;
				featureDescription = featureAttribute.Description;
				runOnSuspend = featureAttribute.RunOnSuspend;
			}
			else
			{
				groupName = GetLastFolderNameFromNamespace(featureType);
				featureName = featureType.Name;
				shouldResetViewBeforeRun = FeatureAttribute.DefaultResetViewBeforeRun;
				runOnSuspend = FeatureAttribute.DefaultRunOnSuspend;
			}

			errorMessage = null;
			featureInfo = new FeatureInfo(
				featureName,
				shouldResetViewBeforeRun,
				featureType,
				groupName,
				featureDescription,
				developerOnly,
				runOnSuspend,
				codeLoader);
			return true;
		}

		/// <summary>
		/// From .....Name1.Name2.ClassName get Name2.
		/// </summary>
		/// <param name="type">Type of feature.</param>
		/// <returns>Group name.</returns>
		private static string GetLastFolderNameFromNamespace(Type type)
		{
			string name = type.FullName;
			string[] names = name.Split('.');
			if (names.Length == 1)
				return names[0];
			else
				return names[names.Length - 2];
		}
	}
}
