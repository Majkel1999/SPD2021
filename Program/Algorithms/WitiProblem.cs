using System;
using System.Collections.Generic;
using System.Diagnostics;
using Google.OrTools.Sat;
using SPD1.Misc;

namespace SPD1.Algorithms
{

    public class WitiProblem
    {
        public static void SolveWithORTools(List<WitiJob> witiData)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CpModel model = new CpModel();

            int sumOfAllWorkTimes = 0; //do czasów rozpoczęścia i zakończenia zadań, jako ograniczenie górne 
            foreach (WitiJob job in witiData)
            {
                sumOfAllWorkTimes += job.workTime;
            }
            int sumOfAllLateTimes = 0;
            foreach (WitiJob job in witiData)
            {
                sumOfAllLateTimes += job.weight * job.desiredEndTime;
            }

            IntVar wiTiOptimalizationObjective = model.NewIntVar(0, sumOfAllLateTimes, "WiTi optimalization objective");

            List<IntVar> inputStartTimes = new List<IntVar>();
            List<IntVar> lateTimes = new List<IntVar>();
            List<IntVar> inputEndTimes = new List<IntVar>();
            List<IntervalVar> modelIntervalVariables = new List<IntervalVar>();
            for (int i = 0; i < witiData.Count; i++)
            {
                WitiJob job = witiData[i];
                var startVariable = model.NewIntVar(0, sumOfAllWorkTimes, "Job " + i + " start");
                var endVariable = model.NewIntVar(0, sumOfAllWorkTimes, "Job " + i + " end");
                var lateVariable = model.NewIntVar(0, sumOfAllLateTimes, "Job " + i + " late");

                modelIntervalVariables.Add(model.NewIntervalVar(startVariable, job.workTime, endVariable, "Interval on job " + i));
                inputStartTimes.Add(startVariable);
                inputEndTimes.Add(endVariable);
                lateTimes.Add(lateVariable);

                model.Add(startVariable >= 0);
                model.Add(endVariable > 0);
                model.Add(lateVariable >= 0);
                model.Add(lateVariable >= (endVariable - job.desiredEndTime) * job.weight);
            }

            model.AddNoOverlap(modelIntervalVariables);
            model.Add(LinearExpr.Sum(lateTimes) <= wiTiOptimalizationObjective);
            model.Minimize(wiTiOptimalizationObjective);
            var solver = new CpSolver();
            var status = solver.Solve(model);
            ConsoleAllocator.ShowConsoleWindow();
            Console.WriteLine(status.ToString());
            Console.WriteLine(solver.ObjectiveValue);
            List<Tuple<int, long>> jobOrder = new List<Tuple<int, long>>();
            for (int i = 0; i < witiData.Count; i++)
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
            Console.WriteLine("\nElapsed time: "+stopwatch.Elapsed.TotalMilliseconds + "ms");
        }
    }
}
