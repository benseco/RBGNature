﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public class CollisionResult
    {
        public static CollisionResult None { get; } = new CollisionResult();

        private bool collides;

        public Vector2 PositionA;
        public Vector2 VelocityA;

        public Vector2 PositionB;
        public Vector2 VelocityB;

        public CollisionIdentity Identity;

        private CollisionResult() { }

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
            if (!collides) return this;

            CollisionResult result = new CollisionResult(PositionB, VelocityB, PositionA, VelocityA);
            result.Identity = Identity;
            return result;
        }
        
        public static implicit operator bool(CollisionResult d)
        {
            return d.collides;
        }
    }
}
