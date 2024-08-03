using Verse;

namespace Job_Satisfaction
{
    public class JobSatisfactionSettings : ModSettings
    {
        public bool relativeMoodBoost = true;
        public bool enableBurnout = true;

        public float workAmountDividerForBills = 1f;
        public float workAmountDividerForFrames = 10f;
        public float workAmountMultiplierForResearch = 1f;
        public float workAmountDividerForHarvesting = 100f;
        public float workAmountDividerForCuttingPlants = 100f;
        public float workAmountDividerForMining = 100f;
        public float workAmountForCleaning = 2f;
        public float workAmountDividerForHauling = 2f;
        public float workAmountDividerForSowing = 4f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref relativeMoodBoost, "relativeMoodBoost", true);
            Scribe_Values.Look(ref enableBurnout, "enableBurnout", true);

            Scribe_Values.Look(ref workAmountDividerForBills, "workAmountDividerForBills", 1f);
            Scribe_Values.Look(ref workAmountDividerForFrames, "workAmountDividerForFrames", 10f);
            Scribe_Values.Look(ref workAmountMultiplierForResearch, "workAmountMultiplierForResearch", 1f);
            Scribe_Values.Look(ref workAmountDividerForHarvesting, "workAmountDividerForHarvesting", 100f);
            Scribe_Values.Look(ref workAmountDividerForCuttingPlants, "workAmountDividerForCuttingPlants", 100f);
            Scribe_Values.Look(ref workAmountDividerForMining, "workAmountDividerForMining", 100f);
            Scribe_Values.Look(ref workAmountForCleaning, "workAmountForCleaning", 2f);
            Scribe_Values.Look(ref workAmountDividerForHauling, "workAmountDividerForHauling", 2f);
            Scribe_Values.Look(ref workAmountDividerForSowing, "workAmountDividerForSowing", 4f);
        }
    }
}
