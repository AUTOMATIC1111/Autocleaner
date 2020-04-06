using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Autocleaner
{
    class JobGiverWaitForRepair : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            if (cleaner == null) return null;
            if (!cleaner.active) return null;
            if (cleaner.health.summaryHealth.SummaryHealthPercent >= 1) return null;
            if (cleaner.charge < cleaner.AutoDef.charge * 0.75f) return null;

            Job job = JobMaker.MakeJob(Globals.AutocleanerWaitForRepair);
            return job;
        }
    }
}
