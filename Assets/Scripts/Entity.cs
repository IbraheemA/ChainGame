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
        public bool invincible { get; protected set; }
        public Vector2 velocity;
        protected float invincibilityTimer = 0, hitStunTimer = 0, launchTimer = 0;
        public moveStates moveState;
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

        public LiveEntity(float maxHealth)
        {
            moveState = moveStates.stationary;
            this.maxHealth = maxHealth;
            health = maxHealth;
            this.moveSpeed = 10;
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
            velocity = knockback;
            invincibilityTimer = invincibilityDuration;
            hitStunTimer = hitStunDuration;
            launchTimer = launchDuration;
            if (invincibilityDuration != 0) { invincible = true; }
            if (hitStunDuration != 0) { moveState = (launchTimer != 0) ? moveStates.launched : moveStates.stunned; }
        }

        protected abstract void Death();

        public virtual void Update() {
            if(invincibilityTimer <= 0)
            {
                invincible = false;
            }
            else
            {
                invincibilityTimer -= Time.deltaTime;
            }
            if (hitStunTimer <= 0)
            {
                if (moveState == moveStates.stunned) { moveState = moveStates.stationary; }
            }
            else
            {
                hitStunTimer -= Time.deltaTime;
            }

            if (launchTimer <= 0)
            {
                if (moveState == moveStates.launched) { moveState = moveStates.stunned; }
            }
            else
            {
                launchTimer -= Time.deltaTime;
            }
        }
    }
}

