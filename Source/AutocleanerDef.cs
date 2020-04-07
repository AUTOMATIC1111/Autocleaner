using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner
{
    public class AutocleanerDef : ThingDef
    {
        public float charge = 100f;
        public int dischargePeriodTicks = 30;
        public ThingDef charger;
    }
}
