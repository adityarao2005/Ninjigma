using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Ninjigma.Util
{
	public static class ImageUtil
	{

		public static async Task<SoftwareBitmap> FromFile(StorageFile file)
		{
			BitmapDecoder decoder = await BitmapDecoder.CreateAsync(await file.OpenAsync(FileAccessMode.Read));

			return await decoder.GetSoftwareBitmapAsync();

		}

		public static SoftwareBitmap Copy(SoftwareBitmap bitmap)
		{
			SoftwareBitmap newB = new SoftwareBitmap(bitmap.BitmapPixelFormat, bitmap.PixelWidth, bitmap.PixelHeight);

			bitmap.CopyTo(newB);

			return newB;
		}

		public static async Task<SoftwareBitmap> Crop(SoftwareBitmap bitmap, BitmapBounds rectangle)
		{
			using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

				encoder.SetSoftwareBitmap(bitmap);

				await encoder.FlushAsync();

				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

				SoftwareBitmap newB = await decoder.GetSoftwareBitmapAsync(bitmap.BitmapPixelFormat, bitmap.BitmapAlphaMode, new BitmapTransform { Bounds = rectangle }, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);

				return newB;
			}
		}

		public static async Task<SoftwareBitmap[,]> Split(SoftwareBitmap bitmap, int rows, int columns)
		{
			int columnWidth = bitmap.PixelWidth / columns;
			int rowHeight = bitmap.PixelHeight / rows;

			SoftwareBitmap[,] bitmaps = new SoftwareBitmap[rows, columns];

			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					SoftwareBitmap bitmapt = await Crop(bitmap, new BitmapBounds { X = (uint)(column * columnWidth), Y = (uint)(row * rowHeight), Width = (uint)columnWidth, Height = (uint)rowHeight });
					bitmaps[row, column] = bitmapt;
				}
			}

			return bitmaps;
		}
	}
}
