using Ninjigma.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
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
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ninjigma
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	// The game page. Where the grid gets initialized and the player plays.
	public sealed partial class GamePage : Page
	{
		// Counting timer
		private DispatcherTimer Timer = new DispatcherTimer();

		// The image
		public SoftwareBitmap Image
		{
			get { return (SoftwareBitmap)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		// The Image as a property 
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(SoftwareBitmap), typeof(GamePage), new PropertyMetadata(null));

		// The actual game
		public GridGame Game => gameFrame.Content as GridGame;

		public GamePage()
		{
			// initialize xaml
			this.InitializeComponent();

			// Set the ticker
			Timer.Tick += Timer_Tick;
			// Tick every 1s
			Timer.Interval = new TimeSpan(0, 0, 1);
		}

		// the time
		private TimeSpan time = new TimeSpan(0, 0, 0);
		private void Timer_Tick(object sender, object e)
		{
			// increment the time
			time += Timer.Interval;
			// Set the text to time to formatted as H:mm:ss
			Time.Text = new DateTime(time.Ticks).ToString("H:mm:ss");
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			// Render content
			base.OnNavigatedTo(e);

			// get navigation parameters
			object[] param = e.Parameter as object[];

			// get the file image and difficulty from the params
			StorageFile file = param[0] as StorageFile;
			Difficulty difficulty = (Difficulty)param[1];

			//
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


			Game.GameStarted += Timer.Start;
			Game.GameEnded += Timer.Stop;
		}

		public async void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			Timer.Stop();
			Game.Pause();

			MessageDialog dialog = new MessageDialog("Do you really want to really head back?", "Alert");
			dialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
			dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;

			IUICommand command = await dialog.ShowAsync();
			int id = (int)command.Id;

			if (id == dialog.CancelCommandIndex)
			{
				Game.Unpause();
				Timer.Start();
				return;
			}

			Frame.Navigate(typeof(MainPage));
		}

		private async void MarkUnsolvable(object sender, RoutedEventArgs e)
		{
			Timer.Stop();
			Game.Pause();


			MessageDialog dialog = new MessageDialog("Are you sure you cannot solve this?", "Alert");
			dialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
			dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;

			IUICommand command = await dialog.ShowAsync();
			int id = (int)command.Id;

			if (id == dialog.CancelCommandIndex)
			{
				Game.Unpause();
				Timer.Start();
				return;
			}

			Frame.Navigate(typeof(MainPage));

		}


	}
}
