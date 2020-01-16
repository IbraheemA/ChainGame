using System;
using UnityEngine;

namespace Entities
{
    public class Enemy : LiveEntity
    {
        protected EnemyObject objectScript;

        public Enemy()
        {
            moveState = moveStates.stationary;
        }

        protected override void Death()
        {
            UnityEngine.Object.Destroy(attachedObject);
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
