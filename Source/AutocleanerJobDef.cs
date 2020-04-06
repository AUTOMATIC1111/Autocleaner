using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner
{
    public class AutocleanerJobDef : JobDef
    {
        public float dischargePerSecond = 0.1f;
        public float activeDischargePerSecond = 0;
    }
}
