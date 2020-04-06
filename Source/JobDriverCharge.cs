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
            CompPower comp = PowerConnectionMaker.BestTransmitterForConnector(pawn.Position, pawn.Map);
            return comp != null;
        }

        PawnAutocleaner cleaner => pawn as PawnAutocleaner;
        AutocleanerJobDef def => job.def as AutocleanerJobDef;
        CompPowerTrader compReceiver => pawn.TryGetComp<CompPowerTrader>();

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (cleaner == null || def == null || compReceiver == null || Map == null || Map.powerNetManager == null)
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }
            Map.powerNetManager.Notify_ConnectorWantsConnect(compReceiver);
            int ticksWaitedForPower = 0;

            yield return new Toil()
            {
                initAction = delegate ()
                {
                    Map.pawnDestinationReservationManager.Reserve(pawn, job, pawn.Position);
                    pawn.pather.StopDead();
                },
                tickAction = delegate ()
                {
                    if (!compReceiver.PowerOn)
                    {
                        if(ticksWaitedForPower++ > 60)
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
                    PowerConnectionMaker.DisconnectFromPowerNet(compReceiver);
                } },
                defaultCompleteMode = ToilCompleteMode.Never,
            };

            yield break;
        }
    }
}
