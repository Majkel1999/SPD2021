using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPD1
{
    class PBAlgorithm
    {
        public List<List<JobObject>> Run(out Stopwatch stopwatch)
        {
            LoadData data = new LoadData();
            data.ReadFromFile();
            stopwatch = new();
            stopwatch.Start();
            List<List<JobObject>> listMinMakespan = new List<List<JobObject>>();
            int minMakespan = 0;

            List<int> listToPermute = new List<int>();
            for(int i = 0; i < data.JobsQuantity; i++)
            {
                listToPermute.Add(i + 1);
            }
            Permutation perms = new Permutation();
            List<List<int>> testPermutations = perms.GetAllPermutations(listToPermute, listToPermute.Count);
            
            //test all permutations with their time
            for (int i = 0; i < testPermutations.Count; i++)
            {
                int tempTime;
                List<List<JobObject>> tempList = new List<List<JobObject>>();
                int[] tasks = testPermutations[i].ToArray();
                for(int j = 0; j < data.MachinesQuantity; j++)
                {
                    tempList.Add(new List<JobObject>());
                    for (int k = 0; k < tasks.Length; k++)
                    {
                        int jobIndex = tasks[k];
                        int startTime;
                        if(j==0 && k ==0)
                        {
                            startTime = 0;
                        }
                        else if(k==0 && j!=0)
                        {
                            startTime = tempList[j - 1][k].StopTime;
                        }
                        else if(k!=0 && j==0)
                        {
                            startTime = tempList[j][k - 1].StopTime;
                        }
                        else
                        {
                            startTime = Math.Max(tempList[j - 1][k].StopTime, tempList[j][k - 1].StopTime);
                        }
                        int stopTime = startTime + data.Jobs[jobIndex-1][j];
                        tempList[j].Add(new JobObject
                        {
                            JobIndex = jobIndex,
                            StartTime = startTime,
                            StopTime = stopTime
                        })  ;
                    }
                }
                tempTime = tempList[data.MachinesQuantity - 1][data.JobsQuantity - 1].StopTime;
                if (minMakespan == 0)
                {
                    listMinMakespan = tempList;
                    minMakespan = tempTime;
                }
                if (minMakespan >= tempTime)
                {
                    listMinMakespan = tempList;
                    minMakespan = tempTime;
                }
            }

            //ConsoleAllocator.ShowConsoleWindow();
            //foreach (List<JobObject> i in listMinMakespan)
            //{
            //    foreach (JobObject j in i)
            //    {
            //        Console.Write(j.StartTime + "." + j.StopTime + "   ");
            //    }
            //    Console.WriteLine();
            //}
            stopwatch.Stop();
            return listMinMakespan;
        }
    }
}
