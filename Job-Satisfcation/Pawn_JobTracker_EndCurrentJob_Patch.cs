using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;
using System.Reflection;
using System.Collections.Generic;

namespace Job_Satisfaction
{
    [HarmonyPatch(typeof(Pawn_JobTracker), "EndCurrentJob")]
    public static class Pawn_JobTracker_EndCurrentJob_Patch
    {
        private static readonly FieldInfo pawnField = typeof(Pawn_JobTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly HashSet<JobDef> idleJobs = new HashSet<JobDef>
        {
            JobDefOf.Wait,
            JobDefOf.Wait_MaintainPosture,
            JobDefOf.Wait_Wander,
            JobDefOf.GotoWander
        };

        private static bool loggedIdleJobMessage = false;

        static void Prefix(Pawn_JobTracker __instance, JobCondition condition, bool startNewJob, bool canReturnToPool)
        {
            try
            {
                if (__instance == null || condition == JobCondition.Succeeded && __instance.curJob == null)
                {
                    return; // Safeguard against null reference issues
                }

                Job job = __instance.curJob;
                Pawn pawn = pawnField?.GetValue(__instance) as Pawn;

                if (pawn == null)
                {
                    if (!loggedIdleJobMessage)
                    {
                        // Log.Message("JobSatisfaction: Pawn is null, skipping.");
                        loggedIdleJobMessage = true;
                    }
                    return;
                }

                if (idleJobs.Contains(job.def))
                {
                    if (!loggedIdleJobMessage)
                    {
                        // Log.Message($"JobSatisfaction: Job '{job.def.defName}' is idle, skipping.");
                        loggedIdleJobMessage = true;
                    }
                    return;
                }

                loggedIdleJobMessage = false;
                // Log.Message($"JobSatisfaction: Processing job '{job.def.defName}' for pawn '{pawn.Name}'.");

                float workAmount = CalculateWorkAmount(job, pawn, condition);

                if (workAmount > 0)
                {
                    var workTracker = Find.World.GetComponent<GameComponent_WorkTracker>();
                    if (workTracker != null)
                    {
                        WorkTracker.AddWork(pawn, workAmount);
                        workTracker.ApplyMoodBoosts(pawn);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in JobSatisfaction.Pawn_JobTracker_EndCurrentJob_Patch.Prefix: {ex}");
            }
        }

        private static float CalculateWorkAmount(Job job, Pawn pawn, JobCondition condition)
        {
            float workAmount = 0f;

            if (job == null || pawn == null)
            {
                return 0f; // Return 0 if job or pawn is null to prevent further errors
            }

            // Now your checks can continue here with the confidence that job and pawn are not null
            if (job.def == JobDefOf.DoBill && job.bill?.recipe != null)
            {
                workAmount = job.bill.recipe.WorkAmountTotal(pawn) / JobSatisfactionMod.settings.workAmountDividerForBills;
            }
            // Continue the rest of the method with similar checks...
            return workAmount;
        }
    }
}
