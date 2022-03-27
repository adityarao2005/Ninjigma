using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninjigma.Util
{
	// Utilities for array objects (1D, jagged, multi-D)
	static class ArrayUtil
	{
		// Multi-D array comparison
		public static bool SequenceEquals<T>(this T[,] a, T[,] b) =>
			// Check dimension
			a.Rank == b.Rank && 
			// Enumerate through arrays and check lengths
			Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d)) &&
			// Check if the entire sequence is equal
			a.Cast<T>().SequenceEqual(b.Cast<T>());

		public static T[,] Copy<T>(this T[,] a)
		{
			T[,] b = new T[a.GetLength(0), a.GetLength(1)];
			for (int row = 0; row < a.GetLength(0); row++)
			{
				for (int column = 0; column < a.GetLength(1); column++)
				{
					b[row, column] = a[row, column];
				}
			}
			return b;
		}

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T t in enumerable)
			{
				action.Invoke(t);
			}
		}
	}


}
