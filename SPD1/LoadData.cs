using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;

namespace SPD1
{
    public class LoadData
    {
        public int machinesQuantity;
        public int jobQuantity;
        public List<List<int>> jobs;
        public void ReadFromFile()
        {
            jobs = new List<List<int>>();

            //ConsoleAllocator.ShowConsoleWindow();
            //Console.WriteLine("Enter name of file: ");

            try
            {
                string fileName;
                OpenFileDialog fileDialog = new OpenFileDialog();
                Nullable<bool> result = fileDialog.ShowDialog();
                if (result == true)
                {
                    fileName = fileDialog.FileName;

                    StreamReader file = new StreamReader(fileName);
                    string[] line;
                    string line2;
                    int lineCounter = 0;

                    line = file.ReadLine().Split();
                    jobQuantity = int.Parse(line[0]); //read count of machines and jobs to do
                    machinesQuantity = int.Parse(line[1]);

                    while ((line2 = file.ReadLine()) != null)
                    {
                        line = line2.Split();
                        jobs.Add(new List<int>());
                        for (int i = 0; i < machinesQuantity; i++)
                        {
                            jobs[lineCounter].Add(int.Parse(line[i]));
                        }
                        lineCounter++;
                    }
                    file.Close();
                }
                //foreach(List<int> i in jobs)
                //{
                //    foreach(int j in i)
                //    {
                //        Console.Write(j+" ");
                //    }
                //    Console.WriteLine();
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //Console.WriteLine(e.Message);
                //Console.WriteLine("Error while reading file");
            }
        }
    }
}
