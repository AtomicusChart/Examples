using System;
using System.Collections.Generic;
using AtomicusChart.Interface;
using AtomicusChart.Interface.Data;
using AtomicusChart.Interface.Processing;

namespace AtomicusChart.Demo.Features
{
	public static class DemoHelper
	{
		private static readonly Random random = new Random();

		public static Color4 RandomizeColor() => new Color4((byte) random.Next(0, 255), (byte) random.Next(0, 255),
			(byte) random.Next(0, 255), 255);

		public static Contour[] GenerateContours(int count, float thickness, OneAxisBounds valueRange)
		{
			float step = (valueRange.Max - valueRange.Min) / (count - 1);
			Contour[] contours = new Contour[count];
			for (int i = 0; i < count; i++)
			{
				contours[i] = new Contour(valueRange.Min + step * (i + 1), RandomizeColor(), thickness);
			}
			return contours;
		}

		public static VertexMask[] RandomizeMasks(int count)
		{
			VertexMask[] result = new VertexMask[count];
			for (int i = 0; i < count; i++)
			{
				int value = random.Next(0, 40);
				if (value < 10)
					result[i] = VertexMask.Default;
				else if (value < 20)
					result[i] = VertexMask.IsNan;
				else if (value < 30)
					result[i] = VertexMask.NegativeInfinity;
				else if (value < 40)
					result[i] = VertexMask.PositiveInfinity;
			}
			return result;
		}

		public static VertexMask[] RandomizeNanMasks(int count)
		{
			VertexMask[] result = new VertexMask[count];
			for (int i = 0; i < count; i++)
			{
				int value = random.Next(0, 20);
				if (value < 10)
					result[i] = VertexMask.Default;
				else if (value < 20)
					result[i] = VertexMask.IsNan;
			}
			return result;
		}

		private static Vector3F[] GenerateParametricSurface(int width, int height)
		{
			return GridHelper.GetStructuredParametricGridPositions((u, v) =>
				{
					double aa = 0.4f, r = 1 - aa * aa, w = Math.Sqrt(r);
					double denominator = (aa * (Math.Pow(w * Math.Cosh(aa * u), 2) + aa * Math.Pow(Math.Sinh(w * v), 2)));
					double x = -u + (2 * r * Math.Cosh(aa * u) * Math.Sinh(aa * u) / denominator);
					double y = 2 * w * Math.Cosh(aa * u) * (-(w * Math.Cos(v) * Math.Cos(w * v)) - (Math.Sin(v) * Math.Sin(w * v))) /
							   denominator;
					double z = 2 * w * Math.Cosh(aa * u) * (-(w * Math.Sin(v) * Math.Cos(w * v)) - (Math.Cos(v) * Math.Sin(w * v))) /
							   denominator;
					return new Vector3F((float)x, (float)y, (float)z);
				}, new Vector2F(-13.2f, -37.4f),
				new Vector2F(13.2f, 37.4f), width, height);
		}

		/// <summary>
		/// Plot[ sin(x) * sin(x*x + y * y), {x, -3, 3}, {y, -3, 3}]
		/// </summary>
		/// <param name="resolution"></param>
		/// <returns>Structured grid.</returns>
		public static Vector3F[] GenerateSinPoints(int resolution)
		{
			return GenerateSquareStructuredGrid((x, y) => (float)(Math.Sin(x) * Math.Sin(x * x + y * y)), -3f, 3f, resolution);
		}

		public static Vector3F[] GenerateSphericalSinPoints(int resolution)
		{
			return GenerateSquareStructuredGrid((x, y) => (float)(Math.Sin(x) * Math.Sin(x * x + y * y)), -3f, 3f, resolution);
		}

		private static Vector3F[] GenerateSquareStructuredGrid(Func<float, float, float> func, float start, float stop, int resolution)
		{
			return GridHelper.GetStructuredParametricGridPositions((u, v) => new Vector3F(u, v, func(u, v)),
				new Vector2F(start, start), new Vector2F(stop, stop), resolution, resolution);
		}

		public static Vector3F[] GenerateIrregularSinPoints(int size) =>
			SkipPoints(GenerateSinPoints(size));

		public static float GetRandomValue(float min, float max) =>
			min + (max - min) * (float) random.NextDouble();

		public static Vector3F GetRandomColor() =>
			new Vector3F(GetRandomValue(0, 1), GetRandomValue(0, 1), GetRandomValue(0, 1));

		public static Vector3F[] GenerateRandomPoints(int count, Vector3F min, Vector3F max)
		{
			var result = new Vector3F[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new Vector3F(GetRandomValue(min.X, max.X), GetRandomValue(min.Y, max.Y), GetRandomValue(min.Z, max.Z));
			}
			return result;
		}

		public static Vector3F[] GenerateIrregularPoints(int width, int height, float min, float max)
		{
			float step = 0.1f;
			int totalCount = width * height;
			Vector3F[] result = new Vector3F[totalCount];
			for (int i = 0; i < totalCount; i++)
			{
				int x = i % width, y = i / width;
				result[i] = new Vector3F(step * x, step * y, GetRandomValue(min, max));
			}
			return result;
		}

		public static Vector3F[] SkipPoints(Vector3F[] points)
		{
			List<Vector3F> result = new List<Vector3F>();
			for (int i = 0; i < points.Length; i++)
			{
				if (random.Next(0, 15) <= 10)
					continue;
				result.Add(points[i]);
			}
			return result.ToArray();
		}

		public static float[] ExtractZValues(Vector3F[] positions)
		{
			float[] result = new float[positions.Length];
			for (int i = 0; i < positions.Length; i++)
			{
				result[i] = positions[i].Z;
			}
			return result;
		}

		public static float[] ExtractZValues(Vector3F[] positions, out OneAxisBounds bounds)
		{
			float[] result = new float[positions.Length];
			float min = float.MaxValue, max = float.MinValue;
			for (int i = 0; i < positions.Length; i++)
			{
				result[i] = positions[i].Z;
				min = Math.Min(min, result[i]);
				max = Math.Max(max, result[i]);
			}
			bounds = new OneAxisBounds(min, max);
			return result;
		}
	}
}
