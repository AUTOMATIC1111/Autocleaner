using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Autocleaner
{
    class JobDriverRepairAutocleaner : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed) && pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            float workNeeded = 1000;
            float workDone = 0;
            Toil repair = new Toil();
            repair.tickAction = delegate ()
            {
                Pawn actor = repair.actor;
                actor.skills.Learn(SkillDefOf.Construction, 0.05f, false);
                float num = actor.GetStatValue(StatDefOf.ConstructionSpeed, true) * 1.7f;
                workDone += num;
                if (workDone >= workNeeded)
                {
                    PawnAutocleaner cleaner = TargetThingA as PawnAutocleaner;
                    if (cleaner == null)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    cleaner.health.Reset();

                    job.GetTarget(TargetIndex.B).Thing.Destroy(DestroyMode.Vanish);
                    actor.records.Increment(RecordDefOf.ThingsRepaired);

                    EffecterDefOf.Deflect_Metal.Spawn().Trigger(job.targetA.Thing, pawn);
                    EndJobWith(JobCondition.Succeeded);
                }
            };
            repair.defaultCompleteMode = ToilCompleteMode.Never;
            repair.activeSkill = () => SkillDefOf.Construction;
            repair.FailOnDespawnedOrNull(TargetIndex.A);
            repair.WithProgressBar(TargetIndex.A, () => workDone / workNeeded, false, -0.5f);
            repair.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            repair.WithEffect(TargetThingA.def.repairEffect, TargetIndex.A);
            yield return repair;
            yield break;
        }
    }
}
