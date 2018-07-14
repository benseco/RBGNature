using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    interface ICollide
    {
        PhysicsObject GetCollisionObject(PhysicsGroupType groupType);
        void OnCollide(PhysicsGroupType groupType, CollisionResult collisionResult);
    }
}
