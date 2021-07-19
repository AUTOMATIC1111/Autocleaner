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
    public class WorkGiverCleanFilth : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.OnCell;
            }
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Filth);
            }
        }

        public override int MaxRegionsToScanBeforeGlobalSearch
        {
            get
            {
                return 4;
            }
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerFilthInHomeArea.FilthInHomeArea;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced)
        {
            return pawn.Map.listerFilthInHomeArea.FilthInHomeArea.Count == 0;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Filth filth = t as Filth;

            return filth != null && filth.Map != null && filth.Position.InAllowedArea(pawn) && filth.Map.areaManager.Home[filth.Position] && pawn.CanReserve(t, 1, -1, null, forced) && filth.TicksSinceThickened >= MinTicksSinceThickened;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job job = JobMaker.MakeJob(Globals.AutocleanerClean);
            job.AddQueuedTarget(TargetIndex.A, t);
            int num = 15;
            Map map = t.Map;
            Room room = t.GetRoom(RegionType.Set_Passable);
            for (int i = 0; i < 100; i++)
            {
                IntVec3 intVec = t.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && RegionAndRoomQuery.RoomAt(intVec, map, RegionType.Set_Passable) == room)
                {
                    List<Thing> thingList = intVec.GetThingList(map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        Thing thing = thingList[j];
                        if (HasJobOnThing(pawn, thing, forced) && thing != t)
                        {
                            job.AddQueuedTarget(TargetIndex.A, thing);
                        }
                    }
                    if (job.GetTargetQueue(TargetIndex.A).Count >= num)
                    {
                        break;
                    }
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }

        private int MinTicksSinceThickened = 10;
    }
}
