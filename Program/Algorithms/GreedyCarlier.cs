using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SPD1.Misc;

namespace SPD1.Algorithms
{
    public class GreedyCarlier
    {
        public int Cmax = int.MaxValue;
        public List<RPQJob> bestSolution = new List<RPQJob>();

        private bool m_isDeep = false;

        public GreedyCarlier(bool isSearchingDeep = false)
        {
            m_isDeep = isSearchingDeep;
        }

        public void Solve(List<RPQJob> inputList, out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            bestSolution = Schrage.Solve(inputList, out Cmax, out Stopwatch stopwatch1);
            Solve(inputList.ToList());

            stopwatch.Stop();
        }

        private void Solve(List<RPQJob> inputList)
        {
            List<RPQJob> newSolution = Schrage.SolveUsingQueue(inputList, out int newCmax);
            if (newCmax <= Cmax)
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

            int leftCmax = SchragePMTN.SolveUsingQueue(inputList);

            job.PreparationTime = originalPreparationTime;
            job.DeliveryTime = modifiedDeliveryTime;
            inputList[jobIndexInList] = job;

            int rigthCmax = SchragePMTN.SolveUsingQueue(inputList);

            bool wentLeft = false;
            bool wentRigth = false;

            if (leftCmax <= rigthCmax && leftCmax < newCmax)
            {
                job.DeliveryTime = originalDeliveryTime;
                job.PreparationTime = modifiedPreparationTime;
                inputList[jobIndexInList] = job;
                wentLeft = true;
                Solve(inputList);
            }
            else if (rigthCmax < leftCmax && rigthCmax < newCmax)
            {
                wentRigth = true;
                Solve(inputList);
            }
            else if (leftCmax == Cmax)
            {
                job.DeliveryTime = originalDeliveryTime;
                job.PreparationTime = modifiedPreparationTime;
                inputList[jobIndexInList] = job;
                wentLeft = true;
                Solve(inputList);
            }
            else if (rigthCmax == Cmax)
            {
                wentRigth = true;
                Solve(inputList);
            }

            if (!wentLeft && leftCmax == newCmax && m_isDeep)
            {
                job.DeliveryTime = originalDeliveryTime;
                job.PreparationTime = modifiedPreparationTime;
                inputList[jobIndexInList] = job;
                Solve(inputList);
            }
            if (!wentRigth && (rigthCmax == leftCmax || rigthCmax == newCmax) && m_isDeep)
            {
                job.DeliveryTime = modifiedDeliveryTime;
                job.PreparationTime = originalPreparationTime;
                inputList[jobIndexInList] = job;
                Solve(inputList);
            }
        }
    }
}