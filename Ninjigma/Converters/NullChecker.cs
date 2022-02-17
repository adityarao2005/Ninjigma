using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Ninjigma.Converters
{
	public class NullChecker : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((parameter as string) == "boolean")
				return (value is null) ? false : true;

			return (value is null) ? 0 : 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
