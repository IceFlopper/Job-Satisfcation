using Verse;
using System.Collections.Generic;
using RimWorld;

namespace Job_Satisfaction
{
    public static class JobSatisfactionUtility
    {
        public static readonly string[] JobSatisfactionThoughts = new string[]
        {
            "JobSatisfaction_Small",
            "JobSatisfaction_Medium",
            "JobSatisfaction_Large",
            "JobSatisfaction_Huge"
        };

        public static void RemoveExistingJobSatisfactionThoughts(Pawn pawn)
        {
            if (pawn.needs != null && pawn.needs.mood != null)
            {
                foreach (string defName in JobSatisfactionUtility.JobSatisfactionThoughts)
                {
                    ThoughtDef existingThoughtDef = ThoughtDef.Named(defName);
                    pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(existingThoughtDef);
                }
            }
        }

        public static void AddJobSatisfactionThought(Pawn pawn, string thoughtDefName)
        {
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
        }
    }
}
