using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Ninjigma.Converters
{
	class EnumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var enumerator = value as string;
			var type = parameter as string;

			Debug.WriteLine(value.GetType());

			Type enumType = Type.GetType(type);
			Type enumUnderlyingType = Enum.GetUnderlyingType(enumType);
			var enums = Enum.GetValues(enumUnderlyingType);
			foreach (object obj in enums)
			{
				if (Enum.GetName(enumUnderlyingType, obj) == enumerator)
				{
					return obj;
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var enumerator = value;

			Type enumUnderlyingType = targetType;
			var enums = Enum.GetValues(enumUnderlyingType);
			foreach (object obj in enums)
			{
				if (obj == enumerator)
				{
					return Enum.GetName(enumUnderlyingType, obj);
				}
			}

			return DependencyProperty.UnsetValue;
		}
	}
}
