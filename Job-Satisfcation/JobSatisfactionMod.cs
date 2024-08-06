using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Job_Satisfaction
{
    public class JobSatisfactionMod : Mod
    {
        public static JobSatisfactionSettings settings;

        private const float DefaultWorkAmountDividerForBills = 3f;
        private const float DefaultWorkAmountDividerForFrames = 12.5f;
        private const float DefaultWorkAmountMultiplierForResearch = 1f;
        private const float DefaultWorkAmountDividerForHarvesting = 100f;
        private const float DefaultWorkAmountDividerForCuttingPlants = 100f;
        private const float DefaultWorkAmountDividerForMining = 100f;
        private const float DefaultWorkAmountForCleaning = 2f;
        private const float DefaultWorkAmountDividerForHauling = 2f;
        private const float DefaultWorkAmountDividerForSowing = 4f;

        public JobSatisfactionMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<JobSatisfactionSettings>();

            // Harmony patching
            var harmony = new Harmony("com.example.rimworldmods.jobsatisfaction");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled("Use Relative Threshold for Mood Boost Based on Work Speed", ref settings.relativeMoodBoost, "If enabled, mood boosts will be relative to the pawn's work speed.");
            listingStandard.CheckboxLabeled("Enable Burnout Feature", ref settings.enableBurnout, "If enabled, pawns can experience burnout with increased work thresholds.");

            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Bills", ref settings.workAmountDividerForBills, DefaultWorkAmountDividerForBills, 0.1f, 10f);
            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Frames", ref settings.workAmountDividerForFrames, DefaultWorkAmountDividerForFrames, 0.1f, 20f);
            AddSliderWithResetButton(listingStandard, "Work Amount Multiplier For Research", ref settings.workAmountMultiplierForResearch, DefaultWorkAmountMultiplierForResearch, 0.1f, 10f);
            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Harvesting", ref settings.workAmountDividerForHarvesting, DefaultWorkAmountDividerForHarvesting, 0.1f, 200f);
            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Cutting Plants", ref settings.workAmountDividerForCuttingPlants, DefaultWorkAmountDividerForCuttingPlants, 0.1f, 200f);
            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Mining", ref settings.workAmountDividerForMining, DefaultWorkAmountDividerForMining, 0.1f, 200f);
            AddSliderWithResetButton(listingStandard, "Work Amount For Cleaning", ref settings.workAmountForCleaning, DefaultWorkAmountForCleaning, 0.1f, 10f);
            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Hauling", ref settings.workAmountDividerForHauling, DefaultWorkAmountDividerForHauling, 0.1f, 10f);
            AddSliderWithResetButton(listingStandard, "Work Amount Divider For Sowing", ref settings.workAmountDividerForSowing, DefaultWorkAmountDividerForSowing, 0.1f, 20f);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        private void AddSliderWithResetButton(Listing_Standard listing, string label, ref float settingValue, float defaultValue, float minValue, float maxValue)
        {
            listing.Label($"{label}: {settingValue:F2}");

            float newValue = listing.Slider(settingValue, minValue, maxValue);
            settingValue = Mathf.Round(newValue * 10f) / 10f; // Round to nearest 0.1

            Rect resetButtonRect = listing.GetRect(Text.LineHeight);
            resetButtonRect.width = 100f;
            if (Widgets.ButtonText(resetButtonRect, "Reset"))
            {
                settingValue = defaultValue;
            }

            listing.Gap();
        }

        public override string SettingsCategory()
        {
            return "Job Satisfaction";
        }
    }
}
