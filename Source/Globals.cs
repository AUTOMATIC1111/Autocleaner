using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner
{
    [DefOf]
    public static class Globals
    {
        public static WorkGiverDef AutocleanerCleanFilth;

        public static JobDef AutocleanerClean;
        public static JobDef AutocleanerGoto;
        public static JobDef AutocleanerCharge;
        public static JobDef AutocleanerWaitForRepair;
        public static JobDef AutocleanerRepair;

        public static ThingDef AutocleanerItem;
        public static ThingDef Autocleaner;

        public static PawnKindDef AutocleanerPawnKind;

        public static FleshTypeDef AutocleanerMechanoid;
    }
}
