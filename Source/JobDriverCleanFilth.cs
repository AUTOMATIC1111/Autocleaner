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
    public class JobDriverCleanFilth : JobDriver
    {
        private Filth Filth
        {
            get
            {
                return (Filth)job.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.A), job, 1, -1, null);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            PawnAutocleaner cleaner = pawn as PawnAutocleaner;
            AutocleanerJobDef def = job.def as AutocleanerJobDef;

            CompPowerTrader comp = pawn.TryGetComp<CompPowerTrader>();
            if (comp != null) PowerConnectionMaker.DisconnectFromPowerNet(comp);

            Toil initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A, null);
            yield return initExtractTargetFromQueue;
            yield return Toils_JobTransforms.SucceedOnNoTargetInQueue(TargetIndex.A);
            yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A, true);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell).JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, initExtractTargetFromQueue).JumpIfOutsideHomeArea(TargetIndex.A, initExtractTargetFromQueue);
            Toil clean = new Toil();
            clean.initAction = delegate ()
            {
                cleaningWorkDone = 0f;
                totalCleaningWorkDone = 0f;
                totalCleaningWorkRequired = Filth.def.filth.cleaningWorkToReduceThickness * Filth.thickness;
            };
            clean.tickAction = delegate ()
            {
                Filth filth = Filth;
                cleaningWorkDone += 1f;
                totalCleaningWorkDone += 1f;

                if (cleaner != null && def != null)
                {
                    cleaner.charge -= def.activeDischargePerSecond / cleaner.AutoDef.dischargePeriodTicks;
                }

                if (cleaningWorkDone > filth.def.filth.cleaningWorkToReduceThickness)
                {
                    filth.ThinFilth();
                    cleaningWorkDone = 0f;
                    if (filth.Destroyed)
                    {
                        clean.actor.records.Increment(RecordDefOf.MessesCleaned);
                        ReadyForNextToil();
                        return;
                    }
                }
            };
            clean.defaultCompleteMode = ToilCompleteMode.Never;
            //            clean.WithEffect(EffecterDefOf.Clean, TargetIndex.A);
            clean.WithProgressBar(TargetIndex.A, () => totalCleaningWorkDone / totalCleaningWorkRequired, true, -0.5f);
            clean.PlaySustainerOrSound(() => SoundDefOf.Interact_CleanFilth);
            clean.JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, initExtractTargetFromQueue);
            clean.JumpIfOutsideHomeArea(TargetIndex.A, initExtractTargetFromQueue);
            yield return clean;
            yield return Toils_Jump.Jump(initExtractTargetFromQueue);
            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref cleaningWorkDone, "cleaningWorkDone", 0f, false);
            Scribe_Values.Look<float>(ref totalCleaningWorkDone, "totalCleaningWorkDone", 0f, false);
            Scribe_Values.Look<float>(ref totalCleaningWorkRequired, "totalCleaningWorkRequired", 0f, false);
        }

        private float cleaningWorkDone;

        private float totalCleaningWorkDone;

        private float totalCleaningWorkRequired;

        private const TargetIndex FilthInd = TargetIndex.A;
    }
}
