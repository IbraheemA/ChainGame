using System;
using UnityEngine;

namespace Entities
{
    public class Player : Entity
    {
        private PlayerObject objectScript;
        private GameObject playerObject;
        private float hookDamage = 5;

        public Player()
        {

        }

        public void processHit(Entity target)
        {
            target.TakeDamage(hookDamage);
        }
    }
}
