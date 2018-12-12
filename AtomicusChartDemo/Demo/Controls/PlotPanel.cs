using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AtomicusChart.Demo.Controls
{
	public class PlotPanel : Panel
	{
		private Dictionary<UIElement, Rect> pos;

		// Override the default Measure method of Panel
		protected override Size MeasureOverride(Size availableSize)
		{
			pos = new Dictionary<UIElement, Rect>();
			if (InternalChildren.Count == 0)
			{
				return new Size(0, 0);
			}
			var maxWidth = 0.0;

			foreach (UIElement child in InternalChildren)
			{
				child.Measure(availableSize);
				maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
			}


			bool flag = true;
			int index = 0;

			var columnCount = (int)Math.Max(availableSize.Width / maxWidth, 1);
			double[] rowsY = new double[columnCount];

			foreach (UIElement child in InternalChildren)
			{
				double x = index * maxWidth;
				double y = rowsY[index];

				pos.Add(child, new Rect(new Point(x, y), child.DesiredSize));

				rowsY[index] += child.DesiredSize.Height;

				if (flag)
				{
					if (index == columnCount - 1)
					{
						flag = false;
					}
					else
					{
						index++;
					}
				}
				else
				{
					if (index == 0)
					{
						flag = true;
					}
					else
					{
						index--;
					}
				}
			}

			double finalHeight = rowsY.Max();
			double finalWidth = columnCount * maxWidth;
			return new Size(finalWidth, finalHeight);
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (KeyValuePair<UIElement, Rect> keyValuePair in pos)
			{
				keyValuePair.Key.Arrange(keyValuePair.Value);
			}
			return finalSize;
		}
	}

}
