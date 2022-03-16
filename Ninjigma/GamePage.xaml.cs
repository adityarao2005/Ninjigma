﻿using Ninjigma.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewBackRequestedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ninjigma
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePage : Page
	{


		public SoftwareBitmap Image
		{
			get { return (SoftwareBitmap)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(SoftwareBitmap), typeof(GamePage), new PropertyMetadata(null));

		public GamePage()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			object[] param = e.Parameter as object[];

			StorageFile file = param[0] as StorageFile;
			Difficulty difficulty = (Difficulty) param[1];

			Image = await ImageUtil.FromFile(file);

			SoftwareBitmapSource source_ = new SoftwareBitmapSource();
			await source_.SetBitmapAsync(SoftwareBitmap.Convert(SoftwareBitmap.Copy(Image), BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied));
			helpImage.Source = source_;


			switch (difficulty)
			{
				case Difficulty.EASY:
					gameFrame.Navigate(typeof(EasyGrid), Image);
					break;
				case Difficulty.MEDIUM:
					gameFrame.Navigate(typeof(MediumGrid), Image);
					break;
				case Difficulty.HARD:
					gameFrame.Navigate(typeof(HardGrid), Image);
					break;
			}
		}

		public async void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			MessageDialog dialog = new MessageDialog("Do you really want to really head back?", "Alert");
			dialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
			dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;

			IUICommand command = await dialog.ShowAsync();
			int id = (int)command.Id;

			if (id == dialog.CancelCommandIndex)
			{
				return;
			}

			Frame.Navigate(typeof(MainPage));
		}

	}
}
