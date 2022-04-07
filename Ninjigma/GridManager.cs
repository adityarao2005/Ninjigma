using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Ninjigma
{
	// Manages a grid mainly for one item per cell type of grid
	public class GridManager
	{
		// The cells
		private List<Cell> cells;

		// read only cells
		public IList<Cell> Cells => cells.AsReadOnly();

		// Gets the userdata
		public T[,] GetUserData<T>()
		{
			// Creates the content array
			T[,] contents = new T[Rows, Columns];

			// Iterates each cell and get the userdata
			this.ForEach(cell =>
			{
				// If the cell is empty then exit
				if (!cell.IsEmpty)
				{
					contents[cell.Row, cell.Column] = (T)cell.UserData;
				}
			});

			// Return the contents
			return contents;
		}

		// The grid of the manager
		public Grid Grid
		{
			get;
		}

		// Rows and Columns of the Grid
		public int Rows => Grid.RowDefinitions.Count;
		public int Columns => Grid.ColumnDefinitions.Count;

		// Constructor which does the initialization
		public GridManager(Grid grid)
		{
			// Sets the grid
			Grid = grid;

			// Creates the list
			cells = new List<Cell>(Rows * Columns);

			// Populate the list with the cells
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					// Create the cell and assign its rows and columns
					Cell cell = new Cell(this);

					cell.Row = row;
					cell.Column = column;

					// Add the cell
					cells.Add(cell);
				}
			}
		}

		// Gets the cell of an element
		public Cell CellOf(FrameworkElement element)
		{
			// Get the cell where the content of the cell is the element
			return (from cell in Cells where cell.Content == element select cell).First();
		}

		// Get the cell from row and column
		public Cell this[int row, int column]
		{
			get
			{
				// Get the cell at the cell index
				Cell cell = Cells[row * Columns + column];

				// Check the cells validity
				if (cell.Row != row || cell.Column != column)
					throw new ArgumentOutOfRangeException();

				// return the cell
				return cell;
			}
		}

		// Iterates through the cells
		public void ForEach(Action<Cell> consumer)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					// invokes the action at cell at row and column
					consumer?.Invoke(this[row, column]);
				}
			}
		}

		// Iterates through the cells
		public void ForEach(Action<Cell, int> consumer)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					// invokes the action at cell at row and column and index
					consumer?.Invoke(this[row, column], row * Columns + column);
				}
			}
		}

		// The cells of the grid manager
		public class Cell
		{
			// The index of the cell
			public int Index => manager.Cells.IndexOf(this);

			// The row and column of the cell
			public int Row
			{
				get; set;
			}

			public int Column
			{
				get; set;
			}

			// Check if the content is empty
			public bool IsEmpty => Content == null;

			// Get the userdata from the Contents datacontext
			public object UserData
			{
				get
				{
					// Check if the cell is empty then perform the action
					if (!IsEmpty)
					{
						return Content.DataContext;
					}
					return null;
				}
				set
				{
					// Check if the cell is empty then perform the action
					if (!IsEmpty)
					{
						Content.DataContext = value;
					}
				}
			}

			// Get the readonly gridmanager
			public readonly GridManager manager;

			// Get the content at the grids child where the child is at set Row and column
			public FrameworkElement Content => manager.Grid.Children.Cast<FrameworkElement>().DefaultIfEmpty(null).FirstOrDefault((FrameworkElement elem) => Grid.GetRow(elem) == Row && Grid.GetColumn(elem) == Column);

			// Constructor which initializes the manager
			public Cell(GridManager manager)
			{
				this.manager = manager;
			}

			// DEBUG-ONLY
			// gets the string version of the cell
			public override string ToString()
			{
				return $"Cell [{Row}, {Column}], IsEmpty = {IsEmpty}";
			}
		}
	}
}
