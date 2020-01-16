using System;
using UnityEngine;

namespace Entities
{
    public class Enemy : LiveEntity
    {
        protected EnemyObject objectScript;
        protected float decisionTimerMax;
        protected float decisionTimer;
        protected float aggroRadius;

        public Enemy(Vector2 position, float maxHealth) : base(maxHealth)
        {
            moveState = moveStates.stationary;
            decisionTimerMax = 1.5f;
            decisionTimer = decisionTimerMax;
            aggroRadius = 15;
        }

        protected override void Death()
        {
            UnityEngine.Object.Destroy(attachedObject);
        }

        public override void Update()
        {
            base.Update();
            StandardSeek(Director.player);
            attachedObject.transform.Translate(velocity * Time.deltaTime);
        }

        protected void StandardSeek(Entity target)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            switch (moveState)
            {
                case moveStates.stunned:
                    velocity *= 0.4f;
                    break;
                case moveStates.stationary:
                    decisionTimer = decisionTimerMax;
                    velocity = vectorToTarget.normalized * moveSpeed;
                    moveState = moveStates.active;
                    break;
                case moveStates.active:
                    decisionTimer = decisionTimerMax;
                    velocity = vectorToTarget.normalized * moveSpeed;
                    moveState = moveStates.active;
                    //Debug.Log(velocity);
                    break;
                default:
                    break;
            }
        }
        protected void StepSeek(Entity target)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            decisionTimer -= Time.deltaTime * Mathf.Max(1, aggroRadius / (vectorToTarget.magnitude));
            switch (moveState)
            {
                case moveStates.stunned:
                    velocity *= 0.4f;
                    break;
                case moveStates.stationary:
                    velocity = Vector2.zero;
                    if (decisionTimer <= 0)
                    {
                        decisionTimer = decisionTimerMax;
                        velocity = vectorToTarget.normalized * moveSpeed;
                        moveState = moveStates.active;
                    }
                    break;
                case moveStates.active:
                    if (decisionTimer <= 0)
                    {
                        decisionTimer = decisionTimerMax;
                        velocity = vectorToTarget.normalized * moveSpeed;
                        moveState = moveStates.active;
                        //Debug.Log(velocity);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
