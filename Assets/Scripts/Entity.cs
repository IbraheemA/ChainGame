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
        public float maxHealth { get; private set; }
        public float health { get; private set; }
        public bool invincible { get; private set; }

        public LiveEntity()
        {

        }

        public LiveEntity(float maxHealth)
        {
            this.maxHealth = maxHealth;
            health = maxHealth;
        }
        public void TakeDamage(float damageAmount)
        {
            Debug.Log("Taking damage: " + damageAmount);
            health -= damageAmount;
            if (health <= 0)
            {
                Death();
            }
        }

        public void ApplyKnockback(Vector2 knockback, bool hitStun, float invincibilityDuration)
        {
            attachedObject.transform.Translate(knockback);

        }

        protected abstract void Death();

        public abstract void Update();
    }
}

