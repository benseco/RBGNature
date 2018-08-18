using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public struct CollisionResult
    {
        public static CollisionResult None { get; } = new CollisionResult();

        private bool collides;

        public readonly Vector2 PositionA;
        public readonly Vector2 VelocityA;

        public readonly Vector2 PositionB;
        public readonly Vector2 VelocityB;

        public CollisionResult(Vector2 positionA, Vector2 velocityA, Vector2 positionB, Vector2 velocityB)
        {
            collides = true;

            PositionA = positionA;
            VelocityA = velocityA;

            PositionB = positionB;
            VelocityB = velocityB;
        }

        public CollisionResult Switch()
        {
            if (collides) return new CollisionResult(PositionB, VelocityB, PositionA, VelocityA);
            else return this;
        }
        
        public static implicit operator bool(CollisionResult d)
        {
            return d.collides;
        }
    }
}
