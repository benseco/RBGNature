using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    struct CollisionResult
    {
        public bool IsCollision;

        public CollisionResult(bool isCollision)
        {
            IsCollision = isCollision;
        }
    }
}
