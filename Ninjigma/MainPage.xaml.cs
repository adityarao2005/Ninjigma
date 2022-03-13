using Ninjigma.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ninjigma
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private Difficulty difficulty;


		private StorageFile oldValue;



		public StorageFile Image
		{
			get { return (StorageFile)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(StorageFile), typeof(MainPage), new PropertyMetadata(null, ImageChanged));

		private static void ImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MainPage page = d as MainPage;

			page.oldValue = e.OldValue as StorageFile;

			page.puzzleImage.Source = new BitmapImage(new Uri((e.NewValue as StorageFile).Path));
		}

		public MainPage()
		{
			this.InitializeComponent();
		}

		private void ContentRadioButton_Click(object sender, RoutedEventArgs e)
		{

			ContentRadioButton button = sender as ContentRadioButton;
			Difficulty difficulty;
			switch (button.Text)
			{
				case "Medium":
					difficulty = Difficulty.MEDIUM;
					break;
				case "Hard":
					difficulty = Difficulty.HARD;
					break;
				case "Easy":
				default:
					difficulty = Difficulty.EASY;
					break;
			}

			this.difficulty = difficulty;
		}

		private void URLButton_Click(object sender, RoutedEventArgs e)
		{
			urlBox.IsEnabled = true;
			loadButton.IsEnabled = true;
		}

		private async void FileButton_Click(object sender, RoutedEventArgs e)
		{
			urlBox.IsEnabled = false;
			loadButton.IsEnabled = false;

			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.ViewMode = PickerViewMode.Thumbnail;
			openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			openPicker.FileTypeFilter.Add(".jpg");
			openPicker.FileTypeFilter.Add(".jpeg");
			openPicker.FileTypeFilter.Add(".png");

			StorageFile file = await openPicker.PickSingleFileAsync();
			if (file != null)
			{
				Image = await IOUtil.CopyFileToLocalAsync(file);

				urlBox.Text = file.Path.Replace("\\", "/");
			}
			else
			{
				MessageDialog message = new MessageDialog("File not selected", "Error");
				await message.ShowAsync();
			}
		}

		//TODO: Must complete
		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			Uri uri = new Uri(urlBox.Text);

			try
			{
				StorageFile file = await IOUtil.CopyURLContentToFile(uri, mime => mime == "image/jpeg" || mime == "image/png");

				Image = file;
			}
			catch
			{
				MessageDialog dlg = new MessageDialog(
					"It seems you have not granted permission for this app to access the file system broadly. " +
					"Without this permission, the app will only be able to access a very limited set of filesystem locations. " +
					"You can grant this permission in the Settings app, if you wish. You can do this now or later. " +
					"If you change the setting while this app is running, it will terminate the app so that the " +
					"setting can be applied. Do you want to do this now? If not then access files through the upload file option instead.",
					"File system permissions");
				dlg.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.InitMessageDialogHandler), 0));
				dlg.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.InitMessageDialogHandler), 1));
				dlg.DefaultCommandIndex = 0;
				dlg.CancelCommandIndex = 1;
				await dlg.ShowAsync();

				return;
			}
		}

		private async void InitMessageDialogHandler(IUICommand command)
		{
			if ((int)command.Id == 0)
			{
				await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
			}
		}

		private async void Image_DragOver(object sender, DragEventArgs e)
		{
			urlBox.IsEnabled = false;
			loadButton.IsEnabled = false;

			e.DragUIOverride.Caption = "Drop Image Here";
			e.DragUIOverride.IsCaptionVisible = true;
			e.DragUIOverride.IsContentVisible = true;
			e.DragUIOverride.IsGlyphVisible = true;
			e.AcceptedOperation = DataPackageOperation.Copy;
			//check the type of the file
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items = await e.DataView.GetStorageItemsAsync();
				if (items.Any())
				{
					var storageFile = items[0] as StorageFile;
					if (storageFile.FileType != ".jpg" && storageFile.FileType != ".png")
					{
						//for disallowed file types
						e.AcceptedOperation = DataPackageOperation.None;
					}
				}
			}
			else
			{
				//for disallowed file types
				e.AcceptedOperation = DataPackageOperation.None;
			}
		}

		private async void Image_Drop(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items = await e.DataView.GetStorageItemsAsync();
				if (items.Any())
				{
					var storageFile = items[0] as StorageFile;
					var contentType = storageFile.ContentType;
					if (contentType == "image/jpg" || contentType == "image/png" || contentType == "image/jpeg")
					{
						Image = await IOUtil.CopyFileToLocalAsync(storageFile);
					}
				}
			}
		}

		private void playButton_Click(object sender, RoutedEventArgs e)
		{
			switch (difficulty)
			{
				case Difficulty.EASY:
					Frame.Navigate(typeof(GamePage_Easy), Image);
					break;
				case Difficulty.MEDIUM:
					Frame.Navigate(typeof(GamePage_Medium), Image);
					break;
				case Difficulty.HARD:
					Frame.Navigate(typeof(GamePage_Hard), Image);
					break;
			}
		}
	}
}