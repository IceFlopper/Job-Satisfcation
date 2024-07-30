using HarmonyLib;
using Verse;

namespace Job_Satisfaction
{
    [StaticConstructorOnStartup]
    public static class JobSatisfactionMod
    {
        static JobSatisfactionMod()
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