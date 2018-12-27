using Microsoft.Xna.Framework;

namespace RBGNature.Physics
{
    interface ICollide
    {
        void Collide(float s, PhysicsGroupType groupType, ICollide other);
        CollisionResult Collide(float s, PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity);
    }
}
