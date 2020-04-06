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
    public class JobGiverGotoCorner : ThinkNode_JobGiver
    {
        public bool SuitablePosition(IntVec3 pos, Map map, bool fallback = false)
        {
            if (!pos.InBounds(map)) return false;
            if (!pos.Roofed(map)) return false;
            if (!pos.Standable(map)) return false;

            if (!fallback)
            {
                int v = 0;
                int h = 0;
                if (new IntVec3(pos.x + 1, pos.y, pos.z + 0).Impassable(map)) { v++; }
                if (new IntVec3(pos.x - 1, pos.y, pos.z + 0).Impassable(map)) { v++; }
                if (new IntVec3(pos.x + 0, pos.y, pos.z + 1).Impassable(map)) { h++; }
                if (new IntVec3(pos.x + 0, pos.y, pos.z - 1).Impassable(map)) { h++; }

                if (v < 1 || h < 1) return false;
            }

            CompPower comp = PowerConnectionMaker.BestTransmitterForConnector(pos, map);
            return comp != null;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            if (cleaner == null) return null;
            if (!cleaner.active) return null;

            Map map = pawn.Map;
            if (SuitablePosition(pawn.Position, map)) return null;

            IntVec3 target;
            if (!RCellFinder.TryFindRandomCellNearWith(pawn.Position, x => SuitablePosition(x, map) && pawn.CanReach(x, PathEndMode.OnCell, Danger.Deadly), pawn.Map, out target))
            {
                if (!RCellFinder.TryFindRandomCellNearWith(pawn.Position, x => SuitablePosition(x, map, true) && pawn.CanReach(x, PathEndMode.OnCell, Danger.Deadly), pawn.Map, out target))
                {
                    return null;
                }
            }

            Job job = JobMaker.MakeJob(Globals.AutocleanerGoto, target);
            return job;
        }
    }
}
