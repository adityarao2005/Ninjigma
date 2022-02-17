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
	public sealed partial class UniformGrid : UserControl
	{
		public UniformGrid()
		{
			this.InitializeComponent();
			
		}

		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new PropertyMetadata(null, GridChanged));

		public int Rows
		{
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}

		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new PropertyMetadata(null, GridChanged));


		public int Columns
		{
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		private static void GridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			
			UniformGrid uniformGrid = d as UniformGrid;
			uniformGrid.grid.RowDefinitions.Clear();
			uniformGrid.grid.ColumnDefinitions.Clear();

			for (int row = 0; row < uniformGrid.Rows; row++)
			{
				uniformGrid.grid.RowDefinitions.Add(new RowDefinition());
			}

			for (int column = 0; column < uniformGrid.Columns; column++)
			{
				uniformGrid.grid.ColumnDefinitions.Add(new ColumnDefinition());
			}


			for (int row = 0; row < uniformGrid.Rows; row++)
			{
				for (int column = 0; column < uniformGrid.Columns; column++)
				{
					Border border = new Border();
					{
						Binding binding = new Binding();
						binding.Source = uniformGrid;
						binding.Path = new PropertyPath("Background");
						BindingOperations.SetBinding(border, Border.BackgroundProperty, binding);
					}
					{
						Binding binding = new Binding();
						binding.Source = uniformGrid;
						binding.Path = new PropertyPath("BorderBrush");
						BindingOperations.SetBinding(border, Border.BorderBrushProperty, binding);
					}

					border.BorderThickness = new Thickness(2);

					Grid.SetColumn(border, column);
					Grid.SetRow(border, row);

					uniformGrid.grid.Children.Add(border);
				}
			}
		}
	}
}
