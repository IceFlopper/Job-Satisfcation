using RimWorld;
using Verse;
using RimWorld.Planet;

namespace Job_Satisfaction
{
    public class GameComponent_WorkTracker : WorldComponent
    {
        private int lastCheckTick = 0;
        private const int checkIntervalTicks = 60000; // 1 in-game day

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
            float workSpeed = pawn.GetStatValue(StatDefOf.WorkSpeedGlobal);
            string thoughtDefName = null;

            // Scale work thresholds based on pawn's work speed
            float smallThreshold = 500 * workSpeed;
            float mediumThreshold = 1000 * workSpeed;
            float largeThreshold = 2000 * workSpeed;
            float hugeThreshold = 3500 * workSpeed;

            if (totalWork > smallThreshold && totalWork <= mediumThreshold)
            {
                thoughtDefName = "JobSatisfaction_Small";
            }
            else if (totalWork > mediumThreshold && totalWork <= largeThreshold)
            {
                thoughtDefName = "JobSatisfaction_Medium";
            }
            else if (totalWork > largeThreshold && totalWork <= hugeThreshold)
            {
                thoughtDefName = "JobSatisfaction_Large";
            }
            else if (totalWork > hugeThreshold)
            {
                thoughtDefName = "JobSatisfaction_Huge";
            }

            if (thoughtDefName != null)
            {
                JobSatisfactionUtility.RemoveExistingJobSatisfactionThoughts(pawn);
                JobSatisfactionUtility.AddJobSatisfactionThought(pawn, thoughtDefName);
            }
        }
    }
}
