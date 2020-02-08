using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities
{
    public abstract class Entity
    {
        public GameObject attachedObject;
        public State state;// = new ActiveState();

        public Entity()
        {
            
        }

        protected List<RaycastHit2D[]> RaycastSweep(Vector2 origin, Vector2 direction, float angle)
        {
            List<RaycastHit2D[]> targets = new List<RaycastHit2D[]>();
            float currentAngle = 0;
            float percentage = 0;
            while(percentage <= 1)
            {
                currentAngle = Mathf.LerpAngle(0, angle, percentage);
                targets.Add(Physics2D.LinecastAll(origin, origin+(Vector2)(Quaternion.Euler(0, 0, currentAngle) * direction)));
                percentage += 0.1f;
            }
            return targets;
        }
    }

    public abstract class LiveEntity : Entity
    {
        //Dictionary<string, float> stats = new Dictionary<string, float>();
        public float maxHealth { get; protected set; }
        public float health { get; protected set; }
        public float moveSpeed { get; protected set; }
        public float damage { get; protected set; }
        public List<Type> targets = new List<Type>();
        public Dictionary<string, Type> StatesList = new Dictionary<string, Type>();

        protected float decisionTimerMax;
        protected float decisionTimer = 0;
        protected float mass;

        public bool invincible { get; protected set; }
        public Vector2 velocity;
        protected float invincibilityTimer = 0, hitStunTimer = 0, launchTimer = 0;
        protected Task launchTask;
        protected CancellationTokenSource launchTaskTokenSource;
        protected moveStates moveState;
        protected float aggroRadius;
        protected float attackRadius;

        public enum moveStates
        {
            active,
            stunned,
            stationary,
            launched
        }

        public LiveEntity()
        {
            moveState = moveStates.stationary;
        }

        public void TakeDamage(float damageAmount)
        {
            //Debug.Log("Taking damage: " + damageAmount);
            health -= damageAmount;
            if (health <= 0)
            {
                Death();
            }
        }

        public void ApplyKnockback(Vector2 knockback, float hitStunDuration, float invincibilityDuration, float launchDuration)
        {
            velocity = knockback/mass;
            invincibilityTimer = invincibilityDuration;
            if (launchDuration > 0)
            {
                state = (State)Activator.CreateInstance(StatesList["launched"], launchDuration, hitStunDuration);
                state.Enter(this);
            }
            else
            {
                state = (State)Activator.CreateInstance(StatesList["stunned"], hitStunDuration);
                state.Enter(this);
            }
            if (invincibilityDuration != 0) { invincible = true; }
        }

        protected abstract void Death();

        public virtual void Update()
        {
            state.Update(this);
            attachedObject.transform.Translate(attachedObject.transform.InverseTransformDirection(velocity) * Time.deltaTime);
            if (invincibilityTimer <= 0) { invincible = false; }
            else { invincibilityTimer -= Time.deltaTime; }
        }

        public void LookAt(Entity target)
        {
            attachedObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, attachedObject.transform.position - target.attachedObject.transform.position);
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

        public void StandardSeek(Entity target)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            velocity = vectorToTarget.normalized * moveSpeed;
            LookAt(target);
        }

        /*protected void StepSeek(Entity target, float aggroRadius)
        {
            //ChaseDecision(target, aggroRadius);
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
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
                        LookAt(target);
                        moveState = moveStates.active;
                    }
                    break;
                case moveStates.active:
                    if (decisionTimer <= 0)
                    {
                        decisionTimer = decisionTimerMax;
                        velocity = vectorToTarget.normalized * moveSpeed;
                        LookAt(target);
                        moveState = moveStates.active;
                        //Debug.Log(velocity);
                    }
                    break;
                default:
                    break;
            }
        }

        public void StepSeek(Vector3 target, float aggroRadius)
        {
            Vector2 vectorToTarget = (target - attachedObject.transform.position);
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
        }*/
    }
}
