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
    /// <summary>
    /// makes it possible to configure autoclears to flee from threats
    /// </summary>
    [HarmonyPatch(typeof(Pawn_PlayerSettings), "UsesConfigurableHostilityResponse", MethodType.Getter)]
    class PatchPawn_PlayerSettings
    {
        static bool Prefix(ref bool __result, Pawn ___pawn)
        {
            if (___pawn?.RaceProps?.FleshType == Globals.AutocleanerMechanoid)
            {
                __result = true;
                return false;
            }
                
            return true;
        }
    }

}
