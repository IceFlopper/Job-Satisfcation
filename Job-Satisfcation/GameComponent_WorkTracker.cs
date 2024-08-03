using RimWorld;
using Verse;
using RimWorld.Planet;

namespace Job_Satisfaction
{
    public class GameComponent_WorkTracker : WorldComponent
    {
        private const int CheckIntervalTicks = 60000; // 1 in-game day

        public GameComponent_WorkTracker(World world) : base(world) { }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();

            int currentHour = GenLocalDate.HourOfDay(Find.CurrentMap);

            // Check if it's midnight (0 hour) to reset the work
            if (currentHour == 0)
            {
                WorkTracker.ResetAllWork();

                // Optionally, apply mood boosts to all pawns
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonists)
                {
                    ApplyMoodBoosts(pawn);
                }
            }
        }

        public void ApplyMoodBoosts(Pawn pawn)
        {
            float totalWork = WorkTracker.GetWork(pawn);
            float workSpeed = JobSatisfactionMod.settings.relativeMoodBoost ? pawn.GetStatValue(StatDefOf.WorkSpeedGlobal) : 1f;
            string thoughtDefName = null;

            // Get trait multipliers
            float traitMultiplier = GetTraitMultiplier(pawn);

            // Scale work thresholds based on pawn's work speed and traits
            float smallThreshold = 500 * workSpeed * traitMultiplier;
            float mediumThreshold = 1000 * workSpeed * traitMultiplier;
            float largeThreshold = 2000 * workSpeed * traitMultiplier;
            float hugeThreshold = 3500 * workSpeed * traitMultiplier;

            Log.Message($"JobSatisfaction: Pawn {pawn.Name} has worked {totalWork} with thresholds - Small: {smallThreshold}, Medium: {mediumThreshold}, Large: {largeThreshold}, Huge: {hugeThreshold}");

            // Check for burnout first
            if (JobSatisfactionMod.settings.enableBurnout && CheckForBurnout(pawn, totalWork, smallThreshold, mediumThreshold, largeThreshold, hugeThreshold))
            {
                // If burnout occurs, skip applying positive mood boost
                Log.Message($"JobSatisfaction: Pawn {pawn.Name} has experienced burnout.");
                return;
            }

            // Determine appropriate thought based on work done and traits
            thoughtDefName = GetThoughtDefName(pawn, totalWork, smallThreshold, mediumThreshold, largeThreshold, hugeThreshold);

            // Apply the thought
            if (thoughtDefName != null)
            {
                JobSatisfactionUtility.RemoveExistingJobSatisfactionThoughts(pawn);
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, thoughtDefName);
            }
        }

        private bool CheckForBurnout(Pawn pawn, float totalWork, float smallThreshold, float mediumThreshold, float largeThreshold, float hugeThreshold)
        {
            float burnoutChance = 0f;

            if (totalWork > smallThreshold && totalWork <= mediumThreshold)
            {
                burnoutChance = 0.02f;
            }
            else if (totalWork > mediumThreshold && totalWork <= largeThreshold)
            {
                burnoutChance = 0.04f;
            }
            else if (totalWork > largeThreshold && totalWork <= hugeThreshold)
            {
                burnoutChance = 0.06f;
            }
            else if (totalWork > hugeThreshold)
            {
                burnoutChance = 0.08f;
            }

            if (Rand.Value < burnoutChance)
            {
                JobSatisfactionUtility.RemoveExistingJobSatisfactionThoughts(pawn);
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, "JobSatisfaction_Burnout");
                return true; // Burnout occurred
            }

            return false; // No burnout
        }

        private float GetTraitMultiplier(Pawn pawn)
        {
            Trait industriousness = pawn.story.traits.GetTrait(TraitDef.Named("Industriousness"));
            if (industriousness != null)
            {
                switch (industriousness.Degree)
                {
                    case 2: // Industrious
                        return 0.8f;
                    case 1: // Hard Worker
                        return 0.9f;
                    case -1: // Lazy
                        return 1.1f;
                    case -2: // Slothful
                        return 1.2f;
                }
            }
            return 1f;
        }

        private string GetThoughtDefName(Pawn pawn, float totalWork, float smallThreshold, float mediumThreshold, float largeThreshold, float hugeThreshold)
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

            // Default thoughts for pawns without the relevant traits
            if (totalWork > smallThreshold && totalWork <= mediumThreshold)
            {
                return "JobSatisfaction_Small";
            }
            else if (totalWork > mediumThreshold && totalWork <= largeThreshold)
            {
                return "JobSatisfaction_Large";
            }
            else if (totalWork > largeThreshold && totalWork <= hugeThreshold)
            {
                return "JobSatisfaction_Huge";
            }
            else if (totalWork > hugeThreshold)
            {
                return "JobSatisfaction_Huge";
            }

            return null;
        }
    }
}
