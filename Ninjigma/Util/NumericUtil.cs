using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninjigma.Util
{
	// Utility class for numerics
	public static class NumericUtil
	{
		// Check if number is in between left and right
		public static bool IsBetween(this double num, double left, double right)
		{
			// perform check
			return num >= left && num < right;
		}

	}
}
