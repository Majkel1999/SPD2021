using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace SPD1
{
    public class PermutationMakeSpan
    {
        public string permutation { get; set; }
        public float makespan { get; set; }
    }

    class PBAlgorithm  //Permutation Based Algorithm
    {
        /// <summary>
        /// Uruchamia algorytm na danych z pliku i zwraca dane sformatowane do wizualizacji
        /// </summary
        public List<List<JobObject>> Run(out Stopwatch stopwatch)
        {
            //Lista wyników permutation-makespan
            List<PermutationMakeSpan> permutationMakeSpanList = new List<PermutationMakeSpan>();

            //otwórz okno do wyświetlania czasów
            MakespansWindow window = new MakespansWindow();
            window.permutationMakeSpanList = permutationMakeSpanList;
            window.Show();

            //wczytaj dane z pliku
            LoadData data = new LoadData();
            data.ReadFromFile();

            //zacznij liczyć czas
            stopwatch = new();
            stopwatch.Start();

            //wynik działania - dane sformatowane dla wyjścia
            List<List<JobObject>> listMinMakespan = new List<List<JobObject>>();
            int minMakespan = 0;

            //stworzenie listy do permutacji
            List<int> listToPermute = new List<int>();
            for (int i = 0; i < data.JobsQuantity; i++)
            {
                listToPermute.Add(i + 1);
            }

            //wykonanie permutacji

            foreach (var perm in Permutation.Permutate(listToPermute, listToPermute.Count))
            {
                List<List<JobObject>> tempList = new List<List<JobObject>>();

                for (int j = 0; j < data.MachinesQuantity; j++)
                {
                    tempList.Add(new List<JobObject>());
                    for (int k = 0; k < perm.Count; k++)
                    {
                        int jobIndex = (int)perm[k];
                        int startTime;
                        if (j == 0 && k == 0) //pierwsza maszyna pierwsze zadanie
                        {
                            startTime = 0;
                        }
                        else if (k == 0 && j != 0) //pierwsze zadanie kolejna maszyna
                        {
                            startTime = tempList[j - 1][k].StopTime;
                        }
                        else if (k != 0 && j == 0) //pierwsza maszyna kolejne zadania
                        {
                            startTime = tempList[j][k - 1].StopTime;
                        }
                        else //pozostałe przypadki
                        {
                            startTime = Math.Max(tempList[j - 1][k].StopTime, tempList[j][k - 1].StopTime);
                        }
                        //zapisanie zestawu danych zadanie, czas rozpoczęcia, czas zakończenia
                        int stopTime = startTime + data.Jobs[jobIndex - 1][j];
                        tempList[j].Add(new JobObject
                        {
                            JobIndex = jobIndex,
                            StartTime = startTime,
                            StopTime = stopTime
                        });
                    }
                }
                //wyznacz czas zakończenia całego cyklu
                int tempTime = tempList[data.MachinesQuantity - 1][data.JobsQuantity - 1].StopTime;

                if (minMakespan == 0) //jeśli to pierwsza permutacja przypisz dane niezależnie jakie są
                {
                    listMinMakespan = tempList;
                    minMakespan = tempTime;
                }
                if (minMakespan >= tempTime) //sprawdzenie czy istnieje mniejszy czas
                {
                    listMinMakespan = tempList;
                    minMakespan = tempTime;
                }

                stopwatch.Stop();
                int[] arr = new int[perm.Count];
                perm.CopyTo(arr, 0);

                permutationMakeSpanList.Add(new PermutationMakeSpan
                {
                    permutation = String.Join(",", arr),
                    makespan = tempTime
                });
               
                stopwatch.Start();
            }

            stopwatch.Stop();
            window.PermList.ItemsSource = permutationMakeSpanList;
            return listMinMakespan;
        }

        public List<List<JobObject>> RunToTest(out Stopwatch stopwatch, LoadData data)
        {
            //zacznij liczyć czas
            stopwatch = new();
            stopwatch.Start();

            //wynik działania - dane sformatowane dla wyjścia
            List<List<JobObject>> listMinMakespan = new List<List<JobObject>>();
            int minMakespan = 0;

            //stworzenie listy do permutacji
            List<int> listToPermute = new List<int>();
            for (int i = 0; i < data.JobsQuantity; i++)
            {
                listToPermute.Add(i + 1);
            }

            //wykonanie permutacji

            foreach (var perm in Permutation.Permutate(listToPermute, listToPermute.Count))
            {
                List<List<JobObject>> tempList = new List<List<JobObject>>();

                for (int j = 0; j < data.MachinesQuantity; j++)
                {
                    tempList.Add(new List<JobObject>());
                    for (int k = 0; k < perm.Count; k++)
                    {
                        int jobIndex = (int)perm[k];
                        int startTime;
                        if (j == 0 && k == 0) //pierwsza maszyna pierwsze zadanie
                        {
                            startTime = 0;
                        }
                        else if (k == 0 && j != 0) //pierwsze zadanie kolejna maszyna
                        {
                            startTime = tempList[j - 1][k].StopTime;
                        }
                        else if (k != 0 && j == 0) //pierwsza maszyna kolejne zadania
                        {
                            startTime = tempList[j][k - 1].StopTime;
                        }
                        else //pozostałe przypadki
                        {
                            startTime = Math.Max(tempList[j - 1][k].StopTime, tempList[j][k - 1].StopTime);
                        }
                        //zapisanie zestawu danych zadanie, czas rozpoczęcia, czas zakończenia
                        int stopTime = startTime + data.Jobs[jobIndex - 1][j];
                        tempList[j].Add(new JobObject
                        {
                            JobIndex = jobIndex,
                            StartTime = startTime,
                            StopTime = stopTime
                        });
                    }
                }
                //wyznacz czas zakończenia całego cyklu
                int tempTime = tempList[data.MachinesQuantity - 1][data.JobsQuantity - 1].StopTime;

                if (minMakespan == 0) //jeśli to pierwsza permutacja przypisz dane niezależnie jakie są
                {
                    listMinMakespan = tempList;
                    minMakespan = tempTime;
                }
                if (minMakespan >= tempTime) //sprawdzenie czy istnieje mniejszy czas
                {
                    listMinMakespan = tempList;
                    minMakespan = tempTime;
                }
            }
            stopwatch.Stop();
            return listMinMakespan;
        }

    }
}
