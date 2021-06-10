using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPD1
{
    class Gantt
    {

        public static List<List<JobObject>> MakeGanttChart(List<int> list, LoadData data)
        {
            List<List<JobObject>> listOfJobs = new List<List<JobObject>>();
            for (int i = 0; i < data.MachinesQuantity; i++)
            {
                listOfJobs.Add(new List<JobObject>());

                int delay = 0;
                int jobTimeSum = 0;
                if (i != 0)
                {
                    for (int k = i - 1; k >= 0; k--)
                    {
                        delay += listOfJobs[k][0].StopTime - listOfJobs[k][0].StartTime;
                    }
                }

                if (i == 0)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        listOfJobs[i].Add(new JobObject
                        {
                            JobIndex = list[j] + 1,
                            StartTime = delay + jobTimeSum,
                            StopTime = delay + jobTimeSum + data.Jobs[list[j]][i]
                        });
                        jobTimeSum += listOfJobs[i][j].StopTime - listOfJobs[i][j].StartTime;
                    }
                }
                else
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        JobObject job = new JobObject();
                        job.JobIndex = list[j] + 1;
                        job.StartTime = delay + jobTimeSum;
                        job.StopTime = delay + jobTimeSum + data.Jobs[list[j]][i];

                        //Jeśli zadanie na poprzedniej maszynie nie jest skończone to przesuń do momentu zakończenia
                        if (job.StartTime < listOfJobs[i - 1][j].StopTime)
                        {
                            job.StartTime = listOfJobs[i - 1][j].StopTime;
                            job.StopTime = job.StartTime + data.Jobs[list[j]][i];
                        }

                        listOfJobs[i].Add(job);
                        jobTimeSum = listOfJobs[i][j].StopTime - delay;
                    }
                }
            }
            return listOfJobs;
        }

        public static int GetCmax(List<List<JobObject>> jobObjects)
		{
            return jobObjects.Last().Last().StopTime;
		}

        public static int GetCmax(List<int> permutation, LoadData data)
        {
            return MakeGanttChart(permutation, data).Last().Last().StopTime;
        }
    }
}
