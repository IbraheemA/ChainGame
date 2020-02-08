using System;
using UnityEngine;

namespace Entities
{
    public class Player : LiveEntity
    {
        //DECLARING VARIABLES
        private PlayerObject objectScript;
        public float hookKnockback = 80;
        public float hookMass = 10;

        public float shootSpeed = 0;
        public Vector2 hookVelocity;
        public float initialAngle = 0;
        public float targetAngle = 0;
        public float rotateTarget = 0;
        public float rotationPercentage;
        public float hookSize;
        public float moveAngle = 0;
        public float hookAngle = 0;
        public float lastHookAngle = 0;
        public float lastMoveAngle;
        public HookState hookState = new HookLoadedState();

        public bool movingLastFrame = false;
        public bool hookBackLastFrame = false;

        public enum hState
        {
            fired, loading, loaded
        }
        public GameObject anchor;
        public GameObject hook;

        public Player(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/PlayerObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<PlayerObject>();
            objectScript.linkedScript = this;
            attachedObject.GetComponent<Identifier>().linkedScript = this;

            state = new ActiveState();

            //TODO: DO THIS BETTER LATER 1 (cont on "2")
            targets.Add(typeof(Grunt));
            targets.Add(typeof(Chaser));

            anchor = attachedObject.transform.GetChild(0).gameObject;
            hook = anchor.transform.GetChild(0).gameObject;
            hookSize = hook.GetComponent<CircleCollider2D>().radius;
            moveSpeed = 20;
            damage = 5;
            health = 40;
        }

        public void parseHookCollisionData(RaycastHit2D[] col)
        {
            foreach (RaycastHit2D i in col)
            {
                if (null != i.transform.gameObject.GetComponent<Identifier>())
                {
                    LiveEntity target = i.transform.gameObject.GetComponent<Identifier>().linkedScript;

                    //TODO: DO THIS BETTER LATER 2 (REPLACE GETTYPE with getsubclass or something)
                    if (targets.Contains(target.GetType()))
                    {
                        hookState.ProcessHit(this, target, i);
                    }
                };
            }
        }

        public override void Update()
        {
            base.Update();
        }

        protected override void Death()
        {
            Debug.Log("Player died!");
        }
    }
}
