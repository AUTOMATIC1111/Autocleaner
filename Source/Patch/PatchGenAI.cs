using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Autocleaner.Patch
{
    [HarmonyPatch(typeof(GenAI), "MachinesLike")]
    class PatchGenAI
    {
        static bool Prefix(ref bool __result, Faction machineFaction, Pawn p)
        {
            if ((p.Faction == machineFaction || p.Faction == null) && p.CurJobDef == Globals.AutocleanerClean)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
