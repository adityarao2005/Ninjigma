using Microsoft.Graphics.Canvas.Text;
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
	// The main/entrance page of the game window
	public sealed partial class MainPage : Page
	{
		// The diffiulty of the game
		private Difficulty difficulty = Difficulty.EASY;

		// The image chosen for the game
		public StorageFile Image
		{
			get { return (StorageFile)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(StorageFile), typeof(MainPage), new PropertyMetadata(null, ImageChanged));

		// When the image is changed set that as the current image
		private static void ImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MainPage page = d as MainPage;

			// Set the source to the path of the image file
			page.puzzleImage.Source = new BitmapImage(new Uri((e.NewValue as StorageFile).Path));
		}

		// Constructor of the main page
		public MainPage()
		{
			this.InitializeComponent();
		}

		// The click of the radiobuttons is handled here, it sets the difficulty
		private void ContentRadioButton_Click(object sender, RoutedEventArgs e)
		{
			// Get the radiobutton clicked
			ContentRadioButton button = sender as ContentRadioButton;
			// Sets the difficulty
			// Check the text and assign accordingly
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
		}

		// When the url button is clicked, it allows the user to load and use the url box
		private void URLButton_Click(object sender, RoutedEventArgs e)
		{
			urlBox.IsEnabled = true;
			loadButton.IsEnabled = true;
		}

		// When the file button is clicked, it prompts the user to choose a file on their system
		private async void FileButton_Click(object sender, RoutedEventArgs e)
		{
			// Set the urlbox to disabled
			urlBox.IsEnabled = false;
			loadButton.IsEnabled = false;

			// Create the file dialog
			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.ViewMode = PickerViewMode.Thumbnail;
			// Start location generally is in the picture location
			openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			// Generally the more newer images are made as a png, or jpeg. We do NOT support gifs in this
			openPicker.FileTypeFilter.Add(".jpg");
			openPicker.FileTypeFilter.Add(".jpeg");
			openPicker.FileTypeFilter.Add(".png");

			// We get the file if any
			StorageFile file = await openPicker.PickSingleFileAsync();
			// If the file is chosen then we keep it as our image
			if (file != null)
			{
				// Copy it to local folder
				Image = await IOUtil.CopyFileToLocalAsync(file);

				// Set the url and replace the path data to make it look like its from the url
				urlBox.Text = file.Path.Replace("\\", "/");
			}
			else
			{
				// Otherwise display error that the file has not been selected
				MessageDialog message = new MessageDialog("File not selected", "Error");
				await message.ShowAsync();
			}
		}

		// When the load url button is clicked, it loads the image from the url
		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			// Get the uri from the urlbox
			Uri uri = new Uri(urlBox.Text);

			// Check if we are allowed to access these domains first
			try
			{
				StorageFile file = await IOUtil.CopyURLContentToFile(uri, mime => mime == "image/jpeg" || mime == "image/png");

				// If the file is invalid then do not set the image
				if (file != null)
					Image = file;
			}
			catch (InvalidOperationException)
			{
				// Generally this error comes when we have a uwp app
				// File permissions become a problem here
				MessageDialog dlg = new MessageDialog(
					"It seems you have not granted permission for this app to access the file system broadly. " +
					"Without this permission, the app will only be able to access a very limited set of filesystem locations. " +
					"You can grant this permission in the Settings app, if you wish. You can do this now or later. " +
					"If you change the setting while this app is running, it will terminate the app so that the " +
					"setting can be applied. Do you want to do this now? If not then access files through the upload file option instead.",
					"File system permissions");

				// Set the commmands for the dialog so that it can call ms:// protocal to enable file permissions
				dlg.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.InitMessageDialogHandler), 0));
				dlg.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.InitMessageDialogHandler), 1));
				// Set the command indices
				dlg.DefaultCommandIndex = 0;
				dlg.CancelCommandIndex = 1;
				// Show the dialog
				await dlg.ShowAsync();

				return;
			} catch (Exception ex)
			{
				// Any other errors get reported here.
				MessageDialog dlg = new MessageDialog($"An error has occurred, we will fix as soon as possible. Contact us straight away. Exception details: {ex.Message}", "Error");
				await dlg.ShowAsync();

				return;
			}
		}

		// The message handler to tell the user to allow file permissions for this app
		private async void InitMessageDialogHandler(IUICommand command)
		{
			// If the command is yes then launch the settings for this
			if ((int)command.Id == 0)
			{
				await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
			}
		}

		// If something has been dragged over, determine first if it is an image and then reject or accept it
		private async void Image_DragOver(object sender, DragEventArgs e)
		{
			// Set the url stuf to be disabled
			urlBox.IsEnabled = false;
			loadButton.IsEnabled = false;

			// Set the caption of the dragged item and make sure that you can see what you are dragging
			e.DragUIOverride.Caption = "Drop Image Here";
			e.DragUIOverride.IsCaptionVisible = true;
			e.DragUIOverride.IsContentVisible = true;
			e.DragUIOverride.IsGlyphVisible = true;
			// Make sure we are copying from it
			e.AcceptedOperation = DataPackageOperation.Copy;
			//check the type of the file is an image
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				// Get the files and check them
				var items = await e.DataView.GetStorageItemsAsync();
				if (items.Any())
				{
					// Get the first/"only" file
					var storageFile = items[0] as StorageFile;
					// If it is not a jpeg or png file then do not allow it
					if (storageFile.FileType != ".jpg" && storageFile.FileType != ".jpeg" && storageFile.FileType != ".png")
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

		// When we drop the image we must also do the same check as when it is dragged before we set it as the one we want
		private async void Image_Drop(object sender, DragEventArgs e)
		{
			// Check the dataview to see if it contains any files
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				// Check the files to see if they are images
				var items = await e.DataView.GetStorageItemsAsync();
				if (items.Any())
				{
					// Get the first file and check whether it is a supported image
					var storageFile = items[0] as StorageFile;
					var contentType = storageFile.ContentType;
					if (contentType == "image/jpg" || contentType == "image/png" || contentType == "image/jpeg")
					{
						// Copy the image and set it as the main one
						Image = await IOUtil.CopyFileToLocalAsync(storageFile);
					}
				}
			}
		}

		// This function occurs when the play button is clicked, what occurs is we navigate to the game
		private void playButton_Click(object sender, RoutedEventArgs e)
		{
			// Save our params in an array
			object[] args = new object[] { Image, difficulty };

			// navigate to the game
			Frame.Navigate(typeof(GamePage), args);
		}

	}
}