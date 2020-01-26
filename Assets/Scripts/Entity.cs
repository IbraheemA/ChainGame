using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities
{
    public abstract class Entity
    {
        public GameObject attachedObject;

        public Entity()
        {
            
        }
    }

    public abstract class LiveEntity : Entity
    {
        public float maxHealth { get; protected set; }
        public float health { get; protected set; }
        public float moveSpeed { get; protected set; }

        protected float decisionTimerMax;
        protected float decisionTimer = 0;
        protected float mass;

        public bool invincible { get; protected set; }
        public Vector2 velocity;
        protected float invincibilityTimer = 0, hitStunTimer = 0, launchTimer = 0;
        protected Task launchTask;
        protected CancellationTokenSource launchTaskTokenSource;
        protected moveStates moveState;
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
            hitStunTimer = hitStunDuration;
            //launchTimer = launchDuration;
            if (invincibilityDuration != 0)
            {
                Task.Run(async () =>
                {
                    invincible = true;
                    await Task.Delay(TimeSpan.FromSeconds(invincibilityDuration));
                    invincible = false;
                });
            }

            if (hitStunDuration != 0)
            {
                Task.Run(async () =>
                {
                    moveState = moveStates.stunned;
                    await Task.Delay(TimeSpan.FromSeconds(hitStunDuration));
                    moveState = moveStates.active;
                });
            }

            if (launchDuration != 0)
            {
                if (launchTaskTokenSource != null) { launchTaskTokenSource.Cancel(); }
                launchTaskTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = launchTaskTokenSource.Token;
                launchTask = Task.Factory.StartNew(async () =>
                {
                    velocity = knockback / mass;
                    await Task.Delay(TimeSpan.FromSeconds(launchDuration));
                    velocity = Vector2.zero;
                }, cancellationToken);
            }
        }

        protected abstract void Death();
 
        public virtual void Update()
        {

        }

        protected void LookAt(Entity target)
        {
            attachedObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, attachedObject.transform.position - target.attachedObject.transform.position);
        }

        protected void ChaseDecision(Entity target, float aggroRadius)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            decisionTimer -= Time.deltaTime * Mathf.Max(1, aggroRadius / (vectorToTarget.magnitude));
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
                    velocity = vectorToTarget.normalized * moveSpeed;
                    LookAt(target);
                    moveState = moveStates.active;
                    break;
                case moveStates.active:
                    velocity = vectorToTarget.normalized * moveSpeed;
                    LookAt(target);
                    moveState = moveStates.active;
                    break;
                default:
                    break;
            }
        }

        protected void StandardSeek(Vector3 target)
        {
            Vector2 vectorToTarget = (target - attachedObject.transform.position);
            switch (moveState)
            {
                case moveStates.stunned:
                    velocity *= 0.4f;
                    break;
                case moveStates.stationary:
                    velocity = vectorToTarget.normalized * moveSpeed;
                    moveState = moveStates.active;
                    break;
                case moveStates.active:
                    velocity = vectorToTarget.normalized * moveSpeed;
                    moveState = moveStates.active;
                    break;
                default:
                    break;
            }
        }

        protected void StepSeek(Entity target, float aggroRadius)
        {
            ChaseDecision(target, aggroRadius);
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            switch (moveState)
            {
                case moveStates.stunned:
                    //velocity *= 0.4f;
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

        protected void StepSeek(Vector3 target, float aggroRadius)
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
        }
    }
}
