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
            float burnoutThreshold = thresholds.hugeThreshold * 1.5f;

            if (totalWork > burnoutThreshold)
            {
                JobSatisfactionUtility.RemoveExistingJobSatisfactionThoughts(pawn, "JobSatisfaction_Burnout");
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, "JobSatisfaction_Burnout");
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
