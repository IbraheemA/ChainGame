using System;
using UnityEngine;

namespace Entities
{
    public abstract class Enemy : LiveEntity
    {
        public Enemy()
        {
            moveState = moveStates.stationary;
        }

        protected override void Death()
        {
            UnityEngine.Object.Destroy(attachedObject);
        }

        public override void Update()
        {
            base.Update();
        }

        public void parseCollisionData(RaycastHit2D[] col)
        {
            foreach (RaycastHit2D i in col)
            {
                LiveEntity target = i.transform.gameObject.GetComponent<Identifier>().linkedScript;
                if (targets.Contains(target.GetType()))
                {
                    processHit(target, i);
                }
            }
        }

        private void processHit(LiveEntity target, RaycastHit2D collision)
        {
            if (!target.invincible)
            {
                target.TakeDamage(damage);
                //Transform t = target.attachedObject.transform;
                //Vector2 knockback = Vector2.down * Quaternion.EulerAngles(attachedObject.transform.rotation);
                //Vector2 knockback = collision.normal * -hookKnockback;
                //target.ApplyKnockback(knockback, 0.2f, 0.2f, 0.05f);
            }
        }

        protected abstract void Attack();
    }
}
