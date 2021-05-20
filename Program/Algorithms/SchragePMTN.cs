using System;
using System.Collections.Generic;
using System.Linq;
using SPD1.Misc;
using System.Diagnostics;

namespace SPD1.Algorithms
{
    class SchragePMTN
    {
        public static int Solve(List<RPQJob> jobs, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            List<RPQJob> notReadyJobs = jobs.ToList();
            List<RPQJob> readyJobs = new List<RPQJob>();
            int time = 0;
            int Cmax = 0;
            stopwatch.Start();
            
            RPQJob l = notReadyJobs[0];
            l.DeliveryTime = int.MaxValue;

            while (readyJobs.Count > 0 || notReadyJobs.Count > 0)
            {
                while (notReadyJobs.Count > 0 && notReadyJobs.Min(x => x.PreparationTime) <= time)
                {
                    RPQJob job = notReadyJobs.Aggregate((currentMinimum, x) => (x.PreparationTime < currentMinimum.PreparationTime ? x : currentMinimum));
                    readyJobs.Add(job);
                    notReadyJobs.Remove(job);
                    if (job.DeliveryTime>l.DeliveryTime)
                    {
                        l.WorkTime = time - job.PreparationTime;
                        time = job.PreparationTime;
                        if (l.WorkTime>0)
                        {
                            readyJobs.Add(l);
                        }
                    }
                }
                if (readyJobs.Count == 0)
                    time = notReadyJobs.Min(x => x.PreparationTime);
                else
                {
                    RPQJob job = readyJobs.Aggregate((currentMaximum, x) => (x.DeliveryTime > currentMaximum.DeliveryTime ? x : currentMaximum));
                    readyJobs.Remove(job);
                    l = job;
                    time += job.WorkTime;
                    Cmax = Math.Max(Cmax, job.DeliveryTime + time);
                }
            }
            stopwatch.Stop();
            return Cmax;
        }

        public static int SolveUsingQueue(List<RPQJob> jobs, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            PriorityQueue jobsPreparationQueue = new PriorityQueue();
            PriorityQueue jobsDeliveryQueue = new PriorityQueue();
            List<RPQJob> temp = jobs.ToList();

            foreach (RPQJob item in temp)
            {
                jobsPreparationQueue.Add(item, item.PreparationTime);
            }

            int time = 0;
            int Cmax = 0;

            stopwatch.Start();
            RPQJob l = jobsPreparationQueue.GetLowest();
            l.DeliveryTime = int.MaxValue;

            while (jobsDeliveryQueue.Count > 0 || jobsPreparationQueue.Count > 0)
            {
                while (jobsPreparationQueue.Count > 0 && jobsPreparationQueue.GetLowest().PreparationTime <= time)
                {
                    RPQJob job = jobsPreparationQueue.GetLowestAndRemove();
                    jobsDeliveryQueue.Add(job, job.DeliveryTime);
                    if (job.DeliveryTime > l.DeliveryTime)
                    {
                        l.WorkTime = time - job.PreparationTime;
                        time = job.PreparationTime;
                        if (l.WorkTime > 0)
                        {
                            jobsDeliveryQueue.Add(l,l.DeliveryTime);
                        }
                    }
                }
                if (jobsDeliveryQueue.Count == 0)
                    time = jobsPreparationQueue.GetLowest().PreparationTime;
                else
                {
                    RPQJob job = jobsDeliveryQueue.GetHighestAndRemove();
                    l = job;
                    time += job.WorkTime;
                    Cmax = Math.Max(Cmax, job.DeliveryTime + time);
                }
            }
            stopwatch.Stop();
            return Cmax;
        }

        public static int SolveUsingQueue(List<RPQJob> jobs)
        {
            PriorityQueue jobsPreparationQueue = new PriorityQueue();
            PriorityQueue jobsDeliveryQueue = new PriorityQueue();
            List<RPQJob> temp = jobs.ToList();

            foreach (RPQJob item in temp)
            {
                jobsPreparationQueue.Add(item, item.PreparationTime);
            }

            int time = 0;
            int Cmax = 0;

            RPQJob l = jobsPreparationQueue.GetLowest();
            l.DeliveryTime = int.MaxValue;

            while (jobsDeliveryQueue.Count > 0 || jobsPreparationQueue.Count > 0)
            {
                while (jobsPreparationQueue.Count > 0 && jobsPreparationQueue.GetLowest().PreparationTime <= time)
                {
                    RPQJob job = jobsPreparationQueue.GetLowestAndRemove();
                    jobsDeliveryQueue.Add(job, job.DeliveryTime);
                    if (job.DeliveryTime > l.DeliveryTime)
                    {
                        l.WorkTime = time - job.PreparationTime;
                        time = job.PreparationTime;
                        if (l.WorkTime > 0)
                        {
                            jobsDeliveryQueue.Add(l, l.DeliveryTime);
                        }
                    }
                }
                if (jobsDeliveryQueue.Count == 0)
                    time = jobsPreparationQueue.GetLowest().PreparationTime;
                else
                {
                    RPQJob job = jobsDeliveryQueue.GetHighestAndRemove();
                    l = job;
                    time += job.WorkTime;
                    Cmax = Math.Max(Cmax, job.DeliveryTime + time);
                }
            }
            return Cmax;
        }
    }
}
