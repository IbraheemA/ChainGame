using UnityEngine;

namespace Entities
{
    public abstract class Entity
    {
        public GameObject attachedObject;
        public float maxHealth { get; private set; }
        public float health { get; private set; }

        public Entity()
        {
            
        }

        public Entity(float maxHealth)
        {
            this.maxHealth = maxHealth;
            health = maxHealth;
        }

        public void TakeDamage(float damageAmount)
        {
            Debug.Log("Taking damage: " + damageAmount);
            health -= damageAmount;
            if(health <= 0)
            {
                Death();
            }
        }

        public void ApplyKnockback(Vector2 knockback)
        {
            attachedObject.transform.Translate(knockback);
        }

        public void Death()
        {
            Object.Destroy(attachedObject);
        }
    }
}

