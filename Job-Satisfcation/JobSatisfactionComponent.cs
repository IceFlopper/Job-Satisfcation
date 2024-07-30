using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Job_Satisfcation.NoMultipleThoughts;
using Verse;

namespace Job_Satisfcation
{
    internal class JobSatisfactionComponent
    {

    public class JobSatisfactionGameComponent : GameComponent
        {
            public JobSatisfactionGameComponent(Game game)
            {
            }

            public override void FinalizeInit()
            {
                base.FinalizeInit();
                AddCompToAllPawns();
            }

            private void AddCompToAllPawns()
            {
                foreach (var pawn in Find.WorldPawns.AllPawnsAlive)
                {
                    if (pawn.TryGetComp<CompLastThought>() == null)
                    {
                        pawn.AllComps.Add(new CompLastThought());
                    }
                }

                Log.Message("JobSatisfaction: Component added to all pawns.");
            }
        }

}
}
