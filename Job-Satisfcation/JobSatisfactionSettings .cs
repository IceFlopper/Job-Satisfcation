using UnityEngine;
using Verse;

namespace Job_Satisfaction
{
    public class JobSatisfactionSettings : ModSettings
    {
        public bool relativeMoodBoost = true;
        public bool enableBurnout = true; // New setting for enabling/disabling burnout

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref relativeMoodBoost, "relativeMoodBoost", true);
            Scribe_Values.Look(ref enableBurnout, "enableBurnout", true); // Save/load the burnout setting
        }
    }
}
