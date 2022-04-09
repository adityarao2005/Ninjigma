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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ninjigma
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	// The hard game grid
	public sealed partial class HardGrid : GridGame
	{
		// Constructor for the grid
		public HardGrid() : base()
		{
		}

		// Gives base code xaml handle to grid
		public override Grid GameGrid()
		{
			return gameGrid;
		}

		// Calls initializecomponent
		public override void Initialize()
		{
			this.InitializeComponent();
		}

		// returns the amount of times that a piece is moved
		public override int CYCLES()
		{
			return 1000;
		}
	}
}
