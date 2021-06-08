using Google.OrTools.Sat;
using System;
using System.Collections;
using SPD1.Misc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SPD1.Algorithms
{
    public class ORWrapper
    {
        public static void solve(List<RPQJob> inputList)
        {
            CpModel model = new CpModel();
            int longestPreparationTime = 0;
            int longestDeliveryTime = 0;
            int workTimeSum = 0;
            //ToDo przerobić na LINQ -> bedzie szybciej
            foreach (RPQJob job in inputList)
            {
                workTimeSum += job.WorkTime;
                longestPreparationTime = Math.Max(longestPreparationTime, job.PreparationTime);
                longestDeliveryTime = Math.Max(longestDeliveryTime, job.DeliveryTime);
            }

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
            jobOrder.Sort((Tuple<int,long> x, Tuple<int,long> y) => {
                if(x.Item2 > y.Item2) return 1;
                if(x.Item2 < y.Item2) return -1;
                return 0;
            });
            foreach(var t in jobOrder)
                Console.Write(t.Item1 +" ");
        }
    }
}