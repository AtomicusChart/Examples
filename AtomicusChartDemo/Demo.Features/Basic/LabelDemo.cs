using System;
using System.Collections.Generic;
using AtomicusChart.Demo.Features.Infrastructure;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Interaction;
using AtomicusChart.Interface.Interaction.RenderDataInteraction;
using AtomicusChart.Interface.PresentationData;
using AtomicusChart.Interface.PresentationData.BaseTypes;

namespace AtomicusChart.Demo.Features.Basic
{
	[Feature(Name = "Labels", GroupName = "Primitives", Description = "Demo shows example of using label primitive. Labels are always faced to user and have constant size in pixels and not depend on view zoom level. You can change properties in legend.")]
	public class LabelDemo : FeatureDemo
	{
		public override void Do(IDemoChartControl chartControl)
		{
			const int pointsCount = 1_000;
			const float amplitude = 0.6f;

			// Calculate points on line.
			var points = new Vector3F[pointsCount];
			for (var i = 0; i < pointsCount; i++)
			{
				float scaledCoord = i / (float)pointsCount;
				points[i] = new Vector3F(
					scaledCoord,
					amplitude * (float)(Math.Cos(5 * 2 * Math.PI * scaledCoord) + 0.5f),
					amplitude * (float)(Math.Sin(5 * Math.PI * scaledCoord) + 0.5f));
			}

			// Create line.
			var line = new Line
			{
				Points = points,
				Strip = true,
				Color = Colors.Blue,
				Thickness = 3,
				Name = "Main line"
			};

			// Add labels.
			const int labelStep = 100;
			var renderData = new List<RenderData> {line};
			int labelIndex = 0;
			for (var i = 0; i < pointsCount; i += labelStep)
			{
				renderData.Add(new Label
				{
					Text = (i / labelStep).ToString(),
					FontFamily = "Arial",
					FontSize = 18,
					Transform = Matrix4F.Translation(points[i]),
					Background = Colors.White,
					Name = $"Label {labelIndex++}"
				});
			}

			// Show axes.
			chartControl.Axes.IsAxes3DVisible = true;

			// Show result.
			chartControl.DataSource = renderData.ToArray();
		}
		
	}
}
