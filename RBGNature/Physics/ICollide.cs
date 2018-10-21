using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    interface ICollide
    {
        void Collide(PhysicsGroupType groupType, ICollide other);
        CollisionResult Collide(PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity);
    }
}
