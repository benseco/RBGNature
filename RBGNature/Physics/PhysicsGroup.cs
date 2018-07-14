using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    class PhysicsGroup
    {
        private List<ICollide> group;
        private PhysicsGroupType groupType;

        public PhysicsGroup(PhysicsGroupType groupType)
        {
            this.groupType = groupType;
            this.group = new List<ICollide>();
        }

        public void Add(ICollide actor)
        {
            group.Add(actor);
        }

        public void Collide()
        {
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    ICollide a = group[i];
                    ICollide b = group[j];
                    //CollisionResult result = PhysicsObject.Collide(
                    //    a.GetCollisionObject(groupType),
                    //    b.GetCollisionObject(groupType));

                    CollisionResult result = a.GetCollisionObject(groupType).Collide(b.GetCollisionObject(groupType));
                    if (result.IsCollision)
                    {
                        a.OnCollide(groupType, result);
                        b.OnCollide(groupType, result);
                    }
                }
            }
        }
    }

    enum PhysicsGroupType
    {
        Physical,
        EnemyFire,
        AllyFire
    }
}
