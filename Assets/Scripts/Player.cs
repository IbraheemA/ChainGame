using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<HookState> hookStatesList = new List<HookState>();

        public bool movingLastFrame = false;
        public bool hookBackLastFrame = false;

        public GameObject anchor;
        public GameObject hook;

        public Player(Vector2 position) : base()
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/PlayerObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<PlayerObject>();
            objectScript.linkedScript = this;
            attachedObject.GetComponent<Identifier>().linkedScript = this;
            StatesList.Add("active", typeof(ActiveState));
            StatesList.Add("launched", typeof(LaunchedState));
            StatesList.Add("stunned", typeof(StunnedState));

            hookStatesList.Add(new HookLoadedState());

            state = (State)Activator.CreateInstance(StatesList["active"]);

            //TODO: DO THIS BETTER LATER 1 (cont on "2")
            targets.Add(typeof(Grunt));
            targets.Add(typeof(Chaser));

            mass = 10;

            anchor = attachedObject.transform.GetChild(0).gameObject;
            hook = anchor.transform.GetChild(0).gameObject;
            hookSize = hook.GetComponent<CircleCollider2D>().radius;
            Stats["moveSpeed"] = 20;
            Stats["damage"] = 5;
            Stats["health"] = 30;
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
                        hookStatesList.Last().ProcessHit(this, target, i);
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
            UnityEngine.Object.Destroy(attachedObject);
            Director.playerIsAlive = false;
        }
    }
}
