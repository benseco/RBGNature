using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor
{
    public static class IActExtensions
    {
        public static float LayerDepth(this IAct iAct, float y)
        {
            return y * 0.0000001f;
        }
    }
}
