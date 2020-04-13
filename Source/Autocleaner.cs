using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Autocleaner
{
    public class Autocleaner : Mod
    {
        public static AutocleanerSettings settings;

        public Autocleaner(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<AutocleanerSettings>();

            var harmony = new Harmony("com.github.automatic1111.autocleaner");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "AutocleanerTitle".Translate();
        }
    }

}
