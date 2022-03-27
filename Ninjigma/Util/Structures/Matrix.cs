using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninjigma.Util.Structures
{
	public struct Indexer
	{
		public int Row { get; set; }
		public int Column { get; set; }
	}

	public struct Matrix<T>
	{
		public T[,] ActualMatrix { get; }
		public int Rows => ActualMatrix.GetLength(0);
		public int Columns => ActualMatrix.GetLength(1);

		public Matrix(T[,] array)
		{
			ActualMatrix = ArrayUtil.Copy(array);
		}

		public Matrix(int rows, int columns)
		{
			ActualMatrix = new T[rows, columns];
		}

		public T this[int row, int column]
		{
			get => ActualMatrix[row, column];
			set => ActualMatrix[row, column] = value;
		}

		public Indexer IndexOf(T t)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					T cont = this[row, column];
					if (cont.Equals(t))
					{
						return new Indexer() { Row = row, Column = column };
					}
				}
			}

			return new Indexer() { Row = -1, Column = -1 };
		}

		public T this[Indexer index]
		{
			get => ActualMatrix[index.Row, index.Column];
			set => ActualMatrix[index.Row, index.Column] = value;
		}

		public void Swap(Indexer first, Indexer second)
		{
			T temp = this[first];

			this[first] = this[second];
			this[second] = temp;
		}

		public bool Validate(Indexer index)
		{
			return 0 <= index.Row && index.Row < Rows && 0 <= index.Column && index.Column < Columns;
		}

		public bool SwapUp(Indexer index)
		{
			if (index.Row == 0 || !Validate(index))
			{
				return false;
			}

			Swap(index, new Indexer() { Row = index.Row - 1, Column = index.Column });

			return true;
		}

		public bool SwapDown(Indexer index)
		{
			if (index.Row == Rows - 1 || !Validate(index))
				return false;

			Swap(index, new Indexer() { Row = index.Row + 1, Column = index.Column });

			return true;
		}


		public bool SwapLeft(Indexer index)
		{
			if (index.Column == 0 || !Validate(index))
				return false;

			Swap(index, new Indexer() { Column = index.Column - 1, Row = index.Row });

			return true;
		}

		public bool SwapRight(Indexer index)
		{
			if (index.Column == Columns - 1 || !Validate(index))
				return false;

			Swap(index, new Indexer() { Column = index.Column + 1, Row = index.Row });

			return true;
		}

		public void Print()
		{
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
