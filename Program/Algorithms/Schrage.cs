using System.Collections.Generic;
using System.Linq;
using System;
using SPD1.Misc;
using System.Diagnostics;

namespace SPD1.Algorithms
{
    public class Schrage
    {
        public static List<RPQJob> Solve(List<RPQJob> inputData, out int Cmax, out Stopwatch stopwatch)
        {
            List<RPQJob> jobs = inputData.ToList();
            stopwatch = new Stopwatch();
            Cmax = 0;
            List<RPQJob> readyJobs = new List<RPQJob>();
            List<RPQJob> solution = new List<RPQJob>();
            int time = jobs.Min(x => x.PreparationTime);

            stopwatch.Start();
            while (readyJobs.Count > 0 || jobs.Count > 0)
            {
                while (jobs.Count > 0 && jobs.Min(x => x.PreparationTime) <= time)
                {
                    RPQJob job = jobs.Aggregate((currentMinimum, x) => (x.PreparationTime < currentMinimum.PreparationTime ? x : currentMinimum));
                    readyJobs.Add(job);
                    jobs.Remove(job);
                }
                if (readyJobs.Count == 0)
                    time = jobs.Min(x => x.PreparationTime);
                else
                {
                    RPQJob job = readyJobs.Aggregate((currentMaximum, x) => (x.DeliveryTime > currentMaximum.DeliveryTime ? x : currentMaximum));
                    readyJobs.Remove(job);
                    solution.Add(job);
                    time += job.WorkTime;
                    Cmax = Math.Max(Cmax, job.DeliveryTime + time);
                }
            }
            stopwatch.Stop();
            return solution;
        }

        public static List<RPQJob> SolveUsingQueue(List<RPQJob> jobs, out int Cmax, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            Cmax = 0;
            PriorityQueue jobsPreparationQueue = new PriorityQueue();
            PriorityQueue jobsDeliveryQueue = new PriorityQueue();

            foreach (RPQJob item in jobs)
            {
                jobsPreparationQueue.Add(item, item.PreparationTime);
            }

            List<RPQJob> solution = new List<RPQJob>();
            int time = jobsPreparationQueue.GetLowest().PreparationTime;

            stopwatch.Start();
            while (jobsDeliveryQueue.Count > 0 || jobsPreparationQueue.Count > 0)
            {
                while (jobsPreparationQueue.Count > 0 && jobsPreparationQueue.GetLowest().PreparationTime <= time)
                {
                    RPQJob job = jobsPreparationQueue.GetLowestAndRemove();
                    jobsDeliveryQueue.Add(job, job.DeliveryTime);
                }
                if (jobsDeliveryQueue.Count == 0)
                    time = jobsPreparationQueue.GetLowest().PreparationTime;
                else
                {
                    RPQJob job = jobsDeliveryQueue.GetHighestAndRemove();
                    solution.Add(job);
                    time += job.WorkTime;
                    Cmax = Math.Max(Cmax, job.DeliveryTime + time);
                }
            }
            stopwatch.Stop();
            return solution;
        }

        public static List<RPQJob> SolveUsingQueue(List<RPQJob> jobs, out int Cmax)
        {

            Cmax = 0;
            PriorityQueue<RPQJob> jobsPreparationQueue = new PriorityQueue<RPQJob>(true);
            PriorityQueue<RPQJob> jobsDeliveryQueue = new PriorityQueue<RPQJob>(false);

            foreach (RPQJob item in jobs)
            {
                jobsPreparationQueue.Enqueue(item.PreparationTime, item);
            }

            List<RPQJob> solution = new List<RPQJob>();
            int time = jobsPreparationQueue.GetValueAtZero().PreparationTime;


            while (jobsDeliveryQueue.Count > 0 || jobsPreparationQueue.Count > 0)
            {
                while (jobsPreparationQueue.Count > 0 && jobsPreparationQueue.GetValueAtZero().PreparationTime <= time)
                {
                    RPQJob job = jobsPreparationQueue.Dequeue();
                    jobsDeliveryQueue.Enqueue(job.DeliveryTime, job);
                }
                if (jobsDeliveryQueue.Count == 0)
                    time = jobsPreparationQueue.GetValueAtZero().PreparationTime;
                else
                {
                    RPQJob job = jobsDeliveryQueue.Dequeue();
                    solution.Add(job);
                    time += job.WorkTime;
                    Cmax = Math.Max(Cmax, job.DeliveryTime + time);
                }
            }

            return solution;
        }
    }
}
