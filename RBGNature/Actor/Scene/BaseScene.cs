using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor.Scene
{
    public abstract class BaseScene : ParentActor
    {
        public Camera Camera { get; protected set; } = new Camera();
        public Color Atmosphere { get; protected set; } = Color.White;
        public abstract void Regenerate();

        public BaseScene NextScene { get; protected set; }
        public void Switched() { NextScene = null; }
        
        public sealed override bool Dead()
        {
            return false;
        }

        /* In addition, all BaseScene inheritors should implement:
         * 1) A singleton called Instance 
         * 2) A private constructor for initial setup 
         */
    }
}
