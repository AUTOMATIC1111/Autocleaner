using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Autocleaner
{
    public class JobGiverAutocleaner : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            if (cleaner == null) return null;
            if (!cleaner.active) return null;
            if (cleaner.LowPower) return null;
            if (cleaner.health.summaryHealth.SummaryHealthPercent < 1) return null;

            WorkGiverCleanFilth scanner = Globals.AutocleanerCleanFilth.Worker as WorkGiverCleanFilth;
            if (scanner == null) return null;

            IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn).Where(x => scanner.HasJobOnThing(pawn, x));
            Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f);
            if (thing == null) return null;

            Job job = scanner.JobOnThing(pawn, thing, false);
            return job;
        }
    }
}
