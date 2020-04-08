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
            if (comp?.PowerNet == null) return false;

            return comp.PowerNet.HasActivePowerSource;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            if (cleaner == null) return null;
            if (!cleaner.active) return null;

            Map map = pawn.Map;
            if (map == null) return null;
            if (SuitablePosition(pawn.Position, map)) return null;

            IntVec3 target = IntVec3.Invalid;
            PathGrid pathGrid = map.pathGrid;
            CellIndices cellIndices = map.cellIndices;
            Predicate<IntVec3> passCheck = delegate (IntVec3 c)
            {
                if (c.GetTerrain(map).IsWater) return false;
                if (!pathGrid.WalkableFast(cellIndices.CellToIndex(c))) return false;

                return true;
            };
            Func<IntVec3, bool> processor = delegate (IntVec3 x)
            {
                if (!SuitablePosition(x, map)) return false;

                target = x;
                return true;
            };

            map.floodFiller.FloodFill(pawn.Position, passCheck, processor);
            if (target != IntVec3.Invalid)
            {
                Job job = JobMaker.MakeJob(Globals.AutocleanerGoto, target);
                return job;
            }

            Func<IntVec3, bool> processorFallback = delegate (IntVec3 x)
            {
                if (!SuitablePosition(x, map, true)) return false;

                target = x;
                return true;
            };
            map.floodFiller.FloodFill(pawn.Position, passCheck, processor);
            if (target != IntVec3.Invalid)
            {
                Job job = JobMaker.MakeJob(Globals.AutocleanerGoto, target);
                return job;
            }

            return null;
        }
    }
}