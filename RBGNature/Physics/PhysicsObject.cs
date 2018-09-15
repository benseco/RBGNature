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
        /// public override CollisionResult Collide(PhysicsObject other) { return other.Collide(this).Switch(); }
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

        /// <summary>
        /// Calculates the time of intersection between two line segments
        /// </summary>
        /// <param name="v1">Initial endpoint of line segment V</param>
        /// <param name="v2">Final endpoint of line segment V</param>
        /// <param name="w1">Initial endpoint of line segment W</param>
        /// <param name="w2">Final endpoint of line segment W</param>
        /// <param name="tv">The fractional time of collision along V</param>
        /// <param name="tw">The fractional time of collision along W</param>
        /// <returns>True if time was able to be calculated</returns>
        public static bool TimeOfIntersection(Vector2 v1, Vector2 v2, Vector2 w1, Vector2 w2, out double tv, out double tw)
        {
            // Derived from http://paulbourke.net/geometry/pointlineplane/Helpers.cs

            tv = -1; tw = -1;

            // Denominator for tv and tw are the same, so store this calculation
            double d =
               (w2.Y - w1.Y) * (v2.X - v1.X)
               -
               (w2.X - w1.X) * (v2.Y - v1.Y);

            double nv =
               (w2.X - w1.X) * (v1.Y - w1.Y)
               -
               (w2.Y - w1.Y) * (v1.X - w1.X);

            double nw =
               (v2.X - v1.X) * (v1.Y - w1.Y)
               -
               (v2.Y - v1.Y) * (v1.X - w1.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If tv and tw were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0) return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            tv = nv / d;
            tw = nw / d;
            
            return true;

        }
    }
}
