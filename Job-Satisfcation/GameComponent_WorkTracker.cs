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

            if (Find.TickManager.TicksGame > lastCheckTick + checkIntervalTicks)
            {
                WorkTracker.ResetAllWork();
                lastCheckTick = Find.TickManager.TicksGame;
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

            //Log.Message($"JobSatisfaction: Checking pawn {pawn.Name}, total work: {totalWork}, work speed: {workSpeed}");

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
                //Log.Message($"JobSatisfaction: Adding thought {thoughtDefName} to pawn {pawn.Name}");

                // Define all job satisfaction thoughts
                string[] jobSatisfactionThoughts = new string[]
                {
                    "JobSatisfaction_Small",
                    "JobSatisfaction_Medium",
                    "JobSatisfaction_Large",
                    "JobSatisfaction_Huge"
                };

                // Remove any existing job satisfaction thoughts
                if (pawn.needs != null && pawn.needs.mood != null)
                {
                    foreach (string defName in jobSatisfactionThoughts)
                    {
                        ThoughtDef existingThoughtDef = ThoughtDef.Named(defName);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(existingThoughtDef);
                    }

                    // Add the new job satisfaction thought
                    ThoughtDef thoughtDef = ThoughtDef.Named(thoughtDefName);
                    if (thoughtDef == null)
                    {
                        Log.Error($"JobSatisfaction: ThoughtDef {thoughtDefName} not found!");
                        return;
                    }

                    Thought_Memory thought = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
                    if (thought == null)
                    {
                        Log.Error($"JobSatisfaction: Failed to create thought {thoughtDefName} for pawn {pawn.Name}");
                        return;
                    }

                    pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
                    //Log.Message($"JobSatisfaction: Added thought {thoughtDefName} to pawn {pawn.Name} for work amount {totalWork}");
                }
                else
                {
                    Log.Warning($"JobSatisfaction: Pawn {pawn.Name} has no mood needs.");
                }
            }
            else
            {
                //Log.Message($"JobSatisfaction: Pawn {pawn.Name} does not meet any work threshold for mood boost.");
            }
        }
    }
}
