using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Autocleaner
{

    [StaticConstructorOnStartup]
    public class GizmoAutocleaner : Gizmo
    {
        static int ID = 10984689;

        public PawnAutocleaner cleaner;

        public override float Order { get; set; } = 200f;

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(ID, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);

                Rect rectLabel = rect;
                rectLabel.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rectLabel, "AutocleanerCharge".Translate());

                Rect rectBar = rect;
                rectBar.yMin = overRect.height / 2f;
                float fillPercent = cleaner.charge / Mathf.Max(1f, cleaner.AutoDef.charge);
                Widgets.FillableBar(rectBar, fillPercent, FullBarTex, EmptyBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rectBar, cleaner.charge.ToString("F0") + " / " + (cleaner.AutoDef.charge).ToString("F0"));

                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(180f / 255, 130f / 255, 31f / 255));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }

}
