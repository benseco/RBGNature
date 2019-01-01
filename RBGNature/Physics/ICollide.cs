using Microsoft.Xna.Framework;

namespace RBGNature.Physics
{
    public interface ICollide
    {
        void Collide(float s, PhysicsGroupType groupType, ICollide other);
        CollisionResult Collide(float s, PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity);
    }
}
