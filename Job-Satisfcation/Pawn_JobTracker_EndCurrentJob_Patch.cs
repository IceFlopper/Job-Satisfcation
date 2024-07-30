using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;
using System.Reflection;

namespace Job_Satisfaction
{
    //make sure work progress resets when mood thought dissapears
    [HarmonyPatch(typeof(Pawn_JobTracker), "EndCurrentJob")]
    public static class Pawn_JobTracker_EndCurrentJob_Patch
    {
        private static readonly FieldInfo pawnField = typeof(Pawn_JobTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        static void Prefix(Pawn_JobTracker __instance, JobCondition condition, bool startNewJob, bool canReturnToPool)
        {
            try
            {
                Job job = __instance.curJob;
                Pawn pawn = pawnField.GetValue(__instance) as Pawn;

                if (job == null || pawn == null)
                {
                    Log.Message("JobSatisfaction: Job or pawn is null, exiting.");
                    return;
                }

                float workAmount = 0f;

                if (job.def == JobDefOf.DoBill && job.bill != null && job.bill.recipe != null)
                {
                    workAmount = job.bill.recipe.WorkAmountTotal(pawn);
                }
                else if (job.def == JobDefOf.FinishFrame && job.targetA.Thing is Frame frame && condition == JobCondition.Succeeded)
                {
                    workAmount = (float)(frame.WorkToBuild / 47.25);
                }
                else if (job.def == JobDefOf.Research)
                {
                    ResearchProjectDef currentResearch = Find.ResearchManager.GetProject();
                    if (currentResearch != null)
                    {
                        workAmount = currentResearch.baseCost;
                    }
                }
                else if (job.def == JobDefOf.Harvest && job.targetA.Thing is Plant plant && condition == JobCondition.Succeeded)
                {
                    workAmount = plant.def.plant.harvestWork;
                }
                else if (job.def == JobDefOf.CutPlant && job.targetA.Thing is Plant plantToCut && condition == JobCondition.Succeeded)
                {
                    workAmount = plantToCut.def.plant.harvestWork; 
                }
                else if (job.def == JobDefOf.Mine && job.targetA.Thing is Mineable mineable && condition == JobCondition.Succeeded)
                {
                    workAmount = mineable.def.building.mineableYield;
                }
                else if (job.def == JobDefOf.Clean)
                {
                    workAmount = 2; 
                }
                else if (job.def == JobDefOf.HaulToCell || job.def == JobDefOf.HaulToContainer)
                {
                    workAmount = 2;
                }

                if (workAmount > 0)
                {
                    WorkTracker.AddWork(pawn, workAmount);
                    //Log.Message($"JobSatisfaction: Added work amount {workAmount} for pawn {pawn.Name} for job {job.def.defName}");

                    // Call ApplyMoodBoosts method here
                    var workTracker = Find.World.GetComponent<GameComponent_WorkTracker>();
                    workTracker?.ApplyMoodBoosts(pawn);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in JobSatisfaction.Pawn_JobTracker_EndCurrentJob_Patch.Prefix: {ex}");
            }
        }
    }
}
