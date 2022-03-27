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
	// The easy game grid. 3x3 touch grid
	public sealed partial class EasyGrid : GridGame
	{
		// invoke base constructor
		public EasyGrid() : base()
		{
		}

		// return grid from xaml
		public override Grid GameGrid()
		{
			return gameGrid;
		}

		// Initialize component in base constructor
		public override void Initialize()
		{
			this.InitializeComponent();
		}

		// return easy number of cycles
		public override int CYCLES()
		{
			return 100;
		}
	}
}
