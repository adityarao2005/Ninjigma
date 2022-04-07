using Ninjigma.Util;
using Ninjigma.Util.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Ninjigma
{
	// The game base code
	public abstract class GridGame : Page
	{
		// The game grid, different for each implementation
		public abstract Grid GameGrid();

		// The grid manager, manages all the cells with in the grid
		private GridManager manager;

		// The image that will be diced for the game
		public SoftwareBitmap Image
		{
			get { return (SoftwareBitmap)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		// The original points
		private Point?[,] originals;

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(SoftwareBitmap), typeof(GridGame), new PropertyMetadata(null));

		// The constructor for the game
		public GridGame()
		{
			// Allows the child class to call the "this.InitializeComponent()" method
			Initialize();
			// Create the grid manager
			manager = new GridManager(GameGrid());
		}

		// Allows the child class to call the "this.InitializeComponent()" method
		public abstract void Initialize();

		// Handles navigation call from GamePage
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			// Executes the base navigation
			base.OnNavigatedTo(e);

			// Gets the image parameter
			Image = e.Parameter as SoftwareBitmap;

			// Dices the bitmaps
			SoftwareBitmap[,] bitmaps = await ImageUtil.Split(ImageUtil.Copy(Image), manager.Rows, manager.Columns);

			// Initialize the original points
			originals = new Point?[manager.Rows, manager.Columns];
			// Iterate through each cell and set its point and bitmap
			manager.ForEach((consumer) =>
			{
				// Get the bitmap at he cords
				SoftwareBitmap bitmap = bitmaps[consumer.Row, consumer.Column];
				SoftwareBitmapSource source = new SoftwareBitmapSource();

				// Add the bitmap to the source
				new Task(async () => await source.SetBitmapAsync(SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied))).RunSynchronously();

				// If the row and columns are not the last row and column
				if (consumer.Row < manager.Rows - 1 || consumer.Column < manager.Columns - 1)
				{
					// Get the point and assign it to its userdata
					Point point = new Point(consumer.Row, consumer.Column);
					consumer.UserData = point;
					// Set the points in the originals
					originals[consumer.Row, consumer.Column] = point;
					// Set the image
					((consumer.Content as Border).Child as Image).Source = source;
				}
			});

			// Randomize the grid
			NewRandomAlgorthm();
		}

		// Gets the childs required amount of cycles
		public abstract int CYCLES();

		// Randomizes grid as CYCLES() times
		public void NewRandomAlgorthm()
		{
			// Create the matrix data structure
			Matrix<Point?> points = new Matrix<Point?>(originals);

			// DEBUG-ONLY
			// Prints the points
			points.Print();

			// Get the randomizer
			var rnd = new Random();

			// Get the rows and columns before hand to not populate the stack
			int rows = manager.Rows;
			int columns = manager.Columns;
			// For every cycle make a move
			for (int i = 0; i < CYCLES(); i++)
			{
				//Effective yet time consuming
				//while (!TryMove(manager.CellOf(manager.Grid.Children[rnd.Next(0, manager.Grid.Children.Count())] as FrameworkElement))) ;
				//More efficient to render all at once required

				// Until the cell has been "moved" do the following
				do
				{
					// Get the random index
					Indexer indexer = new Indexer() { Row = rnd.Next(0, points.Rows), Column = rnd.Next(0, points.Columns) };

					// Create the predicate checker
					Predicate<Point?> predicate = (t) => t == null;

					// Perform the swap and check if the cell has been moved
					if (points.SwapUp(indexer, predicate)) break;
					if (points.SwapDown(indexer, predicate)) break;
					if (points.SwapLeft(indexer, predicate)) break;
					if (points.SwapRight(indexer, predicate)) break;

				} while (true);
			}

			// DEBUG-ONLY
			// Print the points after its done
			points.Print();


			// Use dictorary to map coordinates then assign new ones
			Dictionary<FrameworkElement, Indexer> map = new Dictionary<FrameworkElement, Indexer>();

			// For each child assign the elements coords
			manager.Grid.Children.ForEach(elem => map.Add(elem as FrameworkElement, points.IndexOf(new Point() { X = Grid.GetRow(elem as FrameworkElement), Y = Grid.GetColumn(elem as FrameworkElement) })));

			// For each element set its assigned coords
			map.ForEach(pair =>
			{
				// Get the element and index
				FrameworkElement element = pair.Key;
				Indexer index = pair.Value;

				// Set the row and column
				Grid.SetRow(element, index.Row);
				Grid.SetColumn(element, index.Column);
			});
		}

		// Originally we just set the elements at a random area by ordering it randomly
		public void OriginalRandomizeAlgorthm()
		{
			// Create the randomizer
			var rnd = new Random();
			// Create an ordered list and order it by the randomized value
			var elems = new List<FrameworkElement>(manager.Grid.Children.Cast<FrameworkElement>()).OrderBy(elem => rnd.Next()).ToList();

			// Set its row and column based on index
			for (int i = 0; i < elems.Count(); i++)
			{
				// Row is index / Rows, Column is index REM Rows
				Grid.SetRow(elems[i], i / manager.Rows);
				Grid.SetColumn(elems[i], i % manager.Rows);
			}
		}

		// Create the states of the games
		private bool started, ended, paused;

		// Create a public ended state
		public bool HasEnded => ended;

		// Called when a piece is tapped
		public async void PieceTapped(object sender, TappedRoutedEventArgs e)
		{
			// If the game has not started, then start the game
			if (!started)
			{
				GameStarted.Invoke();
				started = true;
			}

			// If it has ended or been paused do nothing
			if (ended || paused)
			{
				return;
			}

			// Try to move the piece or do nothing
			try
			{
				// Get the cell of the sender
				var cell = manager.CellOf(sender as FrameworkElement);

				// Try to move the cell, exception is generally fired here
				TryMove(cell);

				// Create a temporary array for the current order
				Point?[,] temps = new Point?[manager.Rows, manager.Columns];
				temps = manager.GetUserData<Point?>();

				// Iterate through all the current grids and see if they are the original
				// Create a flag
				bool isEqual = true;
				// 2D iteration
				for (int counter1 = 0; counter1 < temps.GetLength(0); counter1++)
				{
					for (int counter2 = 0; counter2 < originals.GetLength(1); counter2++)
					{
						// Get the current point and supposed to be points
						Point? Current = temps[counter1, counter2];
						Point? Supposed = originals[counter1, counter2];

						// if these points are the same then do nothing
						if (Current == Supposed) { }
						else if (Current != null && Supposed != null && Current.Equals(Supposed)) { }
						// otherwise flag the flag and break the loop
						else
						{
							isEqual = false;
							break;
						}
					}
				}

				// if the flag has not been flagged then end the game
				if (isEqual)
				{
					// End the game
					GameEnded.Invoke();
					ended = true;

					// Disclaim this to the world (sarcasm)
					// Just says you won
					MessageDialog message = new MessageDialog("YOU WON!!");
					await message.ShowAsync();
				}
			}
			// BUG-TEST
			// DEBUG-ONLY
			// If any error occurs display that an error has occurred
			catch (Exception ex)
			{
				// Make the user know about this
				MessageDialog dialog = new MessageDialog(ex.StackTrace, "The app is going to crash because of an unhandled error");
				await dialog.ShowAsync();

				// Crash the app
				throw ex;
			}
		}

		// Try to move the cell
		private bool TryMove(Cell cell)
		{
			// Get the cells adjecent to it
			Cell up = null;
			Cell down = null;
			Cell left = null;
			Cell right = null;

			// Check if the cells actually exist
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

			// Check if the cell above it is empty
			if (up != null && up.IsEmpty)
			{
				// "Swap" them
				var content = cell.Content;
				// Really its just pushing it
				Grid.SetRow(content, up.Row);
			}
			// Check if the cell below it is empty
			else if (down != null && down.IsEmpty)
			{
				// "Swap" them
				var content = cell.Content;
				// Really its just pushing it
				Grid.SetRow(content, down.Row);
			}
			// Check if the cell left it is empty
			else if (left != null && left.IsEmpty)
			{
				// "Swap" them
				var content = cell.Content;

				// Really its just pushing it
				Grid.SetColumn(content, left.Column);
			}
			// Check if the cell right it is empty
			else if (right != null && right.IsEmpty)
			{
				// "Swap" them
				var content = cell.Content;
				// Really its just pushing it
				Grid.SetColumn(content, right.Column);
			}
			// Otherwise report that moving failed
			else { return false; }

			// Send feedback
			return true;
		}

		// Create the events
		public event Action GameStarted, GameEnded;

		// Pause and unpause functions for the game
		public void Pause()
		{
			paused = true;
		}

		public void Unpause()
		{
			paused = false;
		}

	}
}
