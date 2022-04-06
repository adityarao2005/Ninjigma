using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninjigma.Util.Structures
{
	// The indexer used for the matrixes
	public struct Indexer
	{
		public int Row { get; set; }
		public int Column { get; set; }
	}

	// The matrix data structure
	public struct Matrix<T>
	{
		// The actual matrix array
		public T[,] ActualMatrix { get; }
		// Rows and columns from the actual matrix
		public int Rows => ActualMatrix.GetLength(0);
		public int Columns => ActualMatrix.GetLength(1);

		// The matrix constructor that takes in one array
		public Matrix(T[,] array)
		{
			ActualMatrix = ArrayUtil.Copy(array);
		}

		// The matrix constructor that makes one array
		public Matrix(int rows, int columns)
		{
			ActualMatrix = new T[rows, columns];
		}

		// retrieves the object at the index
		public T this[int row, int column]
		{
			get => ActualMatrix[row, column];
			set => ActualMatrix[row, column] = value;
		}

		// Gets the index of the object
		public Indexer IndexOf(T t)
		{
			// Iterate through every row and column
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					// Get the item and check for equality
					T cont = this[row, column];
					if (cont.Equals(t))
					{
						// Return index if equal
						return new Indexer() { Row = row, Column = column };
					}
				}
			}

			// Return invalid index
			return new Indexer() { Row = -1, Column = -1 };
		}

		// Return the object at the specified index
		public T this[Indexer index]
		{
			get => ActualMatrix[index.Row, index.Column];
			set => ActualMatrix[index.Row, index.Column] = value;
		}

		// Swap the two index values
		public void Swap(Indexer first, Indexer second)
		{
			// Temporary variables
			T temp = this[first];

			// Perform the swap
			this[first] = this[second];
			this[second] = temp;
		}

		// Check if index is valid or not
		public bool Validate(Indexer index)
		{
			return 0 <= index.Row && index.Row < Rows && 0 <= index.Column && index.Column < Columns;
		}

		// Swap the up index
		public bool SwapUp(Indexer index, Predicate<T> predicate = null)
		{
			// Check if the index is in a valid range
			if (index.Row == 0 || !Validate(index))
			{
				return false;
			}

			// Get the index above
			Indexer indexer = new Indexer() { Row = index.Row - 1, Column = index.Column };

			// Check if you really want to swap both indices
			if (predicate != null)
				if (!predicate(this[indexer]))
					return false;

			// Swap them
			Swap(index, indexer);

			return true;
		}

		// Swap the down index
		public bool SwapDown(Indexer index, Predicate<T> predicate = null)
		{
			// Check if the index is in a valid range
			if (index.Row == Rows - 1 || !Validate(index))
				return false;

			// Get the index below
			Indexer indexer = new Indexer() { Row = index.Row + 1, Column = index.Column };

			// Check if you really want to swap both indices
			if (predicate != null)
				if (!predicate(this[indexer]))
					return false;

			// Swap them
			Swap(index, indexer);

			return true;
		}

		// Swap the left index
		public bool SwapLeft(Indexer index, Predicate<T> predicate = null)
		{
			// Check if the index is in a valid range
			if (index.Column == 0 || !Validate(index))
				return false;

			// Get the index left
			Indexer indexer = new Indexer() { Column = index.Column - 1, Row = index.Row };

			// Check if you really want to swap both indices
			if (predicate != null)
				if (!predicate(this[indexer]))
					return false;

			// Swap them
			Swap(index, indexer);

			return true;
		}

		// Swap the right index
		public bool SwapRight(Indexer index, Predicate<T> predicate = null)
		{
			// Check if the index is in a valid range
			if (index.Column == Columns - 1 || !Validate(index))
				return false;

			// Get the index right
			Indexer indexer = new Indexer() { Column = index.Column + 1, Row = index.Row };

			// Check if you really want to swap both indices
			if (predicate != null)
				if (!predicate(this[indexer]))
					return false;

			// Swap them
			Swap(index, indexer);

			return true;
		}

		// DEBUG ONLY
		// Prints the data structure
		public void Print()
		{
			// For each row print in the form of an array
			for (int row = 0; row < Rows; row++)
			{
				Debug.Write("[");
				for (int column = 0; column < Columns - 1; column++)
				{
					Debug.Write($"({ActualMatrix[row, column]}),");
				}
				Debug.WriteLine($"({ActualMatrix[row, Columns - 1]})]");
			}
		}
	}
}
