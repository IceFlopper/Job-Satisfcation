using RimWorld;
using Verse;
using RimWorld.Planet;
using System.Linq;

namespace Job_Satisfaction
{
    public class GameComponent_WorkTracker : WorldComponent
    {
        public static bool EnableLogging = false;

        public GameComponent_WorkTracker(World world) : base(world) { }

        public void ApplyMoodBoosts(Pawn pawn)
        {
            float totalWork = WorkTracker.GetWork(pawn);
            var thresholds = JobSatisfactionUtility.CalculateThresholds(pawn);

            if (JobSatisfactionMod.settings.enableBurnout && CheckForBurnout(pawn, totalWork, thresholds))
            {
                return;
            }

            string thoughtDefName = JobSatisfactionUtility.GetThoughtDefName(pawn, totalWork, thresholds.smallThreshold, thresholds.mediumThreshold, thresholds.largeThreshold, thresholds.hugeThreshold);

            if (thoughtDefName != null)
            {
                if (pawn.needs.mood.thoughts.memories.Memories.Any(m => m.def.defName == thoughtDefName))
                {
                    return;
                }

                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, thoughtDefName);
            }
        }

        private bool CheckForBurnout(Pawn pawn, float totalWork, (float smallThreshold, float mediumThreshold, float largeThreshold, float hugeThreshold) thresholds)
        {

            float smallThresholdChance = 0.01f;
            float mediumThresholdChance = 0.01f;
            float largeThresholdChance = 0.03f;
            float hugeThresholdChance = 0.06f;

            if (totalWork > thresholds.smallThreshold && Rand.Value < smallThresholdChance)
            {
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, "JobSatisfaction_Burnout_Small");
                return true;
            }
            else if (totalWork > thresholds.mediumThreshold && Rand.Value < mediumThresholdChance)
            {
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, "JobSatisfaction_Burnout_Medium");
                return true;
            }
            else if (totalWork > thresholds.largeThreshold && Rand.Value < largeThresholdChance)
            {
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, "JobSatisfaction_Burnout_Large");
                return true;
            }
            else if (totalWork > thresholds.hugeThreshold && Rand.Value < hugeThresholdChance)
            {
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, "JobSatisfaction_Burnout_Huge");
                return true;
            }

            return false;
        }


        public void ResetAllWork()
        {
            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonists)
            {
                WorkTracker.ResetWork(pawn);
            }
            Log.Message("JobSatisfaction: Reset work for all pawns.");
        }

        public void LogAllWorkAmounts()
        {
            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonists)
            {
                float workAmount = WorkTracker.GetWork(pawn);
                Log.Message($"JobSatisfaction: Pawn '{pawn.Name}' has {workAmount} work done.");
            }
        }

        public void ToggleLogging()
        {
            EnableLogging = !EnableLogging;
            Log.Message($"JobSatisfaction: Logging is now {(EnableLogging ? "enabled" : "disabled")}.");
        }
    }
}
