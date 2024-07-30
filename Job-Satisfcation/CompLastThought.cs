using Verse;

namespace Job_Satisfaction
{
    public class CompLastThought : ThingComp
    {
        public int lastThoughtTick = -1;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref lastThoughtTick, "lastThoughtTick", -1);
        }
    }

    public class CompProperties_LastThought : CompProperties
    {
        public CompProperties_LastThought()
        {
            this.compClass = typeof(CompLastThought);
        }
    }
}
