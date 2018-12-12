using System;

namespace AtomicusChart.Demo.Features
{
	public static class DemoDataHelper
	{
		public static T[] GetStripData<T>(float start, float stop, int count, Func<float, T> function)
			where T : struct 
		{
			var result = new T[count];
			float step = (stop - start) / count;
			for (int i = 0; i < count; i++)
			{
				result[i] = function(start + i * step);
			}
			return result;
		}
	}
}