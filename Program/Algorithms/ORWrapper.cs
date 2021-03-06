using Google.OrTools.Sat;
using System;
using System.Collections;
using SPD1.Misc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SPD1.Algorithms
{

    public class SolvedData
    {
        public long startTime;
        public int jobNumber;
        public int operationNumber;
        public int duration;
    }
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            solver.StringParameters = "max_time_in_seconds:120.0";
            var status = solver.Solve(model);
            ConsoleAllocator.ShowConsoleWindow();
            Console.WriteLine(status.ToString());
            Console.WriteLine("Cmax: " + solver.ObjectiveValue);
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
            stopwatch.Stop();
            Console.WriteLine("\nElapsed time: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
        }

        public static void Solve(List<JobshopJob> inputList)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            solver.StringParameters = "max_time_in_seconds:120.0";
            var status = solver.Solve(model);

            ConsoleAllocator.ShowConsoleWindow();
            Console.WriteLine("\n" + status.ToString());
            Console.WriteLine("Cmax: " + solver.ObjectiveValue);


            Dictionary<int, List<SolvedData>> machinesSolve = new Dictionary<int, List<SolvedData>>();
            for (int i = 0; i < inputList.Count; i++)
            {
                for (int j = 0; j < inputList[i].OperationsCount; j++)
                {
                    int machineNumber = inputList[i].OperationsList[j].MachineNumber;
                    if (!machinesSolve.ContainsKey(machineNumber))
                    {
                        machinesSolve.Add(machineNumber, new List<SolvedData>());
                    }
                    machinesSolve[machineNumber].Add(new SolvedData
                    {
                        startTime = solver.Value(allTasks[i][j].startVariable),
                        jobNumber = i,
                        operationNumber = j,
                        duration = inputList[i].OperationsList[j].Duration
                    });
                }
            }
            /*
            for (int i = 0; i < machinesCount; i++)
            {
                machinesSolve[i + 1].Sort((x1, x2) =>
                  {
                      if (x1.startTime > x2.startTime) return 1;
                      if (x1.startTime < x2.startTime) return -1;
                      return 0;
                  });
                Console.Write("Machine " + i + " ");
                foreach (SolvedData data in machinesSolve[i + 1])
                {
                    Console.Write("[(" + (data.jobNumber * inputList[0].OperationsCount + data.operationNumber + 1) + "), dur:(" + data.startTime + "=>" + (data.startTime + data.duration) + ")] ");
                }
                Console.WriteLine();
            }
            */
            stopwatch.Stop();
            Console.WriteLine("\nElapsed time: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
        }

        public static void Solve(LoadData loadData)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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

                for (int i = 0; i < job.Count; i++)
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
            for(int i=1;i<allTasks.Count;i++){
                for(int j=0;j<allTasks[i].Count-1;j++){
                    model.Add(allTasks[i][j].endVariable <= allTasks[i][j+1].startVariable);
                }
            }

            for(int i=0;i<loadData.JobsQuantity-1;i++){
                for(int j=i+1;j<loadData.JobsQuantity;j++){
                    var boolean = model.NewBoolVar("bool_"+i+"_"+j);
                    model.Add(allTasks[i][0].startVariable > allTasks[j][0].startVariable).OnlyEnforceIf(boolean);
                    model.Add(allTasks[i][0].startVariable < allTasks[j][0].startVariable).OnlyEnforceIf(boolean.Not());
                    for(int k=1;k<machinesCount;k++){
                        model.Add(allTasks[i][k].startVariable > allTasks[j][k].startVariable).OnlyEnforceIf(boolean);
                        model.Add(allTasks[i][k].startVariable < allTasks[j][k].startVariable).OnlyEnforceIf(boolean.Not());
                    }
                }
            }


            var cmax = model.NewIntVar(0, durationsSum, "Cmax");
            model.AddMaxEquality(cmax, allTasks.Select(x => x.Last().endVariable));
            model.Minimize(cmax);

            var solver = new CpSolver();
            solver.StringParameters = "max_time_in_seconds:120.0";
            var status = solver.Solve(model);

            ConsoleAllocator.ShowConsoleWindow();
            Console.WriteLine(status.ToString());
            Console.WriteLine("Cmax: " + solver.ObjectiveValue);

            List<List<SolvedData>> solution = new List<List<SolvedData>>();
            for (int i = 0; i < loadData.MachinesQuantity; i++)
            {
                solution.Add(new List<SolvedData>());
                for (int j = 0; j < loadData.JobsQuantity; j++)
                {
                    solution[i].Add(new SolvedData
                    {
                        duration = loadData.Jobs[j][i],
                        jobNumber = j,
                        startTime = solver.Value(allTasks[j][i].startVariable)
                    });
                }
            }

            foreach (var data in solution)
            {
                data.Sort((x1, x2) =>
                    {
                        if (x1.startTime > x2.startTime) return 1;
                        if (x1.startTime < x2.startTime) return -1;
                        return 0;
                    });
                foreach (var job in data)
                {
                    Console.Write("[job:" + job.jobNumber + ", dur:" + job.startTime + "=>" + (job.duration + job.startTime) + "] ");
                }
                Console.WriteLine("\n");
            }

            stopwatch.Stop();
            Console.WriteLine("\nElapsed time: " + stopwatch.Elapsed.TotalMilliseconds + "ms");

        }
    }
}