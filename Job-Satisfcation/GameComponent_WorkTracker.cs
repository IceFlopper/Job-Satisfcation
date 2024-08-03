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

            if (Find.TickManager.TicksGame % CheckIntervalTicks == 0)
            {
                //Log.Message("JobSatisfaction: Resetting all work and applying mood boosts.");

                WorkTracker.ResetAllWork();

                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonists)
                {
                    ApplyMoodBoosts(pawn);
                }
            }
        }

        public void ApplyMoodBoosts(Pawn pawn)
        {
            float totalWork = WorkTracker.GetWork(pawn);
            var thresholds = JobSatisfactionUtility.CalculateThresholds(pawn);

            //Log.Message($"JobSatisfaction: Applying mood boosts for pawn '{pawn.Name}', total work: {totalWork}, thresholds: {thresholds}");

            if (JobSatisfactionMod.settings.enableBurnout && CheckForBurnout(pawn, totalWork, thresholds))
            {
                //Log.Message($"JobSatisfaction: Burnout detected for pawn '{pawn.Name}', skipping positive mood boost.");
                return;
            }


            string thoughtDefName = JobSatisfactionUtility.GetThoughtDefName(pawn, totalWork, thresholds.smallThreshold, thresholds.mediumThreshold, thresholds.largeThreshold, thresholds.hugeThreshold);

            if (thoughtDefName != null)
            {
                // Check if the pawn already has the thought
                if (pawn.needs.mood.thoughts.memories.Memories.Any(m => m.def.defName == thoughtDefName))
                {
                    //Log.Message($"JobSatisfaction: Pawn '{pawn.Name}' already has the thought '{thoughtDefName}', skipping.");
                    return;
                }

                //Log.Message($"JobSatisfaction: Adding thought '{thoughtDefName}' to pawn '{pawn.Name}'.");
                JobSatisfactionUtility.RemoveExistingJobSatisfactionThoughts(pawn, thoughtDefName);
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
    }
}
