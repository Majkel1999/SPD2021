﻿using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SPD1
{
	public class NehAlgorithm
	{
		//Klasa żeby łatwiej można było operwać na prorytetach zadań i pamiętać ich i indeksy
		public class PriorityWithJobIndex
		{
			public int Priority;
			public int JobIndex;

			public PriorityWithJobIndex(int priority, int jobIndex)
			{
				Priority = priority;
				JobIndex = jobIndex;
			}
		}

		public List<List<JobObject>> Run(out Stopwatch stopwatch)
		{
			stopwatch = new Stopwatch();

			//Wczytywanie danych
			LoadData data = new LoadData();
			data.ReadFromFile();

			stopwatch.Start();

			//Tworzenie listy zadań z priorytetami (jeden priorytet = suma czasów wykonania danego zadania na każdej z maszyn)
			List<PriorityWithJobIndex> priorityList = new List<PriorityWithJobIndex>();

			for (int i = 0; i < data.JobsQuantity; i++)
			{
				//Dodaj do listy nowy priorytet z indeksem zadania
				priorityList.Add(new PriorityWithJobIndex(0, i));
				for (int j = 0; j < data.MachinesQuantity; j++)
				{
					//Dla każdego zadania zsumuj czas z każdej maszyny dla tego zadania
					priorityList[i].Priority += data.Jobs[i][j];
				}
			}

			//Posortuj je nierosnąco (malejąco)
			priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));

			//Lista która jest aktualnym ułożeniem zadań (np. 2,1,4,3)
			List<PriorityWithJobIndex> outputList = new List<PriorityWithJobIndex>();

			for (int i = 0; i < data.JobsQuantity; i++)
			{
				//Zmienne które przydadzą się przy znalezieniu najkrótszej permutacji (chyba można to nazwać permutacją)
				int shortestCMax = int.MaxValue;
				int shortestCMaxIndeks = -1;

				//Tutaj wykonujemy wstawianie. Dla j-tego zadania na j-tych różnych pozycjach
				for (int j = 0; j < outputList.Count + 1; j++)
				{
					//Wstawianie
					outputList.Insert(j, new PriorityWithJobIndex(priorityList[i].Priority, priorityList[i].JobIndex));

					int permutationCmax = MakeGanttChart(outputList, data).Last().Last().StopTime;

					//Jeśli ten Cmax jest mniejszy niż poprzednie to go zapisz
					if (shortestCMax > permutationCmax)
					{
						shortestCMaxIndeks = j;
						shortestCMax = permutationCmax;
					}

					//Usuwamy wstawiony element, żeby wypróbować kolejną permutację
					outputList.RemoveAt(j);
				}
				//Wstaw tak żeby Cmax był najmniejszy
				outputList.Insert(shortestCMaxIndeks, new PriorityWithJobIndex(priorityList[i].Priority, priorityList[i].JobIndex));
			}

			stopwatch.Stop();

			return MakeGanttChart(outputList, data);
		}

		//Ta funkcja formatuje dane żeby utworzyły diagram Gantta. W ten sposób również można odczytać Cmax
		//Zapożyczone z Johnsona z drobnymi poprawkami wynikającymi z formatu danych wejścowych
		private List<List<JobObject>> MakeGanttChart(List<PriorityWithJobIndex> list, LoadData data)
		{
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
					for (int j = 0; j < list.Count; j++)
					{
						listOfJobs[i].Add(new JobObject
						{
							JobIndex = list[j].JobIndex + 1,
							StartTime = delay + jobTimeSum,
							StopTime = delay + jobTimeSum + data.Jobs[list[j].JobIndex][i]
						});
						jobTimeSum += listOfJobs[i][j].StopTime - listOfJobs[i][j].StartTime;
					}
				}
				else
				{
					for (int j = 0; j < list.Count; j++)
					{
						JobObject job = new JobObject();
						job.JobIndex = list[j].JobIndex + 1;
						job.StartTime = delay + jobTimeSum;
						job.StopTime = delay + jobTimeSum + data.Jobs[list[j].JobIndex][i];

						//Jeśli zadanie na poprzedniej maszynie nie jest skończone to przesuń do momentu zakończenia
						if (job.StartTime < listOfJobs[i - 1][j].StopTime)
						{
							job.StartTime = listOfJobs[i - 1][j].StopTime;
							job.StopTime = job.StartTime + data.Jobs[list[j].JobIndex][i];
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