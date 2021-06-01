using Google.OrTools.Sat;
using System;
using System.Collections;
using SPD1.Misc;
using System.Collections.Generic;

namespace SPD1.Algorithms
{
    public class ORWrapper
    {
        public static void solve(List<RPQJob> inputList)
        {
            CpModel model = new CpModel();
            int longestPreparationTime = 0;
            int longestDeliveryTime= 0;
            int workTimeSum =0;
            foreach (RPQJob job in inputList)
            {
                workTimeSum += job.WorkTime;
                longestPreparationTime = Math.Max(longestPreparationTime,job.PreparationTime);
                longestDeliveryTime = Math.Max(longestDeliveryTime,job.DeliveryTime);
            }
            IntVar cmax = model.NewIntVar(0, 1+ longestDeliveryTime+ longestPreparationTime +workTimeSum,"CMax");
            
        }
    }
}