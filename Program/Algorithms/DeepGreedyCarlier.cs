using SPD1.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPD1.Algorithms
{
    class DeepGreedyCarlier
    {
        public int Cmax = int.MaxValue;
        public List<RPQJob> bestSolution = new List<RPQJob>();
        public void Solve(List<RPQJob> inputList, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            bestSolution = Schrage.Solve(inputList, out Cmax, out Stopwatch stopwatch1);
            Solve(inputList.ToList(), bestSolution, Cmax);

            stopwatch.Stop();
        }
        private void Solve(List<RPQJob> inputList, List<RPQJob> newSolution, int newCmax)
        {
            if (newCmax < Cmax)
            {
                bestSolution = newSolution;
                Cmax = newCmax;
            }

            RPQJob b = Carlier.getJobB(newSolution, newCmax);
            RPQJob a = Carlier.getJobA(newSolution, newCmax, b);
            RPQJob c = Carlier.getJobC(newSolution, a, b);
            if (c.JobIndex == -1)
                return;

            int indexOfJobCNeighbour = newSolution.IndexOf(c) + 1;
            int count = newSolution.IndexOf(b) - indexOfJobCNeighbour + 1;
            List<RPQJob> Klist = newSolution.GetRange(indexOfJobCNeighbour, count);
            int minimumPreparationTime = Klist.Aggregate((current, x) => current.PreparationTime > x.PreparationTime ? x : current).PreparationTime; //najmniejszy czas przygotowania
            int minimumDeliveryTime = Klist.Aggregate((current, x) => current.DeliveryTime > x.DeliveryTime ? x : current).DeliveryTime; //najmniejszy czas dostarczenia 
            int sumOfWorkTimes = Carlier.sumWorkTimes(Klist); //suma czasów wykonania
            int sumTime = minimumPreparationTime + minimumDeliveryTime + sumOfWorkTimes; //h dla listy K bez C

            RPQJob job = inputList.Find(x => x.JobIndex == c.JobIndex);
            int jobIndexInList = inputList.IndexOf(job);

            int originalPreparationTime = job.PreparationTime; //zmienna tymczasowa
            int modifiedPreparationTime = Math.Max(c.PreparationTime, minimumPreparationTime + sumOfWorkTimes);
            int originalDeliveryTime = c.DeliveryTime;
            int modifiedDeliveryTime = Math.Max(c.DeliveryTime, minimumDeliveryTime + sumOfWorkTimes); //podmiana wartości w zadaniu c

            job.PreparationTime = modifiedPreparationTime;
            inputList[jobIndexInList] = job;

            List<RPQJob> leftSolution = Schrage.SolveUsingQueue(inputList, out int leftCmax, out Stopwatch stopwatch1);

            job.PreparationTime = originalPreparationTime;
            job.DeliveryTime = modifiedDeliveryTime;
            inputList[jobIndexInList] = job;

            List<RPQJob> rigthSolution = Schrage.SolveUsingQueue(inputList, out int rigthCmax, out Stopwatch stopwatch2);

            bool wentLeft = false;
            bool wentRight = false;
            if (leftCmax < rigthCmax && leftCmax < newCmax)
            {
                job.DeliveryTime = originalDeliveryTime;
                job.PreparationTime = modifiedPreparationTime;
                inputList[jobIndexInList] = job;

                Solve(inputList, leftSolution, leftCmax);
                wentLeft = true;
            }
            else if (rigthCmax < leftCmax && rigthCmax < newCmax)
            {
                Solve(inputList, rigthSolution, rigthCmax);
                wentRight = true;
            }
            else if (leftCmax == newCmax)
            {
                job.DeliveryTime = originalDeliveryTime;
                job.PreparationTime = modifiedPreparationTime;
                inputList[jobIndexInList] = job;

                Solve(inputList, leftSolution, leftCmax);
                wentLeft = true;
            }
            else if (rigthCmax == newCmax)
            {
                Solve(inputList, rigthSolution, rigthCmax);
                wentRight = true;
            }

            if (!wentLeft)
            {
                job.DeliveryTime = originalDeliveryTime;
                job.PreparationTime = modifiedPreparationTime;
                inputList[jobIndexInList] = job;
                Solve(inputList, leftSolution, leftCmax);
            }
            if (!wentRight)
                Solve(inputList, rigthSolution, rigthCmax);
        }
    }
}
