using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninjigma.Util
{
	static class ArrayUtil
	{
		public static bool SequenceEquals<T>(this T[,] a, T[,] b) => a.Rank == b.Rank && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d)) && a.Cast<T>().SequenceEqual(b.Cast<T>());
	}
}
