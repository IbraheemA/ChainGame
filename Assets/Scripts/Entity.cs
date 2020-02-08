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
        public State state;// = new EnemyActiveState();

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
        protected float aggroRadius;
        protected float attackRadius;


        public LiveEntity()
        {
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

        public void ApplyKnockback(Vector2 knockback, float hitStunDuration, float launchDuration, float invincibilityDuration)
        {
            velocity = knockback/mass;
            Debug.Log(velocity);
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

        public void StandardSeek(Entity target)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            velocity = vectorToTarget.normalized * moveSpeed;
            LookAt(target);
        }
    }
}
