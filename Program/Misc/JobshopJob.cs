using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SPD1
{
    public class JobshopOperation
    {
        public int MachineNumber;
        public int Duration;
    }

    public class JobshopJob
    {
        public List<JobshopOperation> OperationsList = new List<JobshopOperation>();
        public int JobsCount => OperationsList.Count;

    }
    public static class JobshopData
    {

        public static List<JobshopJob> LoadDataFromFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter += "Text document | *.txt";
            if (fileDialog.ShowDialog() == true)
            {
                List<JobshopJob> jobs = new List<JobshopJob>();
                using (StreamReader reader = new StreamReader(fileDialog.FileName))
                {
                    string line = reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        JobshopJob job = new JobshopJob();
                        line = line.Trim();
                        string[] numbers = Regex.Split(line, "\\s+");
                        for (int i = 1; i + 1 < numbers.Length; i += 2)
                        {
                            job.OperationsList.Add(new JobshopOperation
                            {
                                MachineNumber = int.Parse(numbers[i]),
                                Duration = int.Parse(numbers[i + 1])
                            });
                        }
                        jobs.Add(job);
                    }
                }
                return jobs;
            }
            return null;
        }
    }
}