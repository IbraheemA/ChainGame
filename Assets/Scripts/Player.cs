using System;
using UnityEngine;

namespace Entities
{
    public class Player : LiveEntity
    {
        private PlayerObject objectScript;
        private GameObject playerObject;
        private float hookDamage = 2;
        private float hookKnockback = 5;

        public Player(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/PlayerObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<PlayerObject>();
            objectScript.player = this;

        }

        private void processHit(LiveEntity target, RaycastHit2D collision)
        {
            if (objectScript.hookState == PlayerObject.hState.fired)
            {
                target.TakeDamage(hookDamage);
                Transform t = target.attachedObject.transform;
                Vector2 knockback = collision.normal * -hookKnockback; //new Vector2(t.position.x - cp.point.x, t.position.y - cp.point.y) * 5;
                target.ApplyKnockback(knockback, true, 0.2f);
            }
        }

        public void parseHookCollisionData(RaycastHit2D[] col)
        {
            foreach (RaycastHit2D i in col)
            {
                if (i.transform.gameObject.tag == "Enemy")
                {
                    processHit(i.transform.gameObject.GetComponent<EnemyObject>().enemy, i);
                }
            }
        }

        protected override void Death()
        {
            Debug.Log("Player died!");
        }

        public override void Update()
        {
        }
    }
}
