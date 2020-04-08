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
    [StaticConstructorOnStartup]
    public class PawnAutocleaner : Pawn
    {
        static public float lowLower = 0.25f;

        public AutocleanerDef AutoDef => def as AutocleanerDef;

        public bool active = true;
        public float charge;

        public bool Broken => health.hediffSet.hediffs.Count > 0;
        public bool LowPower => charge < AutoDef.charge * lowLower;
        public Thing charger = null;

        public PawnAutocleaner()
        {
            if (relations == null) relations = new Pawn_RelationsTracker(this);
            if (thinker == null) thinker = new Pawn_Thinker(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref active, "autocleanerActive");
            Scribe_Values.Look(ref charge, "autocleanerCharge");
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);

            jobs.StopAll(false, true);
        }

        public void StartCharging()
        {
            StopCharging();

            charger = GenSpawn.Spawn(AutoDef.charger, Position, Map);       
        }

        public void StopCharging()
        {
            if (charger == null) return;

            if(charger.Spawned) charger.Destroy();
            charger = null;
        }

        public override void Tick()
        {
            base.Tick();

            if (!this.IsHashIntervalTick(AutoDef.dischargePeriodTicks)) return;

            AutocleanerJobDef job = CurJobDef as AutocleanerJobDef;
            if (job == null) return;

            charge -= job.dischargePerSecond * GenTicks.TicksToSeconds(AutoDef.dischargePeriodTicks);
            if (charge < 0) charge = 0;
            if (charge > AutoDef.charge) charge = AutoDef.charge;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == this)
            {
                yield return new GizmoAutocleaner
                {
                    cleaner = this
                };
            }

            yield return new Command_Action
            {
                defaultLabel = "AutocleanerDeactivate".Translate(),
                defaultDesc = "AutocleanerDeactivateDesc".Translate(),
                action = delegate
                {
                    Thing thing = GenSpawn.Spawn(Globals.AutocleanerItem, Position, Map);
                    CompStoreAutocleaner comp = thing.TryGetComp<CompStoreAutocleaner>();
                    if (comp != null)
                    {
                        comp.containedThing = this;
                    }

                    DeSpawn();

                    Find.Selector.SelectedObjects.Add(thing);
                },
                icon = iconUninstall,
            };

            if (active)
            {
                yield return new Command_Action
                {
                    defaultLabel = "AutocleanerPause".Translate(),
                    defaultDesc = "AutocleanerPauseDesc".Translate(),
                    action = delegate
                    {
                        Log.Message(def + " " + (def as AutocleanerDef));
                        jobs.StopAll(false, true);
                        active = !active;
                    },
                    icon = iconPause,
                };
            }
            else
            {
                yield return new Command_Action
                {
                    defaultLabel = "AutocleanerResume".Translate(),
                    defaultDesc = "AutocleanerResumeDesc".Translate(),
                    action = delegate
                    {
                        active = !active;
                    },
                    icon = iconResume,
                };
            }

            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            yield break;
        }

        public override string GetInspectString()
        {
            if (charger == null) return base.GetInspectString();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length > 0) stringBuilder.AppendLine();
            stringBuilder.Append(charger.GetInspectString());
            return stringBuilder.ToString();
        }

        public override void Draw()
        {
            base.Draw();

            OverlayTypes overlay = OverlayTypes.Forbidden;

            if (Broken) overlay = OverlayTypes.BrokenDown;
            else if (charger != null && CurJobDef == Globals.AutocleanerCharge) overlay = OverlayTypes.NeedsPower;
            else if (LowPower) overlay = OverlayTypes.PowerOff;

            if (overlay != OverlayTypes.Forbidden) Map.overlayDrawer.DrawOverlay(this, overlay);
        }

        static Texture2D iconUninstall = ContentFinder<Texture2D>.Get("UI/Designators/Uninstall", true);
        static Texture2D iconPause = ContentFinder<Texture2D>.Get("Autocleaner/Pause", true);
        static Texture2D iconResume = ContentFinder<Texture2D>.Get("Autocleaner/Resume", true);
    }
}
