using System;
using UnityEngine;

namespace Entities
{
    public class Player : Entity
    {
        private PlayerObject objectScript;
        private GameObject playerObject;
        private float hookDamage = 2;
        private float hookKnockback = 5;

        public Player()
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/PlayerObject"), new Vector3(0, 0, 0), Quaternion.identity);
            objectScript = attachedObject.GetComponent<PlayerObject>();
            objectScript.player = this;

        }

        public void processHit(Entity target, Collision2D collision)
        {
            if (objectScript.hookState == PlayerObject.hState.fired)
            {
                target.TakeDamage(hookDamage);
                ContactPoint2D cp = collision.GetContact(0);
                Transform t = target.attachedObject.transform;
                //Debug.Log(objectScript.hook.GetComponent<Rigidbody2D>().velocity);
                Vector2 knockback = new Vector2(t.position.x - cp.point.x, t.position.y - cp.point.y) * 5;
                target.ApplyKnockback(knockback);
            }
        }
    }
}
