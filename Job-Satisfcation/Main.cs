using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;
using System.Reflection;
using static Job_Satisfcation.NoMultipleThoughts;

[HarmonyPatch(typeof(Pawn_JobTracker), "EndCurrentJob")]
class Pawn_JobTracker_EndCurrentJob_Patch
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

            string thoughtDefName = null;
            int moodBoost = 0;

            // Check for DoBill jobs (crafting)
            if (job.def == JobDefOf.DoBill && job.bill != null)
            {
                if (job.bill.recipe != null)
                {
                    float workAmount = job.bill.recipe.WorkAmountTotal(pawn);
                    if (workAmount > 50 && workAmount <= 100)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongBill";
                        moodBoost = 3;
                    }
                    else if (workAmount > 100 && workAmount <= 200)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongBill";
                        moodBoost = 5;
                    }
                    else if (workAmount > 200 && workAmount <= 300)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongBill";
                        moodBoost = 10;
                    }
                    else if (workAmount > 300)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongBill";
                        moodBoost = 15;
                    }
                }
            }
            // Check for FinishFrame jobs (construction)
            else if (job.def == JobDefOf.FinishFrame)
            {
                if (job.targetA.Thing is Frame frame && condition == JobCondition.Succeeded)
                {
                    float workToBuild = frame.WorkToBuild;
                    if (workToBuild > 50 && workToBuild <= 100)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongConstruction";
                        moodBoost = 3;
                    }
                    else if (workToBuild > 100 && workToBuild <= 200)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongConstruction";
                        moodBoost = 5;
                    }
                    else if (workToBuild > 200 && workToBuild <= 300)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongConstruction";
                        moodBoost = 10;
                    }
                    else if (workToBuild > 300)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongConstruction";
                        moodBoost = 15;
                    }
                }
            }
            // Check for art jobs (creating art)
            else if (job.def == JobDefOf.DoBill && job.bill.recipe != null)
            {
                if (job.bill.recipe.products.Exists(prod => prod.thingDef.thingClass == typeof(Building_Art)))
                {
                    float workAmount = job.bill.recipe.WorkAmountTotal(pawn);
                    if (workAmount > 50 && workAmount <= 100)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongArt";
                        moodBoost = 3;
                    }
                    else if (workAmount > 100 && workAmount <= 200)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongArt";
                        moodBoost = 5;
                    }
                    else if (workAmount > 200 && workAmount <= 300)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongArt";
                        moodBoost = 10;
                    }
                    else if (workAmount > 300)
                    {
                        thoughtDefName = "JobSatisfaction_CompletedLongArt";
                        moodBoost = 15;
                    }
                }
            }
            //Check for Research jobs
            else if (job.def == JobDefOf.Research)
                    {
                        ResearchProjectDef currentResearch = Find.ResearchManager.GetProject();
                        if (currentResearch != null)
                        {
                            float researchCost = currentResearch.baseCost;
                            if (researchCost > 250 && researchCost <= 500)
                            {
                                thoughtDefName = "JobSatisfaction_CompletedSignificantResearch";
                                moodBoost = 3;
                            }
                            else if (researchCost > 500 && researchCost <= 1000)
                            {
                                thoughtDefName = "JobSatisfaction_CompletedSignificantResearch";
                                moodBoost = 5;
                            }
                            else if (researchCost > 1000 && researchCost <= 2000)
                            {
                                thoughtDefName = "JobSatisfaction_CompletedSignificantResearch";
                                moodBoost = 10;
                            }
                            else if (researchCost > 2000)
                            {
                                thoughtDefName = "JobSatisfaction_CompletedSignificantResearch";
                                moodBoost = 15;
                            }
                        }
                    }

            if (thoughtDefName != null)
            {
                Thought_Memory thought = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDef.Named(thoughtDefName));
                if (pawn.needs != null && pawn.needs.mood != null)
                {
                    thought.moodPowerFactor = moodBoost / 10f; // Adjust the mood effect based on moodBoost
                    pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
                    Log.Message($"JobSatisfaction: Added thought {thoughtDefName} with mood boost {moodBoost} to pawn {pawn.Name} for job {job.def.defName}");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Exception in JobSatisfaction.Pawn_JobTracker_EndCurrentJob_Patch.Prefix: {ex}");
        }
    }
}

[StaticConstructorOnStartup]
public static class JobSatisfactionMod
{
    static JobSatisfactionMod()
    {
        var harmony = new Harmony("com.Lewkah0.JobSatisfaction");
        harmony.PatchAll();
        Log.Message("JobSatisfaction: Harmony patches applied.");
    }
}


