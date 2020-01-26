using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Grunt : Enemy
    {
        private float aggroRadius, attackRadius;
        public Grunt(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.linkedScript = this;

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
            if ((attachedObject.transform.position - Director.player.attachedObject.transform.position).magnitude < attackRadius)
            {
                LookAt(Director.player);
            }
            else
            {
                //ChaseDecision(Director.player, aggroRadius);
                StepSeek(Director.player, aggroRadius);
                attachedObject.transform.Translate(attachedObject.transform.InverseTransformDirection(velocity) * Time.deltaTime);
            }
        }
    }
}
