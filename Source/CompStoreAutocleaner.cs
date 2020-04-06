using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner
{
    public class CompStoreAutocleaner : ThingComp
    {
        public Thing containedThing;

        public override void PostExposeData()
        {
            Scribe_References.Look(ref containedThing, "containedThing");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "AutocleanerActivate".Translate(),
                defaultDesc = "AutocleanerActivateDesc".Translate(),
                action = delegate
                {
                    IntVec3 pos = parent.Position;
                    Map map = parent.Map;

                    parent.Destroy();

                    PawnAutocleaner autocleaner;
                    if (containedThing == null)
                    {
                        PawnGenerationRequest req = new PawnGenerationRequest(Globals.AutocleanerPawnKind, Faction.OfPlayer);
                        req.FixedBiologicalAge = req.FixedChronologicalAge = 0;
                        Pawn pawn = PawnGenerator.GeneratePawn(req);

                        autocleaner = GenSpawn.Spawn(pawn, pos, map) as PawnAutocleaner;
                    }
                    else
                    {
                        autocleaner = GenSpawn.Spawn(containedThing, pos, map) as PawnAutocleaner;
                    }

                    Find.Selector.SelectedObjects.Add(autocleaner);
                },
                icon = TexCommand.Install,
            };
        }

    }
}
