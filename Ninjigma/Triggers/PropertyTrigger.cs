using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Ninjigma.Triggers
{
	class PropertyTrigger : StateTriggerBase
	{


		public object Value
		{
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(PropertyTrigger), new PropertyMetadata(0));

		public object Property
		{
			get { return (object)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Property.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PropertyProperty =
			DependencyProperty.Register("Property", typeof(object), typeof(PropertyTrigger), new PropertyMetadata(0, PropertyChanged));

		private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PropertyTrigger trigger = d as PropertyTrigger;

			if (e.NewValue == trigger.Value)
			{
				trigger.SetActive(true);
			} else
			{
				trigger.SetActive(false);
			}
		}
	}
}
