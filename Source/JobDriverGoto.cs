using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;

namespace Autocleaner
{
    class JobDriverGoto : JobDriver_Goto
    {

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return new Toil()
            {
                initAction = delegate
                {
                    pawn.Rotation = pawn.Rotation.Opposite;
                }
            };
            yield break;
        }
    }
}
