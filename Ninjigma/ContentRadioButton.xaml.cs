using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Ninjigma
{
	public sealed partial class ContentRadioButton : UserControl
	{
		public ContentRadioButton()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ContentRadioButton), new PropertyMetadata(null));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(UIElement), typeof(ContentRadioButton), new PropertyMetadata(null));

		public UIElement Value
		{
			get { return (UIElement)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(ContentRadioButton), new PropertyMetadata(null));

		public string GroupName
		{
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}

		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ContentRadioButton), new PropertyMetadata(null));

		public bool IsSelected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}

		public event RoutedEventHandler Click;

		private void RadioButton_Click(object sender, RoutedEventArgs e) => Click.Invoke(this, e);


	}
}
