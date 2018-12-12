using System;

namespace AtomicusChart.Demo.Features.Infrastructure
{
	public class FeatureAttribute : Attribute
	{
		public const bool DefaultResetViewBeforeRun = true;
		public const bool DefaultRunOnSuspend = true;

		public string GroupName { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool ResetViewBeforeRun { get; set; } = DefaultResetViewBeforeRun;

		public bool RunOnSuspend { get; set; } = DefaultRunOnSuspend;
	}
}
