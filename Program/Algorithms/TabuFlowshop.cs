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

		public List<List<Step>> GenerateNeighbourhood(List<int> permutation, List<Step> modifers, int neighbourhoodSize)
		{
			List<List<Step>> neighbourhood = new List<List<Step>>();
			List<Step> alreadyDoneSteps = modifers.ToList();

			int maxPermutations;

			if (permutation.Count > 8)
				maxPermutations = neighbourhoodSize;
			else
				maxPermutations = Enumerable.Range(1, permutation.Count - 1).Aggregate(1, (p, item) => p * item);
			int counter = 0;

			while (neighbourhood.Count < neighbourhoodSize && neighbourhood.Count < maxPermutations)
			{
				for (int i = 0; i < permutation.Count; i++)
				{
					for (int j = i + 1; j < permutation.Count; j++)
					{
						if (neighbourhood.Count < neighbourhoodSize && neighbourhood.Count < maxPermutations)
						{
							List<Step> steps = new List<Step>();

							foreach (var step in alreadyDoneSteps)
								steps.Add(step);

							steps.Add(new Step(i, j));

							neighbourhood.Add(steps);

							counter++;
						}
						else
							return neighbourhood;
					}
				}
				var rand = new Random();
				alreadyDoneSteps.Clear();
				alreadyDoneSteps.AddRange(neighbourhood[rand.Next(counter)]);
			}

			return null;
		}

		public List<Step> CalculateBestSolution(List<int> startPermutation, List<List<Step>> neighbourhood, List<List<Step>> tabuList, LoadData data, out int Cmax)
		{
			int cmaxOfBestSolution = int.MaxValue;
			List<Step> bestSolution = new List<Step>();

			bool isInTabuList;


			foreach (var permutation in neighbourhood)
			{
				List<int> temporaryPermutation = startPermutation;
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
					{
						temporaryPermutation = SwapElements(temporaryPermutation, step.First, step.Second);
					}

					int temp = Gantt.GetCmax(temporaryPermutation, data);
					if (cmaxOfBestSolution > temp)
					{
						cmaxOfBestSolution = temp;
						bestSolution = permutation;
					}
				}
			}
			Cmax = cmaxOfBestSolution;
			return bestSolution;
		}

		public List<List<JobObject>> Run(out Stopwatch stopwatch, int sizeOfTabuList, int countOfIterations, int countOfPermutations, LoadData data = null)
		{
			if (data == null)
				throw new Exception("Input data is null");

			stopwatch = new Stopwatch();
			List<List<Step>> tabuList = new List<List<Step>>();
			List<Step> bestSolutionOfAlgorithm = new List<Step>();
			int bestSolutionCmax;


			stopwatch.Start();

			List<int> startPermutation = GenerateStartPermutation(data);
			bestSolutionCmax = Gantt.GetCmax(startPermutation, data);

			tabuList.Add(bestSolutionOfAlgorithm);

			int counter = countOfIterations;
			while (counter > 0)
			{
				List<List<Step>> neighbourhood = GenerateNeighbourhood(startPermutation, bestSolutionOfAlgorithm, countOfPermutations);

				List<Step> solution = CalculateBestSolution(startPermutation, neighbourhood, tabuList, data, out int cmax);

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
			}

			stopwatch.Stop();

			List<int> finalList = startPermutation;

			foreach (var step in bestSolutionOfAlgorithm)
				finalList = SwapElements(finalList, step.First, step.Second);

			return Gantt.MakeGanttChart(finalList, data);
		}
	}
}
