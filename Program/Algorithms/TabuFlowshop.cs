using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SPD1.Misc;
using SPD1.Algorithms;

namespace SPD1.Algorithms
{
	class TabuFlowshop
	{
		public struct Step
		{
			public int First;
			public int Second;

			public Step(int first, int second)
			{
				First = first;
				Second = second;
			}
		}

		public static List<int> SwapElements(List<int> list, int A, int B)
		{
			List<int> temp = new List<int>(list);
			int tmp = temp[A];
			temp[A] = temp[B];
			temp[B] = tmp;
			return temp;
		}

		public List<int> GenerateStartPermutation(LoadData data)
		{
			NehAlgorithm nehAlgorithm = new NehAlgorithm();
			List<int> nehResult = nehAlgorithm.RunWithoutGantt(out Stopwatch stopwatch2, data);
			return nehResult;
		}

		public List<List<Step>> GenerateNeighbourhood(List<int> startPermutation, List<Step> startPermutationModifers, int neighbourhoodSize)
		{
			List<List<Step>> neighbourhood = new List<List<Step>>();
			List<Step> alreadyDoneSteps = startPermutationModifers.ToList();

			int maxPermutations;

			if (startPermutation.Count > 8) //Większe od 8 ponieważ 8! daje liczbę dużo większą niż nasze docelowe rozmiary sąsiedztwa
				maxPermutations = neighbourhoodSize;
			else
				maxPermutations = Enumerable.Range(1, startPermutation.Count - 1).Aggregate(1, (p, item) => p * item); //Ekwiwalent silni

			int counter = 0;

			while (neighbourhood.Count < neighbourhoodSize && neighbourhood.Count < maxPermutations)
			{
				for (int i = 0; i < startPermutation.Count; i++)
				{
					for (int j = i + 1; j < startPermutation.Count; j++)
					{
						if (neighbourhood.Count < neighbourhoodSize && neighbourhood.Count < maxPermutations)
						{
							List<Step> steps = new List<Step>();
							steps.AddRange(alreadyDoneSteps);

							steps.Add(new Step(i, j));

							neighbourhood.Add(steps);

							counter++;
						}
						else
							return neighbourhood;
					}
				}
				var random = new Random();
				alreadyDoneSteps.Clear();
				alreadyDoneSteps.AddRange(neighbourhood[random.Next(counter)]); //Wybieramy losowo, żeby nie powtarzać tego samego ruchu
			}
			return neighbourhood;
		}

		public List<Step> CalculateBestSolution(List<int> startPermutation, List<List<Step>> neighbourhood, List<List<Step>> tabuList, LoadData data, out int Cmax)
		{
			int cmaxOfBestSolution = int.MaxValue;
			List<Step> bestSolution = new List<Step>();

			bool isInTabuList;

			foreach (var permutation in neighbourhood)
			{
				List<int> temporaryPermutation = startPermutation.ToList();
				isInTabuList = false;

				foreach (var y in tabuList)
				{
					if (y.SequenceEqual(permutation))
					{
						isInTabuList = true;
						break;
					}
				}

				if (!isInTabuList)
				{
					foreach (Step step in permutation)
						temporaryPermutation = SwapElements(temporaryPermutation, step.First, step.Second);

					int tempCmax = Gantt.GetCmax(temporaryPermutation, data);

					if (tempCmax < cmaxOfBestSolution)
					{
						cmaxOfBestSolution = tempCmax;
						bestSolution = permutation;
					}
				}
			}

			Cmax = cmaxOfBestSolution;
			return bestSolution;
		}

		public List<List<JobObject>> Run(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int neighbourhoodSize, LoadData flowshopData = null)
		{
			if (flowshopData == null)
				throw new Exception("Input data is null");

			stopwatch = new Stopwatch();
			stopwatch.Start();

			List<int> startPermutation = GenerateStartPermutation(flowshopData);
			int bestSolutionCmax = Gantt.GetCmax(startPermutation, flowshopData);

			List<List<Step>> tabuList = new List<List<Step>>();
			List<Step> bestSolutionOfAlgorithm = new List<Step>();

			tabuList.Add(bestSolutionOfAlgorithm);

			int counter = countOfIterations;

			while (counter > 0)
			{
				List<List<Step>> neighbourhood = GenerateNeighbourhood(startPermutation, bestSolutionOfAlgorithm, neighbourhoodSize);

				List<Step> solution = CalculateBestSolution(startPermutation, neighbourhood, tabuList, flowshopData, out int Cmax);

				if (tabuList.Count == sizeOfTabuList)
				{
					tabuList.RemoveAt(0);
					tabuList.Add(solution);
				}
				else
					tabuList.Add(solution);

				if (Cmax < bestSolutionCmax)
				{
					bestSolutionOfAlgorithm = solution;
					bestSolutionCmax = Cmax;
				}
				--counter;
			}

			List<int> outputList = startPermutation;

			foreach (var step in bestSolutionOfAlgorithm)
				outputList = SwapElements(outputList, step.First, step.Second);

			stopwatch.Stop();

			return Gantt.MakeGanttChart(outputList, flowshopData);
		}
	}
}
