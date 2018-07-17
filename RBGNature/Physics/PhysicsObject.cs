using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public abstract class PhysicsObject
    {
        public abstract CollisionResult Collide(Circle c);
        public abstract CollisionResult Collide(TriArray triArray);
        public abstract CollisionResult Collide(PhysicsObject other);
    }
}
