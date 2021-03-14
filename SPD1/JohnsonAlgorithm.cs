using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPD1
{
	public class JohnsonAlgorithm
	{
		/// <summary>
		/// Uruchamia algorytm na danych z pliku i zwraca dane sformatowane do wizualizacji
		/// </summary>
		public List<List<JobObject>> Run()
		{
			//Wczytuje dane z pliku
			LoadData data = new LoadData();
			data.ReadFromFile();

			//Konwertuje dane z pliku
			List<List<int>> dataToConvert = new List<List<int>>();
			for(int i = 0; i < data.MachinesQuantity; i++)
			{
				dataToConvert.Add(new List<int>());
				for(int j = 0; j < data.JobsQuantity; j++)
				{
					dataToConvert[i].Add(data.Jobs[j][i]);
				}
			}
			data.Jobs = dataToConvert;

			return Algorithm(data);
		}
		/// <summary>
		/// Generuje losowe dane wejściowe dla algorytmu
		/// </summary>
		public LoadData GenerateRandomInputData(int machinesQuantity, int JobsQuantity, int maxValue)
		{
			Random rand = new Random();
			LoadData data = new LoadData();
			data.MachinesQuantity = machinesQuantity;
			data.JobsQuantity = JobsQuantity;
			data.Jobs = new List<List<int>>();
			for (int j = 0; j < data.MachinesQuantity; j++)
			{
				data.Jobs.Add(new List<int>());
				for (int i = 0; i < data.JobsQuantity; i++)
				{
					data.Jobs[j].Add(rand.Next() % maxValue + 1);
				}
			}

			return data;
		}

		/// <summary>
		/// Jeśli jest to problem wielomaszynowy zamienia dane wejściowe problemu dwumaszynowego
		/// </summary>
		public LoadData ConvertIntoTwoMachineData(LoadData dataToConvert)
		{
			LoadData newData = new LoadData();
			newData.MachinesQuantity = 2;
			newData.JobsQuantity = dataToConvert.JobsQuantity;
			newData.Jobs = new List<List<int>>();
			newData.Jobs.Add(new List<int>());
			newData.Jobs.Add(new List<int>());

			if (dataToConvert.MachinesQuantity % 2 == 0)
			{
				int halfIndex = dataToConvert.MachinesQuantity / 2;
				for (int i = 0; i < dataToConvert.JobsQuantity; i++)
				{
					newData.Jobs[0].Add(0);
					newData.Jobs[1].Add(0);

					for (int j = 0; j < halfIndex; j++)
					{
						newData.Jobs[0][i] += dataToConvert.Jobs[j][i];
					}
					for (int j = halfIndex; j < dataToConvert.MachinesQuantity; j++)
					{
						newData.Jobs[1][i] += dataToConvert.Jobs[j][i];
					}
				}
			}
			else
			{
				int halfIndex = dataToConvert.MachinesQuantity / 2;
				for (int i = 0; i < dataToConvert.JobsQuantity; i++)
				{
					newData.Jobs[0].Add(0);
					newData.Jobs[1].Add(0);

					for (int j = 0; j <= halfIndex; j++)
					{
						newData.Jobs[0][i] += dataToConvert.Jobs[j][i];
					}
					for (int j = halfIndex; j < dataToConvert.MachinesQuantity; j++)
					{
						newData.Jobs[1][i] += dataToConvert.Jobs[j][i];
					}
				}
			}
			return newData;
		}

		/// <summary>
		/// Zwraca CMax dla danych wyjściowych
		/// </summary>
		public int GetCMax(List<List<JobObject>> jobObjects)
		{
			return jobObjects.Last().Last().StopTime;
		}

		/// <summary>
		/// Algorythm Johnsona
		/// </summary>
		public List<List<JobObject>> Algorithm(LoadData data)
		{
			LoadData oryginalData = data;

			//Sprawdza czy zadane są więcej niż dwie maszyny
			//Jeśli tak to robi z tego dane dla dwóch maszyn
			if (data.MachinesQuantity > 2)
			{
				data = ConvertIntoTwoMachineData(data);
			}

			//Listy pomocnicze do operowania na danych
			List<int> outputIndex = new List<int>();
			List<int> usedIndex = new List<int>();
			for (int i = 0; i < data.JobsQuantity; i++)
			{
				outputIndex.Add(-1);
				usedIndex.Add(-1);
			}

			//Główna pętla algorytmu
			while (usedIndex.Contains(-1))
			{
				//Znajdź najkrótsze zadanie
				int[] shortestJobTime = new int[] { int.MaxValue, int.MaxValue };
				int[] shortstestJobIdex = new int[] { 0, 0 };
				for (int j = 0; j < 2; j++)
				{
					for (int i = 0; i < data.Jobs[j].Count; i++)
					{
						//Pomiń to zadanie jeśli już było wzięte pod uwagę
						if (usedIndex[i] == 0)
							continue;
						//Znajdź najkrótsze zadanie
						if (shortestJobTime[j] > data.Jobs[j][i])
						{
							shortestJobTime[j] = data.Jobs[j][i];
							shortstestJobIdex[j] = i;
						}
					}
				}

				//Zapis indeks najkrótszego zadania
				if (shortestJobTime[0] <= shortestJobTime[1])
				{
					for (int i = 0; i < outputIndex.Count; i++)
					{
						if (outputIndex[i] == -1)
						{
							outputIndex[i] = shortstestJobIdex[0];
							usedIndex[shortstestJobIdex[0]] = 0;
							break;
						}
					}
				}
				else
				{
					for (int i = outputIndex.Count - 1; i >= 0; i--)
					{
						if (outputIndex[i] == -1)
						{
							outputIndex[i] = shortstestJobIdex[1];
							usedIndex[shortstestJobIdex[1]] = 0;
							break;
						}
					}
				}
			}

			//Jeśli był to problem wielomaszynowy podstaw oryginalne dane
			data = oryginalData;

			//Formatowanie wyjścia
			List<List<JobObject>> listOfJobs = new List<List<JobObject>>();
			for (int i = 0; i < data.MachinesQuantity; i++)
			{
				listOfJobs.Add(new List<JobObject>());

				int delay = 0;
				int jobTimeSum = 0;
				if (i != 0)
				{
					for (int k = i - 1; k >= 0; k--)
					{
						delay += listOfJobs[k][0].StopTime - listOfJobs[k][0].StartTime;
					}
				}

				if (i == 0)
				{
					for (int j = 0; j < data.JobsQuantity; j++)
					{
						listOfJobs[i].Add(new JobObject
						{
							JobIndex = outputIndex[j],
							StartTime = delay + jobTimeSum,
							StopTime = delay + jobTimeSum + data.Jobs[i][outputIndex[j]]
						});
						jobTimeSum += listOfJobs[i][j].StopTime - listOfJobs[i][j].StartTime;
					}
				}
				else
				{
					for (int j = 0; j < data.JobsQuantity; j++)
					{
						JobObject job = new JobObject();
						job.JobIndex = outputIndex[j];
						job.StartTime = delay + jobTimeSum;
						job.StopTime = delay + jobTimeSum + data.Jobs[i][outputIndex[j]];

						//Jeśli zadanie na poprzedniej maszynie nie jest skończone to przesuń do momentu zakończenia
						if (job.StartTime < listOfJobs[i - 1][j].StopTime)
						{
							job.StartTime = listOfJobs[i - 1][j].StopTime;
							job.StopTime = job.StartTime + data.Jobs[i][outputIndex[j]];
						}

						listOfJobs[i].Add(job);
						jobTimeSum = listOfJobs[i][j].StopTime - delay;
					}
				}
			}
			return listOfJobs;
		}
	}
}

