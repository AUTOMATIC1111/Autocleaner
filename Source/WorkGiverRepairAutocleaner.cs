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
    class WorkGiverRepairAutocleaner : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Globals.Autocleaner);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerThings.ThingsOfDef(Globals.Autocleaner);
        }

        public override bool ShouldSkip(Pawn pawn, bool forced)
        {
            return pawn.Map.listerThings.ThingsOfDef(Globals.Autocleaner).Count == 0;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            PawnAutocleaner cleaner = t as PawnAutocleaner;
            if (cleaner == null || !cleaner.Broken) return false;

            if (cleaner.CurJobDef == Globals.AutocleanerClean || cleaner.CurJobDef == Globals.AutocleanerGoto)
            {
                JobFailReason.Is("AutocleanerNotStationary".Translate(), null);
                return false;
            }

            if (FindClosestComponent(pawn) == null)
            {
                JobFailReason.Is("NoComponentsToRepair".Translate(), null);
                return false;
            }

            return pawn.CanReserve(t, 1, -1, null, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Thing t2 = FindClosestComponent(pawn);
            Job job = JobMaker.MakeJob(Globals.AutocleanerRepair, t, t2);
            job.count = 1;
            return job;
        }

        private Thing FindClosestComponent(Pawn pawn)
        {
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.ComponentIndustrial), PathEndMode.InteractionCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1, null, false));
        }
    }
}
