using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SPD1
{
    class TSAlgorithm
    {
        public static List<int> Swap(List<int> list, int A, int B)
        {
            List<int> temp = new List<int>(list);
            int tmp = temp[A];
            temp[A] = temp[B];
            temp[B] = tmp;
            return temp;
        }

        public static List<int> Insert(List<int> list, int a, int b)
        {
            var tmp = list.ToList();
            var tmp2 = tmp[a];
            tmp.RemoveAt(a);
            tmp.Insert(b, tmp2);
            return tmp;
        }

        public static List<int> Random(List<int> list, Random random)
        {
            List<int> temp = new List<int>(list);
            temp = temp.OrderBy(x => random.Next()).ToList();
            return temp;
        }

        private long Factorial(int number)
        {
            long factorial = 1;
            if (number == 0)
            {
                return factorial;
            }
            for (int i = 1; i <= number; i++)
            {
                factorial *= i;
            }
            return factorial;
        }
        public List<int> GenerateStartPermutation(LoadData data)
        {
            NehAlgorithm nehAlgorithm = new NehAlgorithm();
            List<int> nehResult = nehAlgorithm.RunWithoutGantt(out Stopwatch stopwatch2, data);
            return nehResult;
        }

        public List<int> GenerateStartPermutationMod2(LoadData data)
        {
            var dataCopy = new LoadData(data);
            JohnsonAlgorithm johnson = new JohnsonAlgorithm();
            var nehResult = johnson.RunToTest(out Stopwatch stopwatch, dataCopy);
            List<int> result = new List<int>();
            result.AddRange(nehResult.FirstOrDefault().Select(i => i.JobIndex));
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = result[i] - 1;
            }
            return result;
        }

        public List<List<int>> GenerateNeighbourhood(List<int> startPermutation, int howManyNGBH)
        {
            List<List<int>> neighbourhood = new List<List<int>>();
            List<int> tempPerm = startPermutation;
            int counter = 0;
            while (neighbourhood.Count < howManyNGBH)
            {
                for (int i = 0; i < startPermutation.Count; i++)
                {
                    for (int j = i + 1; j < startPermutation.Count; j++)
                    {
                        if (neighbourhood.Count < howManyNGBH)
                        {
                            neighbourhood.Add(Swap(tempPerm, i, j));
                        }
                        else
                        {
                            return neighbourhood;
                        }
                    }
                }
                tempPerm = neighbourhood[counter];
                ++counter;
            }
            return null;
        }

        public List<List<int>> GenerateNeighbourhoodMod3(List<int> startPermutation, int howManyNGBH)
        {
            List<List<int>> neighbourhood = new List<List<int>>();
            List<int> tempPerm = startPermutation;
            int counter = 0;
            while (neighbourhood.Count < howManyNGBH)
            {
                for (int i = 0; i < startPermutation.Count; i++)
                {
                    for (int j = i + 1; j < startPermutation.Count; j++)
                    {
                        if (neighbourhood.Count < howManyNGBH)
                        {
                            neighbourhood.Add(Insert(tempPerm, i, j));
                        }
                        else
                        {
                            return neighbourhood;
                        }
                    }
                }
                tempPerm = neighbourhood[counter];
                ++counter;
            }
            return null;
        }

        public List<List<int>> GenerateNeighbourhoodRandom(List<int> startPermutation, int from, int to)
        {
            Random random = new Random();
            int neighbourhoodSize = random.Next(from, to);
            List<List<int>> neighbourhood = new List<List<int>>();
            List<int> tempPerm = startPermutation;
            int counter = 0;
            while (neighbourhood.Count < neighbourhoodSize)
            {
                neighbourhood.Add(Random(tempPerm, random));
                ++counter;
            }
            return neighbourhood;
        }

        public List<int> CalculateBestSolution(List<List<int>> neighbourhood, List<List<int>> tabuList, LoadData data, out int Cmax)
        {
            int cmaxOfBestSolution = int.MaxValue;
            List<int> bestSolution = new List<int>();
            int temp;
            bool isInTabuList;
            foreach (var x in neighbourhood)
            {
                isInTabuList = false;

                foreach (var y in tabuList)
                {
                    if (y.SequenceEqual(x))
                    {
                        isInTabuList = true;
                        break;
                    }
                }
                if (!isInTabuList)
                {
                    temp = Gantt.GetCmax(x, data);
                    if (cmaxOfBestSolution > temp)
                    {
                        cmaxOfBestSolution = temp;
                        bestSolution = x;
                    }
                }
            }
            Cmax = cmaxOfBestSolution;
            return bestSolution;
        }

        public List<List<JobObject>> Run(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int countOfPermutations, LoadData data = null)
        {
            stopwatch = new Stopwatch();
            List<List<int>> tabuList = new List<List<int>>();// maksymalny rozmiar: sizeOfTabuList
            List<int> bestSolutionOfAlgorithm = new List<int>();
            int bestSolutionCmax;

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

            stopwatch.Start();

            //wygenerowanie początkowej kolejności zadań
            List<int> permutation = GenerateStartPermutation(data);
            bestSolutionCmax = Gantt.GetCmax(permutation, data);
            bestSolutionOfAlgorithm = permutation;
            tabuList.Add(bestSolutionOfAlgorithm);
            if (data.JobsQuantity >= 7)
            {
                int counter = countOfIterations;
                while (counter > 0) //warunek stopu jako ilość iteracji
                {
                    //wygeneruj sąsiedztwo
                    List<List<int>> neighbourhood = GenerateNeighbourhood(permutation, countOfPermutations);
                    //wyznacz najlepsze rozwiazanie
                    List<int> solution = CalculateBestSolution(neighbourhood, tabuList, data, out int cmax);
                    //popraw listę tabu
                    if (tabuList.Count == sizeOfTabuList)
                    {
                        tabuList.RemoveAt(0);
                        tabuList.Add(solution);
                    }
                    else
                    {
                        tabuList.Add(solution);
                    }

                    if (bestSolutionCmax > cmax)
                    {
                        bestSolutionOfAlgorithm = solution;
                        bestSolutionCmax = cmax;
                    }
                    --counter;
                    permutation = solution;
                }
            }
            stopwatch.Stop();

            return Gantt.MakeGanttChart(bestSolutionOfAlgorithm, data);
        }

        public List<List<JobObject>> RunMod1(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int countOfPermutations, LoadData data = null)
        {
            stopwatch = new Stopwatch();
            List<List<int>> tabuList = new List<List<int>>();// maksymalny rozmiar: sizeOfTabuList
            List<int> bestSolutionOfAlgorithm = new List<int>();
            int bestSolutionCmax;

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

            stopwatch.Start();

            //wygenerowanie początkowej kolejności zadań
            List<int> permutation = GenerateStartPermutation(data);
            bestSolutionCmax = Gantt.GetCmax(permutation, data);
            bestSolutionOfAlgorithm = permutation;
            tabuList.Add(bestSolutionOfAlgorithm);
            if (data.JobsQuantity >= 7)
            {
                int counter = countOfIterations;
                while (counter > 0) //warunek stopu jako ilość iteracji bez poprawy CMAX
                {
                    //wygeneruj sąsiedztwo
                    List<List<int>> neighbourhood = GenerateNeighbourhood(permutation, countOfPermutations);
                    //wyznacz najlepsze rozwiazanie
                    List<int> solution = CalculateBestSolution(neighbourhood, tabuList, data, out int cmax);
                    //popraw listę tabu
                    if (tabuList.Count == sizeOfTabuList)
                    {
                        tabuList.RemoveAt(0);
                        tabuList.Add(solution);
                    }
                    else
                    {
                        tabuList.Add(solution);
                    }

                    if (bestSolutionCmax > cmax)
                    {
                        bestSolutionOfAlgorithm = solution;
                        bestSolutionCmax = cmax;
                        counter = countOfIterations;
                    }
                    else
                    {
                        --counter;
                    }
                    permutation = solution;
                }
            }
            stopwatch.Stop();

            return Gantt.MakeGanttChart(bestSolutionOfAlgorithm, data);
        }

        public List<List<JobObject>> RunMod2(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int countOfPermutations, LoadData data = null)
        {
            stopwatch = new Stopwatch();
            List<List<int>> tabuList = new List<List<int>>();// maksymalny rozmiar: sizeOfTabuList
            List<int> bestSolutionOfAlgorithm = new List<int>();
            int bestSolutionCmax;

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

            stopwatch.Start();

            //wygenerowanie początkowej kolejności zadań
            List<int> permutation = GenerateStartPermutationMod2(data); //Generowanie johnsonem
            bestSolutionCmax = Gantt.GetCmax(permutation, data);
            bestSolutionOfAlgorithm = permutation;
            tabuList.Add(bestSolutionOfAlgorithm);
            if (data.JobsQuantity >= 7)
            {
                int counter = countOfIterations;
                while (counter > 0) //warunek stopu jako ilość iteracji
                {
                    //wygeneruj sąsiedztwo
                    List<List<int>> neighbourhood = GenerateNeighbourhood(permutation, countOfPermutations);
                    //wyznacz najlepsze rozwiazanie
                    List<int> solution = CalculateBestSolution(neighbourhood, tabuList, data, out int cmax);
                    //popraw listę tabu
                    if (tabuList.Count == sizeOfTabuList)
                    {
                        tabuList.RemoveAt(0);
                        tabuList.Add(solution);
                    }
                    else
                    {
                        tabuList.Add(solution);
                    }

                    if (bestSolutionCmax > cmax)
                    {
                        bestSolutionOfAlgorithm = solution;
                        bestSolutionCmax = cmax;
                    }
                    --counter;
                    permutation = solution;
                }
            }
            stopwatch.Stop();

            return Gantt.MakeGanttChart(bestSolutionOfAlgorithm, data);
        }

        public List<List<JobObject>> RunMod3(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int countOfPermutations, LoadData data = null)
        {
            stopwatch = new Stopwatch();
            List<List<int>> tabuList = new List<List<int>>();// maksymalny rozmiar: sizeOfTabuList
            List<int> bestSolutionOfAlgorithm = new List<int>();
            int bestSolutionCmax;

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

            stopwatch.Start();

            //wygenerowanie początkowej kolejności zadań
            List<int> permutation = GenerateStartPermutation(data);
            bestSolutionCmax = Gantt.GetCmax(permutation, data);
            bestSolutionOfAlgorithm = permutation;
            tabuList.Add(bestSolutionOfAlgorithm);
            if (data.JobsQuantity >= 7)
            {
                int counter = countOfIterations;
                while (counter > 0) //warunek stopu jako ilość iteracji
                {
                    //wygeneruj sąsiedztwo
                    List<List<int>> neighbourhood = GenerateNeighbourhoodMod3(permutation, countOfPermutations);
                    //wyznacz najlepsze rozwiazanie
                    List<int> solution = CalculateBestSolution(neighbourhood, tabuList, data, out int cmax);
                    //popraw listę tabu
                    if (tabuList.Count == sizeOfTabuList)
                    {
                        tabuList.RemoveAt(0);
                        tabuList.Add(solution);
                    }
                    else
                    {
                        tabuList.Add(solution);
                    }

                    if (bestSolutionCmax > cmax)
                    {
                        bestSolutionOfAlgorithm = solution;
                        bestSolutionCmax = cmax;
                    }
                    --counter;
                    permutation = solution;
                }
            }
            stopwatch.Stop();

            return Gantt.MakeGanttChart(bestSolutionOfAlgorithm, data);
        }

        public List<List<JobObject>> RunMod4(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int countOfPermutations, LoadData data = null)
        {
            stopwatch = new Stopwatch();
            List<List<int>> tabuList = new List<List<int>>();// maksymalny rozmiar: sizeOfTabuList
            List<int> bestSolutionOfAlgorithm = new List<int>();
            int bestSolutionCmax;

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

            stopwatch.Start();

            //wygenerowanie początkowej kolejności zadań
            List<int> permutation = GenerateStartPermutation(data);
            bestSolutionCmax = Gantt.GetCmax(permutation, data);
            bestSolutionOfAlgorithm = permutation;
            tabuList.Add(bestSolutionOfAlgorithm);
            if (data.JobsQuantity >= 7)
            {
                int counter = countOfIterations;
                while (counter > 0) //warunek stopu jako ilość iteracji
                {
                    //wygeneruj sąsiedztwo
                    List<List<int>> neighbourhood = GenerateNeighbourhoodRandom(permutation, 100, countOfPermutations);
                    //wyznacz najlepsze rozwiazanie
                    List<int> solution = CalculateBestSolution(neighbourhood, tabuList, data, out int cmax);
                    //popraw listę tabu
                    if (tabuList.Count == sizeOfTabuList)
                    {
                        tabuList.RemoveAt(0);
                        tabuList.Add(solution);
                    }
                    else
                    {
                        tabuList.Add(solution);
                    }

                    if (bestSolutionCmax > cmax)
                    {
                        bestSolutionOfAlgorithm = solution;
                        bestSolutionCmax = cmax;
                    }
                    --counter;
                    permutation = solution;
                }
            }
            stopwatch.Stop();

            return Gantt.MakeGanttChart(bestSolutionOfAlgorithm, data);
        }
    }
}
