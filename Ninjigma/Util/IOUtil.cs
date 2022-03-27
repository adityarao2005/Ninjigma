using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Ninjigma.Util
{

	// Utility class for File I/O operations
	public static class IOUtil
	{
		// Copy a file to the local folder
		public static async Task<StorageFile> CopyFileToLocalAsync(StorageFile file)
		{
			// Copy the file to the folder
			return await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.GenerateUniqueName);
		}

		// Copy the URL content to File
		public static async Task<StorageFile> CopyURLContentToFile(Uri uri, Predicate<string> criteriaOfMime)
		{
			// Create an HTTP client
			using (HttpClient client = new HttpClient())
			{
				// Get http response
				var response = await client.GetAsync(uri);

				// Get content type
				string contentType = response.Content.Headers.ContentType.MediaType;

				// Check if the mime matches
				if (!criteriaOfMime.Invoke(contentType)) { return null; }

				// Create a file in the local folder
				StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(System.IO.Path.GetFileName(uri.LocalPath), CreationCollisionOption.GenerateUniqueName);

				// Create a new file stream
				using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
				{
					// write to the stream
					await response.Content.WriteToStreamAsync(stream);
				}

				// returnt the file
				return file;

			}
		}

		// Create a file if it doesnt exist
		public static async Task<StorageFile> CreateIfNotExists(string name)
		{
			// Return the file
			return await ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
		}
	}
}
