using RimWorld;
using Verse;
using System.Linq;

namespace Job_Satisfaction
{
    public static class JobSatisfactionUtility
    {
        public static readonly string[] JobSatisfactionThoughts = new string[]
        {
            "JobSatisfaction_Small",
            "JobSatisfaction_Medium",
            "JobSatisfaction_Large",
            "JobSatisfaction_Huge",
            "JobSatisfaction_Burnout"
        };

        public static void RemoveExistingJobSatisfactionThoughts(Pawn pawn, string thoughtDefName = null)
        {
            if (pawn.needs != null && pawn.needs.mood != null)
            {
                foreach (string defName in JobSatisfactionUtility.JobSatisfactionThoughts)
                {
                    if (thoughtDefName == null || defName == thoughtDefName)
                    {
                        ThoughtDef existingThoughtDef = ThoughtDef.Named(defName);
                        if (existingThoughtDef != null)
                        {
                            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(existingThoughtDef);
                            //Log.Message($"JobSatisfaction: Removed thought '{defName}' from pawn '{pawn.Name}'.");
                        }
                    }
                }
            }
        }

        public static void AddJobSatisfactionThought(Pawn pawn, string thoughtDefName)
        {
            ThoughtDef thoughtDef = ThoughtDef.Named(thoughtDefName);
            if (thoughtDef == null)
            {
                Log.Error($"JobSatisfaction: ThoughtDef '{thoughtDefName}' not found!");
                return;
            }

            if (!pawn.needs.mood.thoughts.memories.Memories.Any(m => m.def == thoughtDef))
            {
                Thought_Memory thought = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
                if (thought == null)
                {
                    Log.Error($"JobSatisfaction: Failed to create thought '{thoughtDefName}' for pawn '{pawn.Name}'.");
                    return;
                }

                //Log.Message($"JobSatisfaction: Adding thought '{thoughtDefName}' to pawn '{pawn.Name}'.");
                pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
            }
            else
            {
                //Log.Message($"JobSatisfaction: Thought '{thoughtDefName}' already present for pawn '{pawn.Name}', not adding again.");
            }
        }

        public static (float smallThreshold, float mediumThreshold, float largeThreshold, float hugeThreshold) CalculateThresholds(Pawn pawn)
        {
            float workSpeed = pawn.GetStatValue(StatDefOf.WorkSpeedGlobal);
            float traitMultiplier = GetTraitMultiplier(pawn);

            float smallThreshold = 500 * workSpeed * traitMultiplier;
            float mediumThreshold = 1000 * workSpeed * traitMultiplier;
            float largeThreshold = 2000 * workSpeed * traitMultiplier;
            float hugeThreshold = 3500 * workSpeed * traitMultiplier;

            return (smallThreshold, mediumThreshold, largeThreshold, hugeThreshold);
        }

        private static float GetTraitMultiplier(Pawn pawn)
        {
            Trait industriousness = pawn.story.traits.GetTrait(TraitDef.Named("Industriousness"));
            if (industriousness != null)
            {
                switch (industriousness.Degree)
                {
                    case 2:
                        return 0.8f;
                    case 1:
                        return 0.9f;
                    case -1:
                        return 1.1f;
                    case -2:
                        return 1.2f;
                }
            }
            return 1f;
        }

        public static string GetThoughtDefName(Pawn pawn, float totalWork, float smallThreshold, float mediumThreshold, float largeThreshold, float hugeThreshold)
        {
            Trait industriousness = pawn.story.traits.GetTrait(TraitDef.Named("Industriousness"));
            if (industriousness != null)
            {
                string prefix = industriousness.Degree > 0 ? "JobSatisfaction_HardWorker_" : "JobSatisfaction_Lazy_";
                if (totalWork > smallThreshold && totalWork <= mediumThreshold)
                {
                    return prefix + "Small";
                }
                else if (totalWork > mediumThreshold && totalWork <= largeThreshold)
                {
                    return prefix + "Medium";
                }
                else if (totalWork > largeThreshold && totalWork <= hugeThreshold)
                {
                    return prefix + "Large";
                }
                else if (totalWork > hugeThreshold)
                {
                    return prefix + "Huge";
                }
            }

            if (totalWork > smallThreshold && totalWork <= mediumThreshold)
            {
                return "JobSatisfaction_Small";
            }
            else if (totalWork > mediumThreshold && totalWork <= largeThreshold)
            {
                return "JobSatisfaction_Medium";
            }
            else if (totalWork > largeThreshold && totalWork <= hugeThreshold)
            {
                return "JobSatisfaction_Large";
            }
            else if (totalWork > hugeThreshold)
            {
                return "JobSatisfaction_Huge";
            }

            return null;
        }
    }
}
