using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Autocleaner
{
    class JobDriverWaitForRepair : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            AutocleanerJobDef def = job.def as AutocleanerJobDef;
            if (cleaner == null || def == null)
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }

            yield return new Toil()
            {
                initAction = delegate ()
                {
                    Map.pawnDestinationReservationManager.Reserve(pawn, job, pawn.Position);
                    pawn.pather.StopDead();
                },
                tickAction = delegate ()
                {
                    if (!pawn.IsHashIntervalTick(60)) return;

                    if (cleaner.charge < cleaner.AutoDef.charge * 0.75f)
                    {
                        EndJobWith(JobCondition.InterruptForced);
                        return;
                    }
                    if (cleaner.health.summaryHealth.SummaryHealthPercent >= 1)
                    {
                        EndJobWith(JobCondition.Succeeded);
                        return;
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never,
            };

            yield break;
        }
    }
}
