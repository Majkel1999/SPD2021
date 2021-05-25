﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using SPD1.Misc;
using SPD1.Algorithms;

namespace SPD1
{
    class Test
    {
        string[] instance;
        string rpqTestFilesPath = ".\\TestFiles\\RPQ\\";
        string multiTestFilesPath = ".\\TestFiles\\Multi\\";

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

        public void RunTest1()
        {
            instance = Directory.GetFiles(multiTestFilesPath);
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
            instance = Directory.GetFiles(multiTestFilesPath);
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
            instance = Directory.GetFiles(multiTestFilesPath);
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
            ConsoleAllocator.ShowConsoleWindow();
            instance = Directory.GetFiles(multiTestFilesPath);
            StreamWriter file = new StreamWriter("Test4.txt");
            file.WriteLine("Instancja;Maszyny;Zadania;Johnson;;Neh;;;Tabu;;Mod1;;Mod2;;Mod3;;Mod4");
            file.WriteLine("Instancja;Maszyny;Zadania;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas");
            Stopwatch stopwatch;
            List<List<JobObject>> list;
            for (int i = 0; i < instance.Count(); i++)
            {
                Console.WriteLine("Jestesmy na: " + instance[i]);

                LoadData data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + data.MachinesQuantity + ";" + data.JobsQuantity + ";");

                JohnsonAlgorithm johnsonAlgorithm = new JohnsonAlgorithm();
                list = johnsonAlgorithm.RunToTest(out stopwatch, data);
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
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                list = tabuSearch.RunMod1(out stopwatch, 600, 500, 250, data); //Warunek stopu do iteracji bez poprawy
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                list = tabuSearch.RunMod2(out stopwatch, 600, 2000, 250, data); //Johnson
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                list = tabuSearch.RunMod3(out stopwatch, 600, 2000, 250, data);//Swap zamieniony na insert
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + ";");

                data = new LoadData();
                data.ReadFromFileToTest(instance[i]);

                list = tabuSearch.RunMod4(out stopwatch, 600, 2000, 250, data); //Warunek stopu do iteracji bez poprawy
                file.Write(list.Last().Last().StopTime + ";" + stopwatch.Elapsed.TotalMilliseconds + "\n");
            }
            file.Close();
        }

        public void RunSchrageTest()
        {
            instance = Directory.GetFiles(rpqTestFilesPath);
            StreamWriter file = new StreamWriter("SchrageTest.txt");
            file.WriteLine("Instancja;Zadania;Schrage;;SchrageQueue;;SchragePmtn;;SchragePmtnQueue");
            file.WriteLine("Instancja;Zadania;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas");
            Stopwatch stopwatch;
            for (int i = 0; i < instance.Count(); i++)
            {
                Trace.WriteLine("Jestesmy na: " + instance[i]);
                List<RPQJob> list = RPQLoadData.LoadDataFromFile(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + list.Count + ";");

                Schrage.Solve(list, out int Cmax, out stopwatch);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");

                Schrage.SolveUsingQueue(list, out Cmax, out stopwatch);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");

                Cmax = SchragePMTN.Solve(list, out stopwatch);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");

                Cmax = SchragePMTN.SolveUsingQueue(list, out stopwatch);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
            }
            file.Close();
        }

        public void RunTabuRPQTest()
        {
            instance = Directory.GetFiles(rpqTestFilesPath);
            StreamWriter file = new StreamWriter("TabuRPQ.txt");
            file.WriteLine("Instancja;Zadania;Tabu200;;Tabu500;;Tabu1000;;");
            file.WriteLine("Instancja;Zadania;CMax;Czas;CMax;Czas;CMax;Czas");
            Stopwatch stopwatch;
            for (int i = 0; i < instance.Count(); i++)
            {
                Trace.WriteLine("Jestesmy na: " + instance[i]);
                List<RPQJob> list = RPQLoadData.LoadDataFromFile(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + list.Count + ";");

                TSAlgorithm ts = new TSAlgorithm();
                int Cmax = ts.RunRPQ(out stopwatch, 200, 200, 200, list);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");
                stopwatch.Reset();

                list = RPQLoadData.LoadDataFromFile(instance[i]);
                Cmax = ts.RunRPQ(out stopwatch, 500, 500, 500, list);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");
                stopwatch.Reset();

                list = RPQLoadData.LoadDataFromFile(instance[i]);
                Cmax = ts.RunRPQ(out stopwatch, 1000, 1000, 1000, list);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
                stopwatch.Reset();

			}
			file.Close();
		}
