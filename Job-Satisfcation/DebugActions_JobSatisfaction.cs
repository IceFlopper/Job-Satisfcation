using LudeonTK;
using System.Collections.Generic;
using Verse;

namespace Job_Satisfaction
{
    public static class DebugActions_JobSatisfaction
    {
        [DebugAction("Job Satisfaction", "Reset All Work", actionType = DebugActionType.Action)]
        public static void ResetAllWork()
        {
            var workTracker = Find.World.GetComponent<GameComponent_WorkTracker>();
            workTracker?.ResetAllWork();
        }

        [DebugOutput("Job Satisfaction", name = "Log All Work Amounts", onlyWhenPlaying = false)]
        public static void LogAllWorkAmounts()
        {
            var workTracker = Find.World.GetComponent<GameComponent_WorkTracker>();
            workTracker?.LogAllWorkAmounts();
        }

        [DebugOutput("Job Satisfaction", name = "Toggle Work Logging", onlyWhenPlaying = false)]
        public static void ToggleLogging()
        {
            var workTracker = Find.World.GetComponent<GameComponent_WorkTracker>();
            workTracker?.ToggleLogging();
        }
    }
}
