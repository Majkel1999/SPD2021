using System.Collections.Generic;
using System.Linq;
using SPD1.Misc;

namespace SPD1.Algorithms
{
    public class Schrage
    {
        public static List<RPQJob> Solve(List<RPQJob> jobs)
        {
            List<RPQJob> readyJobs = new List<RPQJob>();
            List<RPQJob> solution = new List<RPQJob>();
            int time = jobs.Min(x => x.PreparationTime);

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
                }
            }
            return solution;
        }
    }
}
