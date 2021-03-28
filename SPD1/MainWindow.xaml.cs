using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace SPD1
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void JohnsonAlgButton_Click(object sender, RoutedEventArgs e)
		{
			JohnsonAlgorithm johnsonAlgorithm = new();
			List<List<JobObject>> list = johnsonAlgorithm.Run(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Johnson");
			vis.Show();
		}

		private void PermutationAlgButton_Click(object sender, RoutedEventArgs e)
		{
			PBAlgorithm testOfAlgorithm = new();
			List<List<JobObject>> list = testOfAlgorithm.Run(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Przeglad zupełny");
			vis.Show();
		}

		private void TestButton_Click(object sender, RoutedEventArgs e)
		{
			Test testOfAlgorithms = new();
			testOfAlgorithms.RunTest();
		}

        private void DefaultNehButton_Click(object sender, RoutedEventArgs e)
        {
			NehAlgorithm nehAlgorithm = new NehAlgorithm();
			List<List<JobObject>> list = nehAlgorithm.Run(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Neh");
			vis.Show();
		}

        private void ModifiedNehButton_Click(object sender, RoutedEventArgs e)
        {
			//Tu wstawić kod do zmodyfikowanego algorytmu NEH
        }
    }
}
