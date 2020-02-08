using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Grunt : Enemy
    {
        private GruntObject objectScript;
        public Grunt(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<GruntObject>();
            objectScript.linkedScript = this;
            attachedObject.GetComponent<Identifier>().linkedScript = this;
            StatesList.Add("active", typeof(GruntActiveState));
            StatesList.Add("launched", typeof(LaunchedState));
            StatesList.Add("stunned", typeof(StunnedState));

            state = (State)Activator.CreateInstance(StatesList["active"]);

            //BEHAVIOUR VALUES
            mass = 10;
            decisionTimerMax = 1.5f;
            decisionTimer = 0;
            aggroRadius = 15;
            attackRadius = 3;

            //STATS
            maxHealth = 20;
            health = maxHealth;
            moveSpeed = 10;
        }

        public override void Update()
        {
            base.Update();
        }
        protected override void Attack()
        {
            objectScript.Attack();
        }

        public void ChaseDecision(Entity target)
        {
            GameObject ao = attachedObject;
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            decisionTimer -= Time.deltaTime * Mathf.Max(1, aggroRadius / (vectorToTarget.magnitude));
            if ((ao.transform.position - target.attachedObject.transform.position).magnitude < attackRadius)
            {
                LookAt(target);
                decisionTimer = 1;//the 1 here is the attack timer

            }
            else
            {
                if (decisionTimer <= 0)
                {
                    StandardSeek(target);
                    decisionTimer = decisionTimerMax;
                }
            }
        }
    }
}
