using System;
using UnityEngine;

namespace Entities
{
    public class Player : LiveEntity
    {
        private PlayerObject objectScript;
        private GameObject playerObject;
        private float hookDamage = 5;
        private float hookKnockback = 80;

        public Player(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/PlayerObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<PlayerObject>();
            objectScript.linkedScript = this;

        }

        private void processHit(LiveEntity target, RaycastHit2D collision)
        {
            if (!target.invincible)
            {
                target.TakeDamage(hookDamage);
                Transform t = target.attachedObject.transform;
                Vector2 knockback = collision.normal * -hookKnockback; //new Vector2(t.position.x - cp.point.x, t.position.y - cp.point.y) * 5;
                target.ApplyKnockback(knockback, 0.2f, 0.2f, 0.05f);
            }
        }

        public void parseHookCollisionData(RaycastHit2D[] col)
        {
            foreach (RaycastHit2D i in col)
            {
                if (i.transform.gameObject.tag == "Enemy")
                {
                    processHit(i.transform.gameObject.GetComponent<EnemyObject>().linkedScript, i);
                }
            }
        }

        protected override void Death()
        {
            Debug.Log("Player died!");
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
