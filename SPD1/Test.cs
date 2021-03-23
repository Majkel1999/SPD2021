using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace SPD1
{
    class Test
    {
        string[] instance;
        string testFilesPath = ".\\TestFiles\\";
        public void RunTest()
        {
            instance = Directory.GetFiles(testFilesPath);          
            StreamWriter file = new("Test.txt");
            file.Write("Instancja");
            file.Write(";");
            file.Write("Johnson");
            file.Write(";");
            file.Write("Przeglad zupelny");
            file.Write("\n");
            for (int i =0; i < instance.Count(); i++)
            {
                LoadData data = new LoadData();
                data.ReadFromFileToTest(instance[i]);
                file.Write(instance[i]);
                file.Write(";");
                JohnsonAlgorithm johnsonAlgorithm = new();
                List<List<JobObject>> list = johnsonAlgorithm.RunToTest(out Stopwatch stopwatch,data);
                file.Write(stopwatch.Elapsed.TotalMilliseconds);
                file.Write(";");
                LoadData data2 = new LoadData();
                data2.ReadFromFileToTest(instance[i]);
                PBAlgorithm pbAlgorithm = new();
                List<List<JobObject>> list2 = pbAlgorithm.RunToTest(out Stopwatch stopwatch2,data2);
                file.Write(stopwatch2.Elapsed.TotalMilliseconds);
                file.Write(";");
                file.Write("\n");
            }
            file.Close();
        }
    }
}
