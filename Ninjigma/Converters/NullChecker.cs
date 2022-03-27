using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Ninjigma.Converters
{
	// Checks if a value is null in XAML
	// Used as so:
	// {Binding ElementName=[Element Name or Self], Path={Property}, Converter={StaticResource nullChecker}}
	public class NullChecker : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// return as boolean
			if ((parameter as string) == "boolean")
				return (value is null) ? false : true;

			// return as integer
			return (value is null) ? 0 : 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			// No way to reverse
			throw new NotImplementedException();
		}
	}
}
