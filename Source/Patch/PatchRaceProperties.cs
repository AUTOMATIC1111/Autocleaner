using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner.Patch
{
    [HarmonyPatch(typeof(RaceProperties), "IsFlesh", MethodType.Getter)]
    class PatchRaceProperties
    {
        static bool Prefix(ref bool __result, RaceProperties __instance)
        {
            if (__instance.FleshType == Globals.AutocleanerMechanoid)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
