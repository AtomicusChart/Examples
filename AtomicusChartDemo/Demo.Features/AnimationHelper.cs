using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AtomicusChart.Demo.Features
{
	class AnimationHelper
	{
		private const float DefaultArgumentStep = 0.00025f;
		private const int DefaultInterval = 16;
		private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);

		private Thread thread;
		private Dispatcher dispatcher;
		private float argumentValue;

		public void Start(Func<float, float> asyncAction, Action<float> updateAction, float initialArgument = 0.0f,
			float argumentStep = DefaultArgumentStep, int interval = DefaultInterval)
		{
			argumentValue = initialArgument;
			dispatcher = Dispatcher.CurrentDispatcher;
			stopEvent.Reset();
			thread = new Thread(() =>
			{
				while (!stopEvent.WaitOne(interval))
				{
					argumentValue += argumentStep;
					argumentValue = asyncAction(argumentValue);
					try
					{
						dispatcher.Invoke(() => updateAction(argumentValue));
					}
					catch (TaskCanceledException)
					{
						//Do nothing, application is closing
						return;
					}
				}
			});
			thread.Start();
		}


		public void Stop()
		{
			stopEvent.Set();
			if (thread != null)
				if (!thread.Join(500))
					thread.Abort();
			thread = null;
		}
	}
}
