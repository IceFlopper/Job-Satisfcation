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
                if (condition != JobCondition.Succeeded)
                {
                    if (!loggedIdleJobMessage)
                    {
                        //Log.Message("JobSatisfaction: Job did not succeed, skipping.");
                        loggedIdleJobMessage = true;
                    }
                    return;
                }

                Job job = __instance.curJob;
                Pawn pawn = pawnField.GetValue(__instance) as Pawn;

                if (job == null || pawn == null)
                {
                    if (!loggedIdleJobMessage)
                    {
                        //Log.Message("JobSatisfaction: Job or pawn is null, skipping.");
                        loggedIdleJobMessage = true;
                    }
                    return;
                }

                if (idleJobs.Contains(job.def))
                {
                    if (!loggedIdleJobMessage)
                    {
                        //Log.Message($"JobSatisfaction: Job '{job.def.defName}' is idle, skipping.");
                        loggedIdleJobMessage = true;
                    }
                    return;
                }

                loggedIdleJobMessage = false; // Reset the flag for future messages

                //Log.Message($"JobSatisfaction: Processing job '{job.def.defName}' for pawn '{pawn.Name}'.");

                float workAmount = CalculateWorkAmount(job, pawn, condition);

                //Log.Message($"JobSatisfaction: Calculated work amount {workAmount} for job '{job.def.defName}'.");

                if (workAmount > 0)
                {
                    WorkTracker.AddWork(pawn, workAmount);

                    var workTracker = Find.World.GetComponent<GameComponent_WorkTracker>();
                    workTracker?.ApplyMoodBoosts(pawn);
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

            if (job.def == JobDefOf.DoBill && job.bill != null && job.bill.recipe != null)
            {
                workAmount = job.bill.recipe.WorkAmountTotal(pawn) / JobSatisfactionMod.settings.workAmountDividerForBills;
            }
            else if (job.def == JobDefOf.FinishFrame && job.targetA.Thing is Frame frame)
            {
                workAmount = frame.WorkToBuild / JobSatisfactionMod.settings.workAmountDividerForFrames;
            }
            else if (job.def == JobDefOf.Research)
            {
                ResearchProjectDef currentResearch = Find.ResearchManager.GetProject();
                if (currentResearch != null)
                {
                    workAmount = currentResearch.baseCost * JobSatisfactionMod.settings.workAmountMultiplierForResearch;
                }
            }
            else if (job.def == JobDefOf.Harvest || job.def == JobDefOf.HarvestDesignated)
            {
                if (job.targetA.Thing is Plant plant && condition == JobCondition.Succeeded)
                {
                    workAmount = plant.def.plant.harvestWork / JobSatisfactionMod.settings.workAmountDividerForHarvesting;
                }
            }
            else if (job.def == JobDefOf.CutPlant)
            {
                if (job.targetA.Thing is Plant plantToCut && condition == JobCondition.Succeeded)
                {
                    workAmount = plantToCut.def.plant.harvestWork / JobSatisfactionMod.settings.workAmountDividerForCuttingPlants;
                }
            }
            else if (job.def == JobDefOf.Mine)
            {
                if (job.targetA.Thing is Mineable mineable && condition == JobCondition.Succeeded)
                {
                    workAmount = mineable.MaxHitPoints / JobSatisfactionMod.settings.workAmountDividerForMining;
                }
            }
            else if (job.def == JobDefOf.Clean)
            {
                workAmount = JobSatisfactionMod.settings.workAmountForCleaning;
            }
            else if (job.def == JobDefOf.HaulToCell || job.def == JobDefOf.HaulToContainer)
            {
                if (job.targetA.Thing != null)
                {
                    workAmount = job.targetA.Thing.def.VolumePerUnit * job.targetA.Thing.stackCount / JobSatisfactionMod.settings.workAmountDividerForHauling;
                }
            }
            else if (job.def == JobDefOf.Sow)
            {
                if (job.targetA.Thing is Plant plantToSow)
                {
                    workAmount = plantToSow.def.plant.sowWork / JobSatisfactionMod.settings.workAmountDividerForSowing;
                }
            }

            return workAmount;
        }

    }
}
