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
	// Utitilty class for manipulating Images
	public static class ImageUtil
	{

		// Get image from file
		public static async Task<SoftwareBitmap> FromFile(StorageFile file)
		{
			// Decode the bitmap from file stream
			BitmapDecoder decoder = await BitmapDecoder.CreateAsync(await file.OpenAsync(FileAccessMode.Read));

			return await decoder.GetSoftwareBitmapAsync();

		}

		// Copy one bitmap to the other
		public static SoftwareBitmap Copy(SoftwareBitmap bitmap)
		{
			// create the other
			SoftwareBitmap newB = new SoftwareBitmap(bitmap.BitmapPixelFormat, bitmap.PixelWidth, bitmap.PixelHeight);
			// Copy
			bitmap.CopyTo(newB);
			// Return
			return newB;
		}

		// Crop a copy of the bitmap
		public static async Task<SoftwareBitmap> Crop(SoftwareBitmap bitmap, BitmapBounds rectangle)
		{
			// Create a new inmemory stream to store the copy
			using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
			{
				// Create the encoder
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				// encode the bitmap
				encoder.SetSoftwareBitmap(bitmap);
				// flush the stream
				await encoder.FlushAsync();

				// Decode from stream
				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

				// Crop the decoded copy
				SoftwareBitmap newB = await decoder.GetSoftwareBitmapAsync(bitmap.BitmapPixelFormat, bitmap.BitmapAlphaMode, new BitmapTransform { Bounds = rectangle }, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
				// return copy
				return newB;
			}
		}

		// Split the image into 2D array/grid of specified rows and columns
		public static async Task<SoftwareBitmap[,]> Split(SoftwareBitmap bitmap, int rows, int columns)
		{
			// Initialize the widths of each image
			int columnWidth = bitmap.PixelWidth / columns;
			int rowHeight = bitmap.PixelHeight / rows;

			// Create a software bitmap 2D array
			SoftwareBitmap[,] bitmaps = new SoftwareBitmap[rows, columns];

			// Iterate, crop, and save
			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					// Crop the bitmap at column to pixels and row to pixels
					// with the size being the width of the image
					SoftwareBitmap bitmapt = await Crop(bitmap, new BitmapBounds { X = (uint)(column * columnWidth), Y = (uint)(row * rowHeight), Width = (uint)columnWidth, Height = (uint)rowHeight });
					bitmaps[row, column] = bitmapt;
				}
			}

			return bitmaps;
		}
	}
}
