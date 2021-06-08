using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SPD1.Misc
{
    public class WitiJob
    {
        public int workTime;
        public int weight;
        public int desiredEndTime;

    }
    public static class LoadWitiData
    {
        public static List<WitiJob> LoadDataFromFile(string name)
        {
            List<WitiJob> jobsList = new List<WitiJob>();
            using (StreamReader reader = new StreamReader(name))
            {
                string line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] numbers = Regex.Split(line, "\\s+");
                    jobsList.Add(new WitiJob
                    {
                        workTime = int.Parse(numbers[0]),
                        weight = int.Parse(numbers[1]),
                        desiredEndTime = int.Parse(numbers[2]),
                    });
                }
            }
            return jobsList;
        }

        public static List<WitiJob> LoadDataFromFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter += "Text document | *.txt";
            if (fileDialog.ShowDialog() == true)
            {
                List<WitiJob> jobsList = new List<WitiJob>();
                using (StreamReader reader = new StreamReader(fileDialog.FileName))
                {
                    string line = reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string[] numbers = Regex.Split(line, "\\s+");
                        jobsList.Add(new WitiJob
                        {
                            workTime = int.Parse(numbers[0]),
                            weight = int.Parse(numbers[1]),
                            desiredEndTime = int.Parse(numbers[2]),
                        });
                    }
                }
                return jobsList;
            }
            return null;
        }
    }
}

