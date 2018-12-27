﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor.Scene
{
    abstract class BaseScene : ParentActor
    {
        public Camera Camera { get; protected set; }
        public Color Atmosphere { get; protected set; } = Color.White;
    }
}
