using HarmonyLib;
using UnityEngine;
using Verse;

namespace Job_Satisfaction
{
    public class JobSatisfactionMod : Mod
    {
        public static JobSatisfactionSettings settings;

        public JobSatisfactionMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<JobSatisfactionSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Use Relative Threshold for Mood Boost Based on Work Speed", ref settings.relativeMoodBoost, "If enabled, mood boosts will be relative to the pawn's work speed.");
            listingStandard.CheckboxLabeled("Enable Burnout Feature", ref settings.enableBurnout, "If enabled, pawns can experience burnout with increased work thresholds.");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Job Satisfaction";
        }
    }

    [StaticConstructorOnStartup]
    public static class JobSatisfactionInitializer
    {
        static JobSatisfactionInitializer()
        {
            var harmony = new Harmony("com.Lewkah0.JobSatisfaction");
            harmony.PatchAll();
            Log.Message("JobSatisfaction: Harmony patches applied.");

            LongEventHandler.QueueLongEvent(InitGameComponent, "Initializing Job Satisfaction Mod", false, null);
        }

        private static void InitGameComponent()
        {
            if (Current.Game != null)
            {
                if (Current.Game.GetComponent<JobSatisfactionGameComponent>() == null)
                {
                    Current.Game.components.Add(new JobSatisfactionGameComponent(Current.Game));
                }
            }
        }
    }
}