public void RunFullRPQTest()
        {
            instance = Directory.GetFiles(rpqTestFilesPath);
            StreamWriter file = new StreamWriter("TabuRPQ.txt");
            file.WriteLine("Instancja;Zadania;Tabu200;;Tabu500;;Tabu1000;;Carlier;;GreedyCarlier;;DeepGreedyCarlier");
            file.WriteLine("Instancja;Zadania;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;CMax;Czas;");
            Stopwatch stopwatch;
            for (int i = 0; i < instance.Count(); i++)
            {
                Trace.WriteLine("Jestesmy na: " + instance[i]);
                List<RPQJob> list = RPQLoadData.LoadDataFromFile(instance[i]);

                file.Write(instance[i].Split('\\').Last() + ";" + list.Count + ";");

                TSAlgorithm ts = new TSAlgorithm();
                int Cmax = ts.RunRPQ(out stopwatch, 200, 200, 200, list);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");
                stopwatch.Reset();

                list = RPQLoadData.LoadDataFromFile(instance[i]);
                Cmax = ts.RunRPQ(out stopwatch, 500, 500, 500, list);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");
                stopwatch.Reset();

                list = RPQLoadData.LoadDataFromFile(instance[i]);
                Cmax = ts.RunRPQ(out stopwatch, 1000, 1000, 1000, list);
                file.Write(Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
                stopwatch.Reset();

                if (!instance[i].Contains("data.007") || !instance[i].Contains("data.008"))
                {
                    list = RPQLoadData.LoadDataFromFile(instance[i]);
                    Carlier carlier = new Carlier();
                    carlier.Solve(list, out stopwatch);
                    file.Write(carlier.Cmax + ";");
                    file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
                    stopwatch.Reset();
                }

                list = RPQLoadData.LoadDataFromFile(instance[i]);
                GreedyCarlier greedyCarlier = new GreedyCarlier();
                greedyCarlier.Solve(list, out stopwatch);
                file.Write(greedyCarlier.Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
                stopwatch.Reset();

                list = RPQLoadData.LoadDataFromFile(instance[i]);
                greedyCarlier = new GreedyCarlier(true);
                greedyCarlier.Solve(list, out stopwatch);
                file.Write(greedyCarlier.Cmax + ";");
                file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
                stopwatch.Reset();

            }
            file.Close();
        }

		public void RunCarlierRPQTest()
		{
			string filesPath = ".\\TestFiles\\Data\\";
			instance = Directory.GetFiles(filesPath);
			StreamWriter file = new StreamWriter("CarlierRPQ.txt");
			file.WriteLine("Instancja;Zadania;;BasicCarlier;;CarlierUsingQueue");
			file.WriteLine("Instancja;Zadania;CMax;Czas;CMax;Czas");
			Stopwatch stopwatch;
			for (int i = 0; i < instance.Count()-2; i++)
			{
				Trace.WriteLine("Jestesmy na: " + instance[i]);
				List<RPQJob> list = RPQLoadData.LoadDataFromFile(instance[i]);

				file.Write(instance[i].Split('\\').Last() + ";" + list.Count + ";");

				Carlier carlier = new Carlier();
				carlier.Solve(list, out stopwatch);
				file.Write(carlier.Cmax + ";");
				file.Write(stopwatch.Elapsed.TotalMilliseconds + ";");

				Carlier carlier2 = new Carlier();
				carlier2.Solve(list, out stopwatch);
				file.Write(carlier.Cmax + ";");
				file.Write(stopwatch.Elapsed.TotalMilliseconds + ";\n");
				stopwatch.Reset();
			}
			file.Close();
		}
	}
}
