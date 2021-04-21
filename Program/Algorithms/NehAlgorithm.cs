using System.Collections.Generic;
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

        //Klasa elementu wystepuującego na ścieżce krytycznej - numer zadania wraz z czasem jego wykonania
        public class CriticalPathElement
        {
            public int JobIndex;
            public int JobTime;

            public CriticalPathElement(int jobIndex, int jobTime)
            {
                JobIndex = jobIndex;
                JobTime = jobTime;
            }
        }

        public List<List<JobObject>> Run(out Stopwatch stopwatch, LoadData data = null)
        {
            stopwatch = new Stopwatch();

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

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
            //priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            priorityList = priorityList.OrderByDescending((x => x.Priority)).ToList();

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

        //Ta metoda wyszukuje ścieżkę krytyczną na podstawie diagramu Gantta. 
        //Zwracana jest lista elementów ścieżki
        //działanie rekurencyjne, dla nowo dodanego elementu wchodzimy i sprawdzamy podścieżki
        public List<CriticalPathElement> MakeCriticalPath(List<List<JobObject>> ganttChart, int jobLoc, int machineLoc, List<CriticalPathElement> list)
        {
            //jesli jesteś na poczatku to zwróć całą ścieżkę
            if (jobLoc == 0 && machineLoc == 0)
            {
                return list;
            }
            else
            {
                if (jobLoc == ganttChart[0].Count - 1 && machineLoc == ganttChart.Count - 1)
                {
                    JobObject temp = ganttChart[machineLoc][jobLoc];
                    list.Add(new CriticalPathElement(temp.JobIndex, temp.StopTime - temp.StartTime));
                }
                //jesli jesteśmy na środku to mamy z  lewej strony job na tej samej maszynie i od góry ten sam job na poprzedniej maszynie
                if (jobLoc != 0 && machineLoc != 0)
                {
                    JobObject jobBefore = ganttChart[machineLoc][jobLoc - 1]; //wcześniejsze zadanie
                    JobObject machineBefore = ganttChart[machineLoc - 1][jobLoc]; //to samo zadanie na poprzedniej maszynie
                    JobObject presentObject = ganttChart[machineLoc][jobLoc]; //obecne, dla którego sprawdzamy
                    if (jobBefore.StopTime == presentObject.StartTime || machineBefore.StopTime == presentObject.StartTime)
                    {
                        if (jobBefore.StopTime == presentObject.StartTime)
                        {
                            List<CriticalPathElement> tempList = list.ToList();
                            tempList.Add(new CriticalPathElement(jobBefore.JobIndex, jobBefore.StopTime - jobBefore.StartTime));
                            return MakeCriticalPath(ganttChart, jobLoc - 1, machineLoc, tempList);
                        }
                        if (machineBefore.StopTime == presentObject.StartTime)
                        {
                            List<CriticalPathElement> tempList = list.ToList();
                            tempList.Add(new CriticalPathElement(machineBefore.JobIndex, machineBefore.StopTime - machineBefore.StartTime));
                            return MakeCriticalPath(ganttChart, jobLoc, machineLoc - 1, tempList);
                        }
                    }
                    //               else 
                    //{ 
                    //	return null;
                    //}
                }
                if (machineLoc == 0 && jobLoc != 0)
                {
                    JobObject jobBefore = ganttChart[machineLoc][jobLoc - 1]; //wcześniejsze zadanie
                    JobObject presentObject = ganttChart[machineLoc][jobLoc]; //obecne, dla którego sprawdzamy
                    if (jobBefore.StopTime == presentObject.StartTime)
                    {
                        List<CriticalPathElement> tempList = list.ToList();
                        tempList.Add(new CriticalPathElement(jobBefore.JobIndex, jobBefore.StopTime - jobBefore.StartTime));
                        return MakeCriticalPath(ganttChart, jobLoc - 1, machineLoc, tempList);
                    }
                    //               else 
                    //{ 
                    //	return null;
                    //}

                }
                if (jobLoc == 0 && machineLoc != 0)
                {
                    JobObject machineBefore = ganttChart[machineLoc - 1][jobLoc]; //to samo zadanie na poprzedniej maszynie
                    JobObject presentObject = ganttChart[machineLoc][jobLoc]; //obecne, dla którego sprawdzamy
                    if (machineBefore.StopTime == presentObject.StartTime)
                    {
                        List<CriticalPathElement> tempList = list.ToList();
                        tempList.Add(new CriticalPathElement(machineBefore.JobIndex, machineBefore.StopTime - machineBefore.StartTime));
                        return MakeCriticalPath(ganttChart, jobLoc, machineLoc - 1, tempList);
                    }
                    //               else 
                    //{
                    //	return null;
                    //}
                }
                return null;
            }
        }

        public CriticalPathElement LongestOperationJob(List<CriticalPathElement> criticalPath, int jobToIgnore)
        {
            CriticalPathElement longestOperation = new CriticalPathElement(0, 0);

            foreach (CriticalPathElement elem in criticalPath)
            {
                if (longestOperation.JobTime < elem.JobTime && elem.JobIndex != jobToIgnore)
                {
                    longestOperation.JobTime = elem.JobTime;
                    longestOperation.JobIndex = elem.JobIndex;
                }
            }
            return longestOperation;
        }

        public CriticalPathElement LargestSumOperationJob(List<CriticalPathElement> criticalPath, int jobToIgnore)
        {
            List<CriticalPathElement> sumList = new List<CriticalPathElement>();

            foreach (CriticalPathElement elem in criticalPath)
            {
                if (!sumList.Any())
                {
                    sumList.Add(elem);
                }
                else
                {
                    CriticalPathElement sumElem = sumList.Find(x => x.JobIndex == elem.JobIndex);
                    if (sumElem != null)
                    {
                        sumElem.JobTime += elem.JobTime;
                    }
                    else
                    {
                        sumList.Add(elem);
                    }
                }
            }

            CriticalPathElement longestOperation = new CriticalPathElement(0, 0);

            foreach (CriticalPathElement elem in sumList)
            {
                if (longestOperation.JobTime < elem.JobTime && elem.JobIndex != jobToIgnore)
                {
                    longestOperation.JobTime = elem.JobTime;
                    longestOperation.JobIndex = elem.JobIndex;
                }
            }
            return longestOperation;
        }

        public CriticalPathElement MostOperationsJob(List<CriticalPathElement> criticalPath, int jobToIgnore)
        {
            //w polu JobTime z CriticalPathElement tym razem przypisana będzie ilość operacji zamaist czasu
            List<CriticalPathElement> sumList = new List<CriticalPathElement>();

            foreach (CriticalPathElement elem in criticalPath)
            {
                if (!sumList.Any())
                {
                    sumList.Add(new CriticalPathElement(elem.JobIndex, 1));
                }
                else
                {
                    CriticalPathElement sumElem = sumList.Find(x => x.JobIndex == elem.JobIndex);
                    if (sumElem != null)
                    {
                        sumElem.JobTime++;
                    }
                    else
                    {
                        sumList.Add(new CriticalPathElement(elem.JobIndex, 1));
                    }
                }
            }

            CriticalPathElement mostOperations = new CriticalPathElement(0, 0);

            foreach (CriticalPathElement elem in sumList)
            {
                if (mostOperations.JobTime < elem.JobTime && elem.JobIndex != jobToIgnore)
                {
                    mostOperations.JobTime = elem.JobTime;
                    mostOperations.JobIndex = elem.JobIndex;
                }
            }
            return mostOperations;
        }

        public int BiggestCmaxChangeJob(List<PriorityWithJobIndex> currentList, int jobToIgnore, LoadData data)
        {
            int shortestCMax = int.MaxValue;
            int shortestCMaxIndeks = -1;
            for (int i = 0; i < currentList.Count; i++)
            {
                PriorityWithJobIndex temp = currentList[i];
                if (temp.JobIndex != jobToIgnore)
                {
                    currentList.RemoveAt(i);
                    int permutationCmax = MakeGanttChart(currentList, data).Last().Last().StopTime;
                    if (permutationCmax < shortestCMax)
                    {
                        shortestCMax = permutationCmax;
                        shortestCMaxIndeks = i;
                    }
                    currentList.Insert(i, temp);
                }
            }

            return shortestCMaxIndeks;
        }
        public List<List<JobObject>> RunModification1(out Stopwatch stopwatch, LoadData data = null)
        {
            stopwatch = new Stopwatch();

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

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
            //priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            priorityList = priorityList.OrderByDescending((x => x.Priority)).ToList();

            //Lista która jest aktualnym ułożeniem zadań (np. 2,1,4,3)
            List<PriorityWithJobIndex> outputList = new List<PriorityWithJobIndex>();

            for (int i = 0; i < data.JobsQuantity; i++)
            {
                //Zmienne, które przydadzą się przy znalezieniu najkrótszej permutacji (chyba można to nazwać permutacją)
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
                if (outputList.Count > 1)
                {
                    List<List<JobObject>> ganttData = MakeGanttChart(outputList, data);

                    List<CriticalPathElement> criticalPath = new List<CriticalPathElement>();
                    criticalPath = MakeCriticalPath(ganttData, ganttData[0].Count - 1, ganttData.Count - 1, criticalPath);

                    CriticalPathElement longestOperation = LongestOperationJob(criticalPath, priorityList[i].JobIndex);

                    shortestCMax = ganttData.Last().Last().StopTime;

                    int indexLogestOperation = outputList.IndexOf(outputList.Find(x => x.JobIndex == longestOperation.JobIndex - 1));
                    outputList.RemoveAt(indexLogestOperation);
                    shortestCMaxIndeks = indexLogestOperation;

                    //Tutaj wykonujemy wstawianie. Dla x-tego zadania na x-tych różnych pozycjach - krok 5 
                    for (int j = 0; j < outputList.Count + 1; j++)
                    {
                        outputList.Insert(j, new PriorityWithJobIndex(0, longestOperation.JobIndex - 1)); //make 0 for priority

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
                    outputList.Insert(shortestCMaxIndeks, new PriorityWithJobIndex(0, longestOperation.JobIndex - 1));
                }
            }

            stopwatch.Stop();
            return MakeGanttChart(outputList, data);
        }

        public List<List<JobObject>> RunModification2(out Stopwatch stopwatch, LoadData data = null)
        {
            stopwatch = new Stopwatch();

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

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
            //priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            priorityList = priorityList.OrderByDescending((x => x.Priority)).ToList();

            //Lista która jest aktualnym ułożeniem zadań (np. 2,1,4,3)
            List<PriorityWithJobIndex> outputList = new List<PriorityWithJobIndex>();

            for (int i = 0; i < data.JobsQuantity; i++)
            {
                //Zmienne, które przydadzą się przy znalezieniu najkrótszej permutacji (chyba można to nazwać permutacją)
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
                if (outputList.Count > 1)
                {
                    List<List<JobObject>> ganttData = MakeGanttChart(outputList, data);

                    List<CriticalPathElement> criticalPath = new List<CriticalPathElement>();
                    criticalPath = MakeCriticalPath(ganttData, ganttData[0].Count - 1, ganttData.Count - 1, criticalPath);

                    CriticalPathElement longestOperation = LargestSumOperationJob(criticalPath, priorityList[i].JobIndex);

                    shortestCMax = ganttData.Last().Last().StopTime;

                    int indexLogestOperation = outputList.IndexOf(outputList.Find(x => x.JobIndex == longestOperation.JobIndex - 1));
                    outputList.RemoveAt(indexLogestOperation);
                    shortestCMaxIndeks = indexLogestOperation;

                    //Tutaj wykonujemy wstawianie. Dla x-tego zadania na x-tych różnych pozycjach - krok 5 
                    for (int j = 0; j < outputList.Count + 1; j++)
                    {
                        outputList.Insert(j, new PriorityWithJobIndex(0, longestOperation.JobIndex - 1)); //make 0 for priority

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
                    outputList.Insert(shortestCMaxIndeks, new PriorityWithJobIndex(0, longestOperation.JobIndex - 1));
                }
            }

            stopwatch.Stop();
            return MakeGanttChart(outputList, data);
        }

        public List<List<JobObject>> RunModification3(out Stopwatch stopwatch, LoadData data = null)
        {
            stopwatch = new Stopwatch();

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

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
            //priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            priorityList = priorityList.OrderByDescending((x => x.Priority)).ToList();

            //Lista która jest aktualnym ułożeniem zadań (np. 2,1,4,3)
            List<PriorityWithJobIndex> outputList = new List<PriorityWithJobIndex>();

            for (int i = 0; i < data.JobsQuantity; i++)
            {
                //Zmienne, które przydadzą się przy znalezieniu najkrótszej permutacji (chyba można to nazwać permutacją)
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
                if (outputList.Count > 1)
                {
                    List<List<JobObject>> ganttData = MakeGanttChart(outputList, data);

                    List<CriticalPathElement> criticalPath = new List<CriticalPathElement>();
                    criticalPath = MakeCriticalPath(ganttData, ganttData[0].Count - 1, ganttData.Count - 1, criticalPath);

                    CriticalPathElement longestOperation = MostOperationsJob(criticalPath, priorityList[i].JobIndex);

                    shortestCMax = ganttData.Last().Last().StopTime;

                    int indexLogestOperation = outputList.IndexOf(outputList.Find(x => x.JobIndex == longestOperation.JobIndex - 1));
                    outputList.RemoveAt(indexLogestOperation);
                    shortestCMaxIndeks = indexLogestOperation;

                    //Tutaj wykonujemy wstawianie. Dla x-tego zadania na x-tych różnych pozycjach - krok 5 
                    for (int j = 0; j < outputList.Count + 1; j++)
                    {
                        outputList.Insert(j, new PriorityWithJobIndex(0, longestOperation.JobIndex - 1)); //make 0 for priority

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
                    outputList.Insert(shortestCMaxIndeks, new PriorityWithJobIndex(0, longestOperation.JobIndex - 1));
                }
            }

            stopwatch.Stop();
            return MakeGanttChart(outputList, data);
        }

        public List<List<JobObject>> RunModification4(out Stopwatch stopwatch, LoadData data = null)
        {
            stopwatch = new Stopwatch();

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

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
            //priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            priorityList = priorityList.OrderByDescending((x => x.Priority)).ToList();

            //Lista która jest aktualnym ułożeniem zadań (np. 2,1,4,3)
            List<PriorityWithJobIndex> outputList = new List<PriorityWithJobIndex>();

            for (int i = 0; i < data.JobsQuantity; i++)
            {
                //Zmienne, które przydadzą się przy znalezieniu najkrótszej permutacji (chyba można to nazwać permutacją)
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

                if (outputList.Count > 1)
                {

                    int indexOnList = BiggestCmaxChangeJob(outputList, priorityList[i].JobIndex, data);
                    int jobIndexOfBiggestCmaxChange = outputList[indexOnList].JobIndex;
                    shortestCMaxIndeks = -1;
                    shortestCMax = int.MaxValue;
                    outputList.RemoveAt(indexOnList);

                    //Tutaj wykonujemy wstawianie. Dla x-tego zadania na x-tych różnych pozycjach - krok 5 
                    for (int j = 0; j < outputList.Count + 1; j++)
                    {
                        outputList.Insert(j, new PriorityWithJobIndex(0, jobIndexOfBiggestCmaxChange)); //make 0 for priority

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
                    outputList.Insert(shortestCMaxIndeks, new PriorityWithJobIndex(0, jobIndexOfBiggestCmaxChange));
                }
            }

            stopwatch.Stop();
            return MakeGanttChart(outputList, data);
        }

        public List<int> RunWithoutGantt(out Stopwatch stopwatch, LoadData data = null)
        {
            stopwatch = new Stopwatch();

            //Wczytywanie danych
            if (data == null)
            {
                data = new LoadData();
                data.ReadFromFile();
            }

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
            //priorityList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            priorityList = priorityList.OrderByDescending((x => x.Priority)).ToList();

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
            List<int> returnList = new List<int>();
            foreach(var x in outputList)
            {
                returnList.Add(x.JobIndex);
            }
            return returnList;
        }
    }
}
