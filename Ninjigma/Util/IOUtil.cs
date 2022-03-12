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
	public static class IOUtil
	{
		public static async Task<StorageFile> CopyFileToLocalAsync(StorageFile file)
		{
			return await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.GenerateUniqueName);
		}

		public static async Task<StorageFile> CopyURLContentToFile(Uri uri, Predicate<string> criteriaOfMime)
		{

			using (HttpClient client = new HttpClient())
			{
				var response = await client.GetAsync(uri);

				string contentType = response.Content.Headers.ContentType.MediaType;

				if (!criteriaOfMime.Invoke(contentType)) { return null; }


				StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(System.IO.Path.GetFileName(uri.LocalPath), CreationCollisionOption.GenerateUniqueName);

				using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
				{

					await response.Content.WriteToStreamAsync(stream);
				}

				return file;

			}
		}


	}
}
