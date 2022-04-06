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
	// Create a new user control which adds content to radio buttons
	public sealed partial class ContentRadioButton : UserControl
	{
		public ContentRadioButton()
		{
			// Initializes the component
			this.InitializeComponent();
		}

		// The text of the radio button
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ContentRadioButton), new PropertyMetadata(null));

		// Text property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		// The content value
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(UIElement), typeof(ContentRadioButton), new PropertyMetadata(null));

		// content value property
		public UIElement Value
		{
			get { return (UIElement)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		// Group name of radiobutton
		public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(ContentRadioButton), new PropertyMetadata(null));

		// Group name property
		public string GroupName
		{
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}

		// Selection property
		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ContentRadioButton), new PropertyMetadata(null));

		// Check selected
		public bool IsSelected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}

		// Click event handler
		public event RoutedEventHandler Click;

		// Handles the radiobutton click event
		private void RadioButton_Click(object sender, RoutedEventArgs e) => Click.Invoke(this, e);


	}
}
