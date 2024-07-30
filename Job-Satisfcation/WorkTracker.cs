using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

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
            Log.Message($"JobSatisfaction: Total work amount {WorkTracker.GetWork(pawn)} for pawn {pawn.Name}");
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
            }
        }

        public static void ResetAllWork()
        {
            workAmounts.Clear();
        }
    }
}
