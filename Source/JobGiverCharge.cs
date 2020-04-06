using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Autocleaner
{
    public class JobGiverCharge : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            if (cleaner == null) return null;
            if (!cleaner.active) return null;
            if (cleaner.charge >= cleaner.AutoDef.charge) return null;

            Job job = JobMaker.MakeJob(Globals.AutocleanerCharge);
            return job;
        }
    }
}
