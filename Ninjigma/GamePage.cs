using Ninjigma.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Ninjigma.GridManager;

using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewBackRequestedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs;

namespace Ninjigma
{
	public abstract partial class GamePage : Page
	{
		public abstract Grid GameGrid();

		private GridManager manager;

		public SoftwareBitmap Image
		{
			get { return (SoftwareBitmap)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		private Point[,] originals;

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(SoftwareBitmap), typeof(GamePage_Easy), new PropertyMetadata(null));


		public GamePage()
		{
			Initialize();
			manager = new GridManager(GameGrid());
		}

		public abstract void Initialize();

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			StorageFile file = e.Parameter as StorageFile;

			Image = await ImageUtil.FromFile(file);

			SoftwareBitmap[,] bitmaps = await ImageUtil.Split(ImageUtil.Copy(Image), manager.Rows, manager.Columns);


			originals = new Point[manager.Rows, manager.Columns];
			manager.ForEach((consumer) =>
			{

				SoftwareBitmap bitmap = bitmaps[consumer.Row, consumer.Column];
				SoftwareBitmapSource source = new SoftwareBitmapSource();

				new Task(async () => await source.SetBitmapAsync(SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied))).RunSynchronously();

				if (consumer.Row < manager.Rows - 1 || consumer.Column < manager.Columns - 1)
				{
					Point point = new Point(consumer.Row, consumer.Column);
					consumer.UserData = point;
					originals[consumer.Row, consumer.Column] = point;
					((consumer.Content as Border).Child as Image).Source = source;
				}
			});

			var rnd = new Random();
			var elems = new List<FrameworkElement>(manager.Grid.Children.Cast<FrameworkElement>()).OrderBy(elem => rnd.Next()).ToList();

			for (int i = 0; i < elems.Count(); i++)
			{
				Grid.SetRow(elems[i], i / manager.Rows);
				Grid.SetColumn(elems[i], i % manager.Rows);
			}

			SoftwareBitmapSource source_ = new SoftwareBitmapSource();
			await source_.SetBitmapAsync(SoftwareBitmap.Convert(SoftwareBitmap.Copy(Image), BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied));
			HelpImage().Source = source_;

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

		public abstract Image HelpImage();

		protected async void PieceTapped(object sender, TappedRoutedEventArgs e)
		{
			var cell = manager.CellOf(sender as FrameworkElement);

			try
			{
				Cell up = null;
				Cell down = null;
				Cell left = null;
				Cell right = null;
				try
				{
					up = manager[cell.Row - 1, cell.Column];
				}
				catch { up = null; }
				try
				{
					down = manager[cell.Row + 1, cell.Column];
				}
				catch { down = null; }
				try
				{
					left = manager[cell.Row, cell.Column - 1];
				}
				catch { left = null; }
				try
				{
					right = manager[cell.Row, cell.Column + 1];
				}
				catch { right = null; }


				if (up != null && up.IsEmpty)
				{
					var content = cell.Content;

					Grid.SetRow(content, up.Row);
				}
				else if (down != null && down.IsEmpty)
				{
					var content = cell.Content;

					Grid.SetRow(content, down.Row);
				}
				else if (left != null && left.IsEmpty)
				{
					var content = cell.Content;

					Grid.SetColumn(content, left.Column);
				}
				else if (right != null && right.IsEmpty)
				{
					var content = cell.Content;

					Grid.SetColumn(content, right.Column);
				}
				else { return; }

				Point[,] temps = new Point[manager.Rows, manager.Columns];
				temps = manager.GetUserData<Point>();

				bool isEqual = true;
				for (int counter1 = 0; counter1 < temps.GetLength(0); counter1++)
				{
					for (int counter2 = 0; counter2 < originals.GetLength(1); counter2++)
					{
						Point Current = temps[counter1, counter2];
						Point Supposed = originals[counter1, counter2];
						if (Current != Supposed)
						{
							isEqual = false;
							break;
						}
					}
				}

				if (isEqual)
				{
					MessageDialog message = new MessageDialog("YOU WON!!");
					await message.ShowAsync();
				}
			}
			catch (Exception ex)
			{
				MessageDialog dialog = new MessageDialog(ex.StackTrace, "Failed because:");

				await dialog.ShowAsync();

				throw ex;
			}
		}

	}
}
