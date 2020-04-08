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
    public class JobDriverCharge : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        PawnAutocleaner cleaner => pawn as PawnAutocleaner;
        AutocleanerJobDef def => job.def as AutocleanerJobDef;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (cleaner == null || def == null || Map == null || Map.powerNetManager == null)
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }

            int ticksWaitedForPower = 0;
            yield return new Toil()
            {
                initAction = delegate ()
                {
                    Map.pawnDestinationReservationManager.Reserve(pawn, job, pawn.Position);
                    pawn.pather.StopDead();

                    cleaner.StartCharging();
                },
                tickAction = delegate ()
                {
                    if (cleaner.charger == null)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    CompPowerTrader comp = cleaner.charger.TryGetComp<CompPowerTrader>();
                    if (comp == null || !comp.PowerOn)
                    {
                        if(ticksWaitedForPower++ > 20)
                            EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    cleaner.charge -= def.activeDischargePerSecond / cleaner.AutoDef.dischargePeriodTicks;
                    if (cleaner.charge >= cleaner.AutoDef.charge)
                    {
                        cleaner.charge = cleaner.AutoDef.charge;
                        EndJobWith(JobCondition.Succeeded);
                        return;
                    }
                },
                finishActions = new List<Action>() { delegate () {
                    cleaner.StopCharging();
                } },
                defaultCompleteMode = ToilCompleteMode.Never,
            };

            yield break;
        }
    }
}
