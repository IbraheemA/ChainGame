using System;
using UnityEngine;

namespace Entities
{
    public abstract class Enemy : LiveEntity
    {
        public float aggroRadius;
        public float attackRadius;
        public Enemy()
        {
        }

        protected override void Death()
        {
            UnityEngine.Object.Destroy(attachedObject);
        }

        public override void Update()
        {
            base.Update();
        }

        public virtual bool ProcessHit(LiveEntity target)
        {
            return false;
        }
        public virtual bool ProcessHit(LiveEntity target, GameObject hitBox)
        {
            return false;
        }

        public abstract void Attack(LiveEntity target);
    }
}
