using SPD1.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SPD1.Algorithms
{
    class Carlier
    {
        public int upperBoundary = int.MaxValue;
        public int lowerBoundary = 0;
        public int Cmax = int.MaxValue;
        public List<RPQJob> bestSolution = new List<RPQJob>();

        /// <summary>
        /// Funkcja ustawiająca pola klasy. Wykonać przed wykonaniem Solve() lub SolveUsingQueue()
        /// </summary>
        public void ClearBeforeRun()
        {
            upperBoundary = int.MaxValue;
            lowerBoundary = 0;
            Cmax = int.MaxValue;
            bestSolution = new List<RPQJob>();
        }
        public List<RPQJob> Solve(List<RPQJob> input, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            List<RPQJob> newSolution = Schrage.Solve(input, out int newCmax, out Stopwatch stopwatch1);
            if (newCmax < this.upperBoundary)
            {
                upperBoundary = newCmax;
                bestSolution = newSolution;
                Cmax = newCmax;
            }
            RPQJob b = getJobB(newSolution, newCmax);
            RPQJob a = getJobA(newSolution, newCmax, b);
            RPQJob c = getJobC(newSolution, a, b);
            if (c.JobIndex == -1)
            {
                stopwatch.Stop();
                return bestSolution;
            }
            int nextToCIndexInList = newSolution.IndexOf(c) + 1;
            int count = newSolution.IndexOf(b) - nextToCIndexInList + 1;
            List<RPQJob> Klist = newSolution.GetRange(nextToCIndexInList, count);
            int minPrepTime = Klist.Aggregate((current, x) => current.PreparationTime > x.PreparationTime ? x : current).PreparationTime; //najmniejszy czas przygotowania
            int minDelivTime = Klist.Aggregate((current, x) => current.DeliveryTime > x.DeliveryTime ? x : current).DeliveryTime; //najmniejszy czas dostarczenia 
            int workTimes = sumWorkTimes(Klist); //suma czasów wykonania
            int sumTime = minPrepTime + minDelivTime + workTimes; //h dla listy K bez C

            RPQJob cJobInInput = input.Find(x => x.JobIndex == c.JobIndex);
            int tempIndex = input.IndexOf(cJobInInput);
            int cPrepTimeTemp = cJobInInput.PreparationTime; //zmienna tymczasowa
            cJobInInput.PreparationTime = Math.Max(c.PreparationTime, minPrepTime + workTimes);
            input[tempIndex] = cJobInInput;

            lowerBoundary = SchragePMTN.Solve(input.ToList(), out Stopwatch stopwatch2);

            int minPrepTimeWithC = (cJobInInput.PreparationTime < minPrepTime) ? cJobInInput.PreparationTime : minPrepTime;
            int minDelivTimeWithC = (cJobInInput.DeliveryTime < minDelivTime) ? cJobInInput.DeliveryTime : minDelivTime;
            int sumWithC = minPrepTimeWithC + minDelivTimeWithC + workTimes + cJobInInput.WorkTime; //h dla listy K z C

            lowerBoundary = Math.Max(sumTime, Math.Max(sumWithC, lowerBoundary));
            if (lowerBoundary < upperBoundary)
            {
                Solve(input.ToList(), out Stopwatch stopwatch3);
            }
            cJobInInput.PreparationTime = cPrepTimeTemp;
            input[tempIndex] = cJobInInput;
            //c.PreparationTime = cPrepTimeTemp;

            int cDelivTimeTemp = c.DeliveryTime;
            cJobInInput.DeliveryTime = Math.Max(c.DeliveryTime, minDelivTime + workTimes); //podmiana wartości w zadaniu c
            input[tempIndex] = cJobInInput;
            lowerBoundary = SchragePMTN.Solve(input.ToList(), out stopwatch2);

            minPrepTimeWithC = (cJobInInput.PreparationTime < minPrepTime) ? cJobInInput.PreparationTime : minPrepTime;
            minDelivTimeWithC = (cJobInInput.DeliveryTime < minDelivTime) ? cJobInInput.DeliveryTime : minDelivTime;
            sumWithC = minPrepTimeWithC + minDelivTimeWithC + workTimes + cJobInInput.WorkTime; //h dla listy K z C

            lowerBoundary = Math.Max(sumTime, Math.Max(sumWithC, lowerBoundary));
            if (lowerBoundary < upperBoundary)
            {
                Solve(input.ToList(), out Stopwatch stopwatch3);
            }
            cJobInInput.DeliveryTime = cDelivTimeTemp;
            input[tempIndex] = cJobInInput;
            return null;
        }
        public List<RPQJob> SolveUsingQueue(List<RPQJob> input, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            List<RPQJob> newSolution = Schrage.SolveUsingQueue(input, out int newCmax);
            if (newCmax < this.upperBoundary)
            {
                upperBoundary = newCmax;
                bestSolution = newSolution;
                Cmax = newCmax;
            }
            RPQJob b = getJobB(newSolution, newCmax);
            RPQJob a = getJobA(newSolution, newCmax, b);
            RPQJob c = getJobC(newSolution, a, b);
            if (c.JobIndex == -1)
            {
                stopwatch.Stop();
                return bestSolution;
            }
            int nextToCIndexInList = newSolution.IndexOf(c) + 1;
            int count = newSolution.IndexOf(b) - nextToCIndexInList + 1;
            List<RPQJob> Klist = newSolution.GetRange(nextToCIndexInList, count);
            int minPrepTime = Klist.Aggregate((current, x) => current.PreparationTime > x.PreparationTime ? x : current).PreparationTime; //najmniejszy czas przygotowania
            int minDelivTime = Klist.Aggregate((current, x) => current.DeliveryTime > x.DeliveryTime ? x : current).DeliveryTime; //najmniejszy czas dostarczenia 
            int workTimes = sumWorkTimes(Klist); //suma czasów wykonania
            int sumTime = minPrepTime + minDelivTime + workTimes; //h dla listy K bez C

            RPQJob cJobInInput = input.Find(x => x.JobIndex == c.JobIndex);
            int tempIndex = input.IndexOf(cJobInInput);
            int cPrepTimeTemp = cJobInInput.PreparationTime; //zmienna tymczasowa
            cJobInInput.PreparationTime = Math.Max(c.PreparationTime, minPrepTime + workTimes);
            input[tempIndex] = cJobInInput;
            lowerBoundary = SchragePMTN.SolveUsingQueue(input.ToList());

            int minPrepTimeWithC = (cJobInInput.PreparationTime < minPrepTime) ? cJobInInput.PreparationTime : minPrepTime;
            int minDelivTimeWithC = (cJobInInput.DeliveryTime < minDelivTime) ? cJobInInput.DeliveryTime : minDelivTime;
            int sumWithC = minPrepTimeWithC + minDelivTimeWithC + workTimes + cJobInInput.WorkTime; //h dla listy K z C
            lowerBoundary = Math.Max(sumTime, Math.Max(sumWithC, lowerBoundary));
            if (lowerBoundary < upperBoundary)
            {
                SolveUsingQueue(input.ToList(), out Stopwatch stopwatch3);
            }
            cJobInInput.PreparationTime = cPrepTimeTemp;
            input[tempIndex] = cJobInInput;
            //c.PreparationTime = cPrepTimeTemp;

            int cDelivTimeTemp = c.DeliveryTime;
            cJobInInput.DeliveryTime = Math.Max(c.DeliveryTime, minDelivTime + workTimes); //podmiana wartości w zadaniu c
            input[tempIndex] = cJobInInput;
            lowerBoundary = SchragePMTN.SolveUsingQueue(input.ToList());

            minPrepTimeWithC = (cJobInInput.PreparationTime < minPrepTime) ? cJobInInput.PreparationTime : minPrepTime;
            minDelivTimeWithC = (cJobInInput.DeliveryTime < minDelivTime) ? cJobInInput.DeliveryTime : minDelivTime;
            sumWithC = minPrepTimeWithC + minDelivTimeWithC + workTimes + cJobInInput.WorkTime; //h dla listy K z C

            lowerBoundary = Math.Max(sumTime, Math.Max(sumWithC, lowerBoundary));
            if (lowerBoundary < upperBoundary)
            {
                SolveUsingQueue(input.ToList(), out Stopwatch stopwatch3);
            }
            cJobInInput.DeliveryTime = cDelivTimeTemp;
            input[tempIndex] = cJobInInput;
            return null;
        }
        /// <summary>
        /// Zwaca zadanie i, którego czas dostarczenia na wykresie == cmax
        /// </summary>
        /// <param name="list">lista zadań</param>
        /// <param name="cmax">wartość Cmax</param>
        /// <returns>Zadanie i, takie że: ChartDeliveryTime(i) == cmax</returns>
        public static RPQJob getJobB(List<RPQJob> list, int cmax)
        {
            List<RPQChartData> chartData = RPQChart.MakeRPQChart(list); //wykonaj wykres
            for (int i = chartData.Count - 1; i >= 0; i--) //przszukuj od tyłu tego co ma sumę równą cmax
            {
                if (cmax == chartData[i].DeliveryTime) //jak znajdziesz to zwróć
                    return list[i];
            }
            return new RPQJob
            {
                JobIndex = -1
            };//jak nie znajdziesz to zwróć job o indeksie -1
        }
        /// <summary>
        /// Zwraca zadanie i, którego suma: czas przygotowania zadania i + suma czasów wykonania 
        /// zadań z przedziału:<i,b> + czas dostarczernia zadania b = cmax
        /// </summary>
        /// <param name="list">lista zadań</param>
        /// <param name="cmax">wartość Cmax</param>
        /// <param name="jobB">Zadanie B</param>
        /// <returns>Zadanie i, takie że r(i)+sumWorkTimes(i,B)+q(B) = cmax</returns>
        public static RPQJob getJobA(List<RPQJob> list, int cmax, RPQJob jobB)
        {
            int count = list.Count;
            int jobBindex = list.IndexOf(jobB);
            for (int i = 0; i < count; i++)
            {
                RPQJob job = list[i];
                if (cmax == (job.PreparationTime + sumWorkTimes(list, job, i, jobB, jobBindex) + jobB.DeliveryTime))
                {
                    return job;//jak znajdziesz to zwróć
                }
            }
            return new RPQJob
            {
                JobIndex = -1
            };//jak nie znajdziesz to zwróć job o indeksie -1
        }
        /// <summary>
        /// funkcja zwracająca sumę czasów wykonania zadań z przedziału od zadania startJob do zadania endJob
        /// </summary>
        /// <param name="list">lista z zadaniami</param>
        /// <param name="startJob">zadanie początkowe</param>
        /// <param name="endJob">zadanie końcowe</param>
        /// <returns>suma czasów wykonania</returns>
        public static int sumWorkTimes(List<RPQJob> list, RPQJob startJob, int startIndex, RPQJob endJob, int endIndex)
        {
            int time = 0;
            List<RPQJob> temp = new List<RPQJob>();
            if (startIndex < endIndex)
            {
                int count = endIndex - startIndex + 1;
                temp = list.GetRange(startIndex, count);
            }
            else
            {
                int count = startIndex - endIndex + 1;
                temp = list.GetRange(endIndex, count);
            }
            time = sumWorkTimes(temp);
            return time;
        }
        /// <summary>
        /// Zwraca pierwsze zadanie, które ma mniejszy czas dostarczenia od zadania jobB i jest jemu najbliższe. 
        /// Zadania przeszukiwane są na przedziale <jobA,jobB) od tyłu.
        /// </summary>
        /// <param name="list">lista zadań</param>
        /// <param name="jobA">zadanie początkowe</param>
        /// <param name="jobB">zadanie kończące</param>
        /// <returns></returns>
        public static RPQJob getJobC(List<RPQJob> list, RPQJob jobA, RPQJob jobB)
        {
            //zacznij od zadania bezpośrednio przed zadaniem B, zacznij od tyłu
            int startIndex = list.IndexOf(jobB) - 1;
            int endIndex = list.IndexOf(jobA);
            if (startIndex > endIndex)
            {
                for (int i = startIndex; i >= endIndex; i--)
                {
                    if (list[i].DeliveryTime < jobB.DeliveryTime)
                    {
                        return list[i];
                    }
                }
            }
            else
            {
                for (int i = endIndex; i >= startIndex; i--)
                {
                    if (list[i].DeliveryTime < jobB.DeliveryTime)
                    {
                        return list[i];
                    }
                }
            }

            return new RPQJob
            {
                JobIndex = -1
            };//jak nie znajdziesz to zwróć job o indeksie -1
        }

        /// <summary>
        /// funkcja zwracająca sumę czasów wykonania zadań z listy list
        /// </summary>
        /// <param name="list">lista z zadaniami</param>
        /// <returns>suma czasów wykonania</returns>
        public static int sumWorkTimes(List<RPQJob> list)
        {
            int time = 0;
            foreach (RPQJob job in list)
            {
                time += job.WorkTime; //dodaj czasy wykonania
            }
            return time;
        }
    }
}
