using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Autocleaner
{
    public class CompAutocleanerPowerTrader : CompPowerTrader
    {

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
           
        }

        public override void PostDeSpawn(Map map)
        {
            map.powerNetManager.Notify_ConnectorDespawned(this);
            map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlagDefOf.PowerGrid, true, false);
        }
        public override void LostConnectParent()
        {

        }

        public override void PostDraw()
        {
            
        }
    }
}
