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
	// This is the medium difficulty grid
	public sealed partial class MediumGrid : GridGame
	{
		// inherit abstract constructor
		public MediumGrid() : base()
		{
		}

		// Make sure we use the grid from xaml
		public override Grid GameGrid()
		{
			return gameGrid;
		}

		// Make sure we initialize this before it starts
		public override void Initialize()
		{
			this.InitializeComponent();
		}

		// Make sure this cycles 500 for difficulty
		public override int CYCLES()
		{
			return 500;
		}
	}
}
