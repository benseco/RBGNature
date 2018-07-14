using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    class Circle : PhysicsObject
    {
        public double Radius { get; set; }
        public Vector2 Center { get; set; }

        public override CollisionResult Collide(Circle c)
        {
            if (Vector2.Distance(Center, c.Center) < Radius + c.Radius) return new CollisionResult();
            return new CollisionResult(false);
        }

        public override CollisionResult Collide(TriArray triArray)
        {
            //Implemented in TriArray.cs
            return triArray.Collide(this);
        }

        public override CollisionResult Collide(PhysicsObject other)
        {
            return other.Collide(this);
        }
    }
}
