using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System;

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
			//List<NehAlgorithm.CriticalPathElement> criticalPathElements = new();
			//criticalPathElements = nehAlgorithm.MakeCriticalPath(list,list[0].Count-1,list.Count-1,criticalPathElements);
			//ConsoleAllocator.ShowConsoleWindow();
			//foreach(NehAlgorithm.CriticalPathElement elem in criticalPathElements)
            //{ 
			//	Console.Write(elem.JobIndex.ToString() + " ");
			//	Console.Write(elem.JobTime.ToString() + "\n");
			//}
			vis.Show();
		}

        private void ModifiedNehButton_Click(object sender, RoutedEventArgs e)
        {
			NehAlgorithm nehAlgorithm = new NehAlgorithm();
			List<List<JobObject>> list = nehAlgorithm.RunModification1(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Neh");
			vis.Show();
		}
		private void ModifiedNeh2Button_Click(object sender, RoutedEventArgs e)
		{
			NehAlgorithm nehAlgorithm = new NehAlgorithm();
			List<List<JobObject>> list = nehAlgorithm.RunModification2(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Neh");
			vis.Show();
		}
		private void ModifiedNeh3Button_Click(object sender, RoutedEventArgs e)
		{
			NehAlgorithm nehAlgorithm = new NehAlgorithm();
			List<List<JobObject>> list = nehAlgorithm.RunModification3(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Neh");
			vis.Show();
		}
		private void ModifiedNeh4Button_Click(object sender, RoutedEventArgs e)
		{
			NehAlgorithm nehAlgorithm = new NehAlgorithm();
			List<List<JobObject>> list = nehAlgorithm.RunModification4(out Stopwatch stopwatch);
			Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "Neh");
			vis.Show();
		}
	}
}
