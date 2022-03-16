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
	public class GridManager
	{
		private List<Cell> cells;

		public IList<Cell> Cells => cells.AsReadOnly();

		public T[,] GetUserData<T>()
		{
			T[,] contents = new T[Rows, Columns];

			this.ForEach(cell =>
			{
				if (!cell.IsEmpty)
				{
					Debug.WriteLine(cell.UserData == null ? "null" : cell.UserData.ToString());
					contents[cell.Row, cell.Column] = (T)cell.UserData;
				}
			});

			return contents;
		}

		public Grid Grid
		{
			get;
		}

		public int Rows => Grid.RowDefinitions.Count;

		public int Columns => Grid.ColumnDefinitions.Count;

		public GridManager(Grid grid)
		{
			Grid = grid;

			cells = new List<Cell>(Rows * Columns);

			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					Cell cell = new Cell(this);

					cell.Row = row;
					cell.Column = column;

					cells.Add(cell);
				}
			}
		}

		public Cell CellOf(FrameworkElement element)
		{
			return (from cell in Cells where cell.Content == element select cell).First();
		}

		public Cell this[int row, int column]
		{
			get
			{
				Cell cell = Cells[row * Columns + column];

				if (cell.Row != row || cell.Column != column)
					throw new ArgumentOutOfRangeException();

				return cell;
			}
		}

		public void ForEach(Action<Cell> consumer)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					consumer?.Invoke(this[row, column]);
				}
			}
		}

		public void ForEach(Action<Cell, int> consumer)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					consumer?.Invoke(this[row, column], row * Columns + column);
				}
			}
		}


		public class Cell
		{
			public int Index => manager.Cells.IndexOf(this);

			public int Row
			{
				get; set;
			}

			public int Column
			{
				get; set;
			}

			public bool IsEmpty => Content == null;

			public object UserData
			{
				get
				{
					if (!IsEmpty)
					{
						return Content.DataContext;
					}
					return null;
				}
				set
				{
					if (!IsEmpty)
					{
						Content.DataContext = value;
					}
				}
			}

			public readonly GridManager manager;

			public FrameworkElement Content => manager.Grid.Children.Cast<FrameworkElement>().DefaultIfEmpty(null).FirstOrDefault((FrameworkElement elem) => Grid.GetRow(elem) == Row && Grid.GetColumn(elem) == Column);

			public Cell(GridManager manager)
			{
				this.manager = manager;
			}

			public override string ToString()
			{
				return $"Cell [{Row}, {Column}], IsEmpty = {IsEmpty}";
			}
		}
	}
}
