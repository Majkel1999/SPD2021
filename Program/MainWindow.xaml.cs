using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using SPD1.Algorithms;
using SPD1.Misc;

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
            testOfAlgorithms.RunTest2();
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

        private void Test2Button_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.RunTest2();
        }

        private void Test3Button_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.RunTest3();
        }

        private void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.ParseFile();
        }

        private void TabuSearchButton_Click(object sender, RoutedEventArgs e)
        {
            TSAlgorithm tabuSearch = new TSAlgorithm();
            List<List<JobObject>> list = tabuSearch.RunMod1(out Stopwatch stopwatch, 600, 150, 700);
            Visualization vis = new(list, stopwatch.Elapsed.TotalMilliseconds, "tabuSearch");
            vis.Show();
        }

        private void TabuModButton_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.RunTest4();
        }

        private void RPQSolve_Click(object sender, RoutedEventArgs e)
        {
            List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            RPQViewer view = new RPQViewer(Schrage.Solve(list, out int Cmax, out Stopwatch stopwatch));
            Trace.WriteLine("Cmax: " + Cmax);
            Trace.WriteLine("Elapsed ms: " + stopwatch.Elapsed.TotalMilliseconds);

            Schrage.SolveUsingQueue(list, out Cmax, out Stopwatch stopwatch1);
            Trace.WriteLine("Cmax queue: " + Cmax);
            Trace.WriteLine("Elapsed ms queue: " + stopwatch1.Elapsed.TotalMilliseconds);

            view.Show();
        }

        private void SchragePmtnButton_Click(object sender, RoutedEventArgs e)
        {
            List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            int Cmax = SchragePMTN.Solve(list, out Stopwatch stopwatch);
            Trace.WriteLine("Cmax: " + Cmax);
            Trace.WriteLine("Elapsed ms: " + stopwatch.Elapsed.TotalMilliseconds);

            Cmax = SchragePMTN.SolveUsingQueue(list, out Stopwatch stopwatch1);
            Trace.WriteLine("Cmax queue: " + Cmax);
            Trace.WriteLine("Elapsed ms queue: " + stopwatch1.Elapsed.TotalMilliseconds);
        }

        private void SchragTestButton_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.RunSchrageTest();
        }

        private void BasicCarlierSolveButton_Click(object sender, RoutedEventArgs e)
        {
            //carlier.ClearBeforeRun(); //używać tej funkcji szczególnie przy wywoływaniu kilku badań na tym samym obiekcie

            List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            Carlier carlier = new Carlier();
            carlier.Solve(list, out Stopwatch stopwatch);
            RPQViewer view = new RPQViewer(carlier.bestSolution);
            Trace.WriteLine("Cmax: " + carlier.Cmax);
            Trace.WriteLine("Elapsed ms: " + stopwatch.Elapsed.TotalMilliseconds);
            view.Show();
        }

        private void BasicCarlierSolveUsingQueueButton_Click(object sender, RoutedEventArgs e)
        {
            List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            Carlier carlier = new Carlier();
            carlier.SolveUsingQueue(list, out Stopwatch stopwatch);
            RPQViewer view = new RPQViewer(carlier.bestSolution);
            Trace.WriteLine("Cmax: " + carlier.Cmax);
            Trace.WriteLine("Elapsed ms: " + stopwatch.Elapsed.TotalMilliseconds);
            //Trace.WriteLine("recur: " + carlier.counter);
            view.Show();
        }

        private void TabuRPQ_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.RunTabuRPQTest();
        }

        private void GreedyCarlierButton_Click(object sender, RoutedEventArgs e)
        {
            List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            GreedyCarlier carlier = new GreedyCarlier();
            carlier.Solve(list, out Stopwatch stopwatch);
            RPQViewer view = new RPQViewer(carlier.bestSolution);
            Trace.WriteLine("Cmax: " + carlier.Cmax);
            Trace.WriteLine("Stopwatch: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
            view.Show();
        }

        private void DeepGreedyCarlierButton_Click(object sender, RoutedEventArgs e)
        {
            List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            GreedyCarlier carlier = new GreedyCarlier(isSearchingDeep: true);
            carlier.Solve(list, out Stopwatch stopwatch);
            RPQViewer view = new RPQViewer(carlier.bestSolution);
            Trace.WriteLine("Cmax: " + carlier.Cmax);
            Trace.WriteLine("Stopwatch: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
            view.Show();
        }

        private void CarlierTestButton_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.RunFullRPQTest();
        }

        private void ORToolsButton_Click(object sender, RoutedEventArgs e)
        {
            //List<RPQJob> list = RPQLoadData.LoadDataFromFile();
            List<JobshopJob> list = JobshopData.LoadDataFromFile();
            ORWrapper.Solve(list);
        }

        private void WitiWithORToolsButton_Click(object sender, RoutedEventArgs e)
        {
            List<WitiJob> list = LoadWitiData.LoadDataFromFile();
            WitiProblem.SolveWithORTools(list);

        }
    }
}
