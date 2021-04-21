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
        private int _machinesQuantity;
        public int MachinesQuantity { get => _machinesQuantity; set => _machinesQuantity = value; }
        private int _jobsQuantity;
        public int JobsQuantity { get => _jobsQuantity; set => _jobsQuantity = value; }
        private List<List<int>> _jobs;
        public List<List<int>> Jobs { get => _jobs; set => _jobs = value; }

        public LoadData() { }

        public LoadData(LoadData loadData)
		{
            MachinesQuantity = loadData.MachinesQuantity;
            JobsQuantity = loadData.JobsQuantity;
            Jobs = loadData.Jobs;
		}

        public void ReadFromFile()
        {
            _jobs = new List<List<int>>();
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
                    _jobsQuantity = int.Parse(line[0]); //read count of machines and jobs to do
                    _machinesQuantity = int.Parse(line[1]);

                    while ((line2 = file.ReadLine()) != null)
                    {
                        line = line2.Split();
                        _jobs.Add(new List<int>());
                        for (int i = 0; i < _machinesQuantity; i++)
                        {
                            _jobs[lineCounter].Add(int.Parse(line[i]));
                        }
                        lineCounter++;
                    }
                    file.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void ReadFromFileToTest(string fileName)
        {
            _jobs = new List<List<int>>();
            try
            {
                StreamReader file = new StreamReader(fileName);
                string[] line;
                string line2;
                int lineCounter = 0;

                line = file.ReadLine().Split();
                _jobsQuantity = int.Parse(line[0]); //read count of machines and jobs to do
                _machinesQuantity = int.Parse(line[1]);

                while ((line2 = file.ReadLine()) != null)
                {
                    line = line2.Split();
                    _jobs.Add(new List<int>());
                    for (int i = 0; i < _machinesQuantity; i++)
                    {
                        _jobs[lineCounter].Add(int.Parse(line[i]));
                    }
                    lineCounter++;
                }
                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


    }
}
