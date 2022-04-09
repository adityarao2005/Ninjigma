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
	// This is a grid that only has 1 component that is uniformly the same through its rows and columns
	public sealed partial class UniformGrid : UserControl
	{
		// The constructor of the uniform grid
		public UniformGrid()
		{
			this.InitializeComponent();
		}

		// The rows property
		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new PropertyMetadata(null, GridChanged));

		public int Rows
		{
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}

		// the columns property
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new PropertyMetadata(null, GridChanged));

		public int Columns
		{
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		// When the grid is changed, remove all row and column constraints and repopulate the grid accordingly
		private static void GridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// Get the grid
			UniformGrid uniformGrid = d as UniformGrid;
			// Clear row and column constraints
			uniformGrid.grid.RowDefinitions.Clear();
			uniformGrid.grid.ColumnDefinitions.Clear();

			// Re add them
			for (int row = 0; row < uniformGrid.Rows; row++)
			{
				uniformGrid.grid.RowDefinitions.Add(new RowDefinition());
			}

			for (int column = 0; column < uniformGrid.Columns; column++)
			{
				uniformGrid.grid.ColumnDefinitions.Add(new ColumnDefinition());
			}

			// Add the border outline for each row and column
			for (int row = 0; row < uniformGrid.Rows; row++)
			{
				for (int column = 0; column < uniformGrid.Columns; column++)
				{
					// Create a border with the same background and same borderbrush. Make sure they are bound
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

					// Set the default thickness to 2
					border.BorderThickness = new Thickness(2);

					// Set the row and column of the border
					Grid.SetColumn(border, column);
					Grid.SetRow(border, row);

					// Add the border to the children
					uniformGrid.grid.Children.Add(border);
				}
			}
		}
	}
}
