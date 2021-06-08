using Google.OrTools.Sat;
using System;
using System.Collections;
using SPD1.Misc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SPD1.Algorithms
{
	public class TaskType
	{
		public IntVar startVariable;
		public IntVar endVariable;
		public IntervalVar intervalVariable;
	}
	public class ORWrapper
	{
		public static void Solve(List<RPQJob> inputList)
		{
			CpModel model = new CpModel();
			int longestPreparationTime = 0;
			int longestDeliveryTime = 0;
			int workTimeSum = 0;
			workTimeSum += inputList.Sum(x => x.WorkTime);
			longestPreparationTime = inputList.Max(x => x.PreparationTime);
			longestDeliveryTime = inputList.Max(x => x.DeliveryTime);

			List<IntVar> inputStartTimes = new List<IntVar>();
			List<IntVar> inputEndTimes = new List<IntVar>();
			List<IntervalVar> modelIntervalVariables = new List<IntervalVar>();

			int maxValue = 1 + longestDeliveryTime + longestPreparationTime + workTimeSum;
			IntVar cmax = model.NewIntVar(0, maxValue, "CMax");

			foreach (RPQJob job in inputList)
			{
				var startVariable = model.NewIntVar(0, maxValue, "Job " + job.JobIndex + " start");
				var endVariable = model.NewIntVar(0, maxValue, "Job " + job.JobIndex + " end");
				modelIntervalVariables.Add(model.NewIntervalVar(startVariable, job.WorkTime, endVariable, "Interval on job " + job.JobIndex));
				inputStartTimes.Add(startVariable);
				inputEndTimes.Add(endVariable);

				model.Add(startVariable >= job.PreparationTime);
				model.Add(cmax >= endVariable + job.DeliveryTime);
			}
			model.AddNoOverlap(modelIntervalVariables);
			model.Minimize(cmax);
			var solver = new CpSolver();
			var status = solver.Solve(model);
			ConsoleAllocator.ShowConsoleWindow();
			Console.WriteLine(status.ToString());
			Console.WriteLine(solver.ObjectiveValue);
			List<Tuple<int, long>> jobOrder = new List<Tuple<int, long>>();
			for (int i = 0; i < inputList.Count; i++)
				jobOrder.Add(Tuple.Create(i, solver.Value(inputStartTimes[i])));
			jobOrder.Sort((Tuple<int, long> x, Tuple<int, long> y) =>
			{
				if (x.Item2 > y.Item2) return 1;
				if (x.Item2 < y.Item2) return -1;
				return 0;
			});
			foreach (var t in jobOrder)
				Console.Write(t.Item1 + " ");
		}

		public static void Solve(List<JobshopJob> inputList)
		{
			CpModel model = new CpModel();
			int machinesCount = inputList.Max(x => x.OperationsList.Max(x => x.MachineNumber));
			int durationsSum = inputList.Sum(x => x.OperationsList.Sum(x => x.Duration));
			List<List<TaskType>> allTasks = new List<List<TaskType>>();
			List<IntervalVar>[] machinesIntervals = new List<IntervalVar>[machinesCount];
			for (int i = 0; i < machinesCount; i++)
			{
				machinesIntervals[i] = new List<IntervalVar>();
			}

			int x = 0;
			foreach (JobshopJob job in inputList)
			{
				int y = 0;
				allTasks.Add(new List<TaskType>());
				foreach (JobshopOperation operation in job.OperationsList)
				{
					var startVar = model.NewIntVar(0, durationsSum, "start_" + x + "_" + y);
					var endVar = model.NewIntVar(0, durationsSum, "end_" + x + "_" + y);
					var intervalVar = model.NewIntervalVar(startVar, operation.Duration, endVar, "interval_" + x + "_" + y);
					allTasks[x].Add(new TaskType
					{
						startVariable = startVar,
						endVariable = endVar,
						intervalVariable = intervalVar
					});
					machinesIntervals[operation.MachineNumber - 1].Add(intervalVar);
					y++;
				}
				x++;
			}

			for (int i = 0; i < machinesCount; i++)
			{
				model.AddNoOverlap(machinesIntervals[i]);
			}

			x = 0;
			foreach (var job in inputList)
			{
				for (int y = 0; y < job.OperationsList.Count - 1; y++)
				{
					model.Add(allTasks[x][y + 1].startVariable >= allTasks[x][y].endVariable);
				}
				x++;
			}

			var cmax = model.NewIntVar(0, durationsSum, "Cmax");
			model.AddMaxEquality(cmax, allTasks.Select(x => x.Last().endVariable));
			model.Minimize(cmax);

			var solver = new CpSolver();
			var status = solver.Solve(model);

			ConsoleAllocator.ShowConsoleWindow();
			Console.WriteLine(status.ToString());
			Console.WriteLine(solver.ObjectiveValue);
		}

		public static void Solve(LoadData loadData)
		{
			CpModel model = new CpModel();
			int machinesCount = loadData.MachinesQuantity;
			int durationsSum = loadData.Jobs.Sum(x => x.Sum(x => x));
			List<List<TaskType>> allTasks = new List<List<TaskType>>();
			List<IntervalVar>[] machinesIntervals = new List<IntervalVar>[machinesCount];
			for (int i = 0; i < machinesCount; i++)
			{
				machinesIntervals[i] = new List<IntervalVar>();
			}

			int x = 0;
			foreach (var job in loadData.Jobs)
			{
				int y = 0;
				allTasks.Add(new List<TaskType>());

				for(int i = 0; i < job.Count; i++)
				{

					var startVar = model.NewIntVar(0, durationsSum, "start_" + x + "_" + y);
					var endVar = model.NewIntVar(0, durationsSum, "end_" + x + "_" + y);
					var intervalVar = model.NewIntervalVar(startVar, job[i], endVar, "interval_" + x + "_" + y);
					allTasks[x].Add(new TaskType
					{
						startVariable = startVar,
						endVariable = endVar,
						intervalVariable = intervalVar
					});
					machinesIntervals[i].Add(intervalVar);
					y++;
				}
				x++;
			}

			for (int i = 0; i < machinesCount; i++)
			{
				model.AddNoOverlap(machinesIntervals[i]);
			}

			x = 0;
			foreach (var job in loadData.Jobs)
			{
				for (int y = 0; y < job.Count - 1; y++)
				{
					model.Add(allTasks[x][y + 1].startVariable >= allTasks[x][y].endVariable);
				}
				x++;
			}

			var cmax = model.NewIntVar(0, durationsSum, "Cmax");
			model.AddMaxEquality(cmax, allTasks.Select(x => x.Last().endVariable));
			model.Minimize(cmax);

			var solver = new CpSolver();
			var status = solver.Solve(model);

			ConsoleAllocator.ShowConsoleWindow();
			Console.WriteLine(status.ToString());
			Console.WriteLine(solver.ObjectiveValue);
		}
	}
}