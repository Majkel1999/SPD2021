using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SPD1.Misc
{
    public struct RPQJob
    {
        public int JobIndex { get; set; }
        public int PreparationTime { get; set; }
        public int WorkTime { get; set; }
        public int DeliveryTime { get; set; }
    }

    public struct RPQChartData
    {
        public int JobIndex { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int DeliveryTime { get; set; }
    }
    public class RPQChart
    {
        public static List<RPQChartData> MakeRPQChart(List<RPQJob> chartData)
        {
            RPQChartData prev = new RPQChartData();
            List<RPQChartData> solution = new List<RPQChartData>();
            for( int i = 0;i<chartData.Count;i++)
            {
                RPQJob job = chartData[i];
                RPQChartData data = new RPQChartData();
                data.JobIndex = job.JobIndex;
                if (solution.Count == 0)
                {
                    data.StartTime = job.PreparationTime;
                }
                else if(job.PreparationTime>prev.EndTime)
                {
                    data.StartTime = job.PreparationTime;
                }
                else
                {
                    data.StartTime = prev.EndTime;
                }
                data.EndTime = data.StartTime + job.WorkTime;
                data.DeliveryTime = data.EndTime + job.DeliveryTime;
                prev = data;
                solution.Add(data);
            }
            return solution;
        }
    }
    public static class RPQLoadData
    {
        public static List<RPQJob> LoadDataFromFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter += "Text document | *.txt";
            if (fileDialog.ShowDialog() == true)
            {
                List<RPQJob> jobsList = new List<RPQJob>();
                using (StreamReader reader = new StreamReader(fileDialog.FileName))
                {
                    string line = reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string[] numbers = Regex.Split(line, "\\s+");
                        jobsList.Add(new RPQJob
                        {
                            JobIndex = jobsList.Count + 1,
                            PreparationTime = int.Parse(numbers[0]),
                            WorkTime = int.Parse(numbers[1]),
                            DeliveryTime = int.Parse(numbers[2])
                        });
                    }
                }
                return jobsList;
            }
            return null;
        }

        public static List<RPQJob> LoadDataFromFile(string name)
        {
            List<RPQJob> jobsList = new List<RPQJob>();
            using (StreamReader reader = new StreamReader(name))
            {
                string line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] numbers = Regex.Split(line, "\\s+");
                    jobsList.Add(new RPQJob
                    {
                        JobIndex = jobsList.Count + 1,
                        PreparationTime = int.Parse(numbers[0]),
                        WorkTime = int.Parse(numbers[1]),
                        DeliveryTime = int.Parse(numbers[2])
                    });
                }
            }
            return jobsList;
        }

    }

}
