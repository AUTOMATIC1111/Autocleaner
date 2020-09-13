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
        public bool lowQualityPathing = false;
        public bool disableSchedule = false;

        override public void ExposeData()
        {
            Scribe_Values.Look(ref lowQualityPathing, "lowQualityPathing", false);
            Scribe_Values.Look(ref disableSchedule, "disableSchedule", false);
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("AutocleanerLQPathingName".Translate(), ref lowQualityPathing, "AutocleanerLQPathingDesc".Translate());
            listing_Standard.CheckboxLabeled("AutocleanerDisableScheduleName".Translate(), ref disableSchedule, "AutocleanerDisableScheduleDesc".Translate());
            listing_Standard.End();
        }
    }

}
