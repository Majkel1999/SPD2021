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
        public void RunTest1()
        {
            instance = Directory.GetFiles(testFilesPath);
            StreamWriter file = new StreamWriter("Test1.txt");
            file.Write("Instancja");
            file.Write(";");
            file.Write("Johnson");
            file.Write(";");
            file.Write("Przeglad zupelny");
            file.Write("\n");
            for (int i = 0; i < instance.Count(); i++)
            {
                LoadData data = new LoadData();
                data.ReadFromFileToTest(instance[i]);
                file.Write(instance[i]);
                file.Write(";");
                JohnsonAlgorithm johnsonAlgorithm = new JohnsonAlgorithm();
                List<List<JobObject>> list = johnsonAlgorithm.RunToTest(out Stopwatch stopwatch, data);
                file.Write(stopwatch.Elapsed.TotalMilliseconds);
                file.Write(";");
                LoadData data2 = new LoadData();
                data2.ReadFromFileToTest(instance[i]);
                PBAlgorithm pbAlgorithm = new PBAlgorithm();
                List<List<JobObject>> list2 = pbAlgorithm.RunToTest(out Stopwatch stopwatch2, data2);
                file.Write(stopwatch2.Elapsed.TotalMilliseconds);
                file.Write(";");
                file.Write("\n");
            }
            file.Close();
        }

        public void RunTest2()
        {
            instance = Directory.GetFiles(testFilesPath);
            StreamWriter file = new StreamWriter("Test2.txt");
            file.WriteLine("Instancja;Maszyny;Zadania;Johnson;;Neh;;NehMod1;;NehMod2;;NehMod3;;NehMod4");
            file.WriteLine("Instancja;Maszyny;Zadania;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas");
            Stopwatch stopwatch;
            for (int i = 0; i < instance.Count(); i++)
            {
                Trace.WriteLine("Jestesmy na: " + instance[i]);
                LoadData data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + data.MachinesQuantity + ";" + data.JobsQuantity + ";");

                JohnsonAlgorithm johnsonAlgorithm = new JohnsonAlgorithm();
                List<List<JobObject>> list = johnsonAlgorithm.RunToTest(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                NehAlgorithm nehAlgorithm = new NehAlgorithm();
                list = nehAlgorithm.Run(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                list = nehAlgorithm.RunModification1(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");
                list = nehAlgorithm.RunModification2(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");
                list = nehAlgorithm.RunModification3(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");
                list = nehAlgorithm.RunModification4(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + "\n");
            }
            file.Close();
        }

        public void RunTest3()
        {
            instance = Directory.GetFiles(testFilesPath);
            StreamWriter file = new StreamWriter("Test3.txt");
            file.WriteLine("Instancja;Maszyny;Zadania;Johnson;;Neh;;Tabu");
            file.WriteLine("Instancja;Maszyny;Zadania;CMax;Czas;CMax;Czas;CMax;Czas");
            Stopwatch stopwatch;
            for (int i = 0; i < instance.Count(); i++)
            {
                Trace.WriteLine("Jestesmy na: " + instance[i]);
                LoadData data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + data.MachinesQuantity + ";" + data.JobsQuantity + ";");

                JohnsonAlgorithm johnsonAlgorithm = new JohnsonAlgorithm();
                List<List<JobObject>> list = johnsonAlgorithm.RunToTest(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                NehAlgorithm nehAlgorithm = new NehAlgorithm();
                list = nehAlgorithm.Run(out stopwatch, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                TSAlgorithm tabuSearch = new TSAlgorithm();
                list = tabuSearch.Run(out stopwatch, 600, 2000, 250, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + "\n");
            }
            file.Close();
        }

        public void RunTest4()
        {
            instance = Directory.GetFiles(testFilesPath);
            StreamWriter file = new StreamWriter("Test4.txt");
            file.WriteLine("Instancja;Maszyny;Zadania;Tabu;;Mod1;;Mod2;;Mod3");
            file.WriteLine("Instancja;Maszyny;Zadania;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas");
            Stopwatch stopwatch;
            List<List<JobObject>> list;
            for (int i = 0; i < instance.Count(); i++)
            {
                Trace.WriteLine("Jestesmy na: " + instance[i]);

                LoadData data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + data.MachinesQuantity + ";" + data.JobsQuantity + ";");

                TSAlgorithm tabuSearch = new TSAlgorithm();
                list = tabuSearch.Run(out stopwatch, 600, 2000, 250, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                //data = new LoadData();
                //data.ReadFromFileToTest(instance[i]);

                //list = tabuSearch.RunMod1(out stopwatch, 600, 2000, 250, data);
                //file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                //data = new LoadData();
                //data.ReadFromFileToTest(instance[i]);

                //list = tabuSearch.RunMod2(out stopwatch, 600, 2000, 250, data);
                //file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                list = tabuSearch.RunMod3(out stopwatch, 2000, 2000, 250, data);
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + "\n");
            }
            file.Close();
        }

        public void ParseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> b = openFileDialog.ShowDialog();
            if (b == true)
            {
                string fileName = openFileDialog.FileName;
                if (!fileName.EndsWith("txt"))
                {
                    return;
                }
                using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("data"))
                        {
                            using (StreamWriter sw = new StreamWriter(".\\TestFiles\\Data\\" + line.TrimEnd(':') + ".txt"))
                            {
                                while ((line = sr.ReadLine()) != null)
                                {
                                    line.Trim();
                                    if (line == "")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        sw.WriteLine(line);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
