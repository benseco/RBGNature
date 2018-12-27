using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{
    internal class DialogueBite
    {
        internal readonly string Name;
        internal readonly List<Fragment> Fragments;
        internal readonly Texture2D Portrait;
        internal readonly PlateType Plate;

        internal DialogueBite(string name, List<Fragment> fragments, Texture2D portrait, PlateType plate)
        {
            Name = name;
            Fragments = fragments;
            Portrait = portrait;
            Plate = plate;
        }
    }
}
