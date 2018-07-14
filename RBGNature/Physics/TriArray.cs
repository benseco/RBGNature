using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    class TriArray : PhysicsObject
    {
        private int[,,] array;

        public TriArray(int[,,] array)
        {
            this.array = array;
        }

        public override CollisionResult Collide(Circle c)
        {
            // a . a
            //------- * b
            // b . b



            int xIndex = (int)(c.Center.X / 20);
            int yIndex = (int)(c.Center.Y / 20);
            int xMin = (int)((c.Center.X - c.Radius) / 20);
            int xMax = (int)((c.Center.X + c.Radius) / 20);
            int yMin = (int)((c.Center.Y - c.Radius) / 20);
            int yMax = (int)((c.Center.Y + c.Radius) / 20);

            Console.Write("Index: " + xIndex + ", " + yIndex);
            Console.Write(" | XBounds: " + xMin + " - " + xMax);
            Console.WriteLine(" | YBounds: " + yMin + " - " + yMax);

            for (int i = yMin; i <= yMax; i++)
            {
                if (i < 0 || i >= array.GetLength(0)) continue; // Index is out of bounds

                for (int j = xMin; j <= xMax; j++)
                {
                    if (j < 0 || j >= array.GetLength(1)) continue; // Index is out of bounds

                    if (array[i, j, 0] == 1 ||
                        array[i, j, 1] == 1 ||
                        array[i, j, 2] == 1 ||
                        array[i, j, 3] == 1)
                        return new CollisionResult(true);

                }
            }

            return new CollisionResult(false);
        }

        public override CollisionResult Collide(TriArray triArray)
        {
            //This should never occur as of now, so throw error.
            throw new NotImplementedException();
        }

        public override CollisionResult Collide(PhysicsObject other)
        {
            return other.Collide(this);
        }
    }
}
