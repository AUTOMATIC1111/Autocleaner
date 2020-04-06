using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Autocleaner
{
    public class AutocleanerSettings : ModSettings
    {
        public bool enable = true;


        override public void ExposeData()
        {
            Scribe_Values.Look(ref enable, "enable");
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("AutocleanerEnableName".Translate(), ref enable, "AutocleanerEnableDesc".Translate());
            listing_Standard.End();
        }
    }

}
