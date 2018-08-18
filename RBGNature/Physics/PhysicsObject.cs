using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public abstract class PhysicsObject
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Mass;

        // For now, PhysicsObject requires that all deriving classes be able to collide with each other
        public abstract CollisionResult Collide(Circle c);
        public abstract CollisionResult Collide(TriArray triArray);

        /// <summary>
        /// Navigates collision to derived class. Standard implementation is:
        /// public override CollisionResult Collide(PhysicsObject other) { return other.Collide(this).Negate(); }
        /// </summary>
        /// <param name="other">The other Physics object in this collision</param>
        /// <returns></returns>
        public abstract CollisionResult Collide(PhysicsObject other);

        public static Vector2 ClosestOnSegment(Vector2 A, Vector2 B, Vector2 P)
        {
            Vector2 AP = P - A;       //Vector from A to P   
            Vector2 AB = B - A;       //Vector from A to B  

            float magnitudeAB = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return A;

            }
            else if (distance > 1)
            {
                return B;
            }
            else
            {
                return A + AB * distance;
            }
        }
    }
}
