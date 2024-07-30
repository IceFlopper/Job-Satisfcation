using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Job_Satisfcation
{
    internal class NoMultipleThoughts
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
}
