using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace Job_Satisfaction
{
    [HarmonyPatch(typeof(MemoryThoughtHandler), "RemoveMemory")]
    public static class MemoryThoughtHandler_RemoveMemory_Patch
    {
        static void Prefix(MemoryThoughtHandler __instance, Thought_Memory th)
        {
            try
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                if (pawn == null || th == null)
                {
                    Log.Message("JobSatisfaction: Pawn or Thought_Memory is null, exiting.");
                    return;
                }

                // Check if the thought memory is naturally expiring
                if (th.age >= th.def.durationDays * 60000) 
                {
                    if (Array.Exists(JobSatisfactionUtility.JobSatisfactionThoughts, thoughtDefName => th.def.defName == thoughtDefName))
                    {
                        WorkTracker.ResetWork(pawn);
                        Log.Message($"JobSatisfaction: Reset work for pawn {pawn.Name} due to expiring thought {th.def.defName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in JobSatisfaction.MemoryThoughtHandler_RemoveMemory_Patch.Prefix: {ex}");
            }
        }
    }
}
