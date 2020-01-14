using UnityEngine;

namespace Entities
{
    public abstract class Entity
    {
        public void TakeDamage(float damageAmount)
        {
            Debug.Log("Taking damage: " + damageAmount);
        }
    }
}

