using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner.Patch
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), "Pawns", MethodType.Getter)]
    class PatchMainTabWindow_PawnTablePawns
    {
        static IEnumerable<Pawn> Postfix(IEnumerable<Pawn> pawns, MainTabWindow_PawnTable __instance)
        {
            foreach (Pawn pawn in pawns)
            {
                yield return pawn;
            }

            if (Autocleaner.settings.disableSchedule)
            {
                yield break;
            }

            if (!(__instance is MainTabWindow_Restrict))
            {
                yield break;
            }

            foreach (Pawn pawn in Find.CurrentMap.mapPawns.AllPawns.Where(x => x.Faction == Faction.OfPlayer && x.kindDef == Globals.AutocleanerPawnKind && !pawns.Contains(x)))
            {
                yield return pawn;
            }
        }
    }
}
