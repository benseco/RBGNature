using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public class PhysicsGroup
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

        public void Remove(ICollide actor)
        {
            group.Remove(actor);
        }

        public void Collide(float s)
        {
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    group[i].Collide(s, groupType, group[j]);
                }
            }
        }

        public void Collide(PhysicsGroup other, float s)
        {
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = 0; j < other.group.Count; j++)
                {
                    group[i].Collide(s, groupType, other[j]);
                }
            }
        }

        public ICollide this[int i]
        {
            get { return group[i]; }
        }
    }

    public enum PhysicsGroupType
    {
        Physical,
        EnemyFire,
        AllyFire
    }
}
