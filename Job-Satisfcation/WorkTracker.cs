using System;
using System.Collections.Generic;
using Verse;

namespace Job_Satisfaction
{
    public static class WorkTracker
    {
        private static Dictionary<Pawn, float> workAmounts = new Dictionary<Pawn, float>();

        public static void AddWork(Pawn pawn, float amount)
        {
            if (!workAmounts.ContainsKey(pawn))
            {
                workAmounts[pawn] = 0f;
            }
            workAmounts[pawn] += amount;
            //Log.Message($"JobSatisfaction: Added {amount} work to pawn '{pawn.Name}', total work now {workAmounts[pawn]}.");
        }

        public static float GetWork(Pawn pawn)
        {
            if (workAmounts.ContainsKey(pawn))
            {
                return workAmounts[pawn];
            }
            return 0f;
        }

        public static void ResetWork(Pawn pawn)
        {
            if (workAmounts.ContainsKey(pawn))
            {
                workAmounts[pawn] = 0f;
                //Log.Message($"JobSatisfaction: Reset work for pawn '{pawn.Name}'.");
            }
        }

        public static void ResetAllWork()
        {
            workAmounts.Clear();
            //Log.Message("JobSatisfaction: Reset all work for all pawns.");
        }
    }
}
