using System;
using AtomicusChart.Interface.Data;
using AtomicusChart.Volumes.DataReaders;

namespace AtomicusChart.Demo.Features.VolumeData
{
	internal static class VolumeHelper
	{
		private const int DefaultSize = 100;

		public class RandomFloatIntensityReader3D : IntensityImage3DReader
		{
			private readonly float[] data;
			private readonly int stride;
			private readonly int depthSlice;
			private readonly float randomFactor;

			public RandomFloatIntensityReader3D(int width, int height, int depth, OneAxisBounds valueRange, float randomFactor) :
				base(new IntensityImage3DDescription(width, height, depth, Intensity3DFormat.Float))
			{
				data = new float[width * height * depth];
				ValueRange = valueRange;
				stride = sizeof(float) * width;
				depthSlice = stride * depth;
				this.randomFactor = randomFactor;
			}

			public override void InitializeResource(IResourceWriter3D resourceWriter) => UpdateResource(data, resourceWriter);

			public void RandomizeValues(float argument)
			{
				float[] RandomFloatData()
				{
					var result = new float[Description.Width * Description.Height * Description.Depth];
					for (int i = 0; i < data.Length; i++)
					{
						int z = i / depthSlice, planeIndex = i % depthSlice;
						int x = planeIndex % Description.Width, y = planeIndex / Description.Height;
						result[i] = (float)((ValueRange.Min + Math.Cos(argument * x) * Math.Sin(y * argument) * Math.Sin(Math.Pow(z, 2) * argument)) * randomFactor);
					}
					return result;
				}
				OnDataChanged(new DelegateWriter3D(writer => { UpdateResource(RandomFloatData(), writer); }));
			}

			private void UpdateResource(float[] newData, IResourceWriter3D resourceWriter) =>
				resourceWriter.UpdateResource(newData, stride, depthSlice);

		}

		public static FloatIntensityImage3DReader GetDefaultIntensityImage3DReader()
		{
			return new FloatIntensityImage3DReader(RandomizeData(DefaultSize, DefaultSize, DefaultSize),
				DefaultSize, DefaultSize, new OneAxisBounds(0, 1));
		}

		public static IntensityImage3DReader GetRealIntensityImage3DReader()
		{
			var data = Properties.Resources.skull;
			var size = (int) Math.Round(Math.Pow(data.Length, 1d / 3d));
			float min = float.MaxValue, max = float.MinValue;
			foreach (var val in data)
			{
				min = Math.Min(min, val);
				max = Math.Max(max, val);
			}
			return new ByteIntensityImage3DReader(data, size, size, new OneAxisBounds(min, max));
		}

		public static float[] RandomizeData(int width, int height, int depth)
		{
			float[] result = new float[width * height * depth];
			int rX = width / 2, rY = height / 2, rZ = depth / 2;
			int index = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int z = 0; z < depth; z++)
					{
						int realX = x - rX, realY = y - rY, realZ = z - rZ;
						if (((realX * realX) / (rX * rX) + (realY * realY) / (rY * rY) + (realZ * realZ) / (rZ * rZ)) <= 1.0f)
						{
							result[index] = 1;
						}
						else
						{
							result[index] = 0.5f;
						}

						index++;
					}
				}
			}
			return result;
		}

		public static float[] RandomizeData2(int width, int height, int depth)
		{
			float[] result = new float[width * height * depth];
			int index = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int z = 0; z < depth; z++)
					{
						result[index] = 1;
						index++;
					}
				}
			}
			return result;
		}
	}
}
