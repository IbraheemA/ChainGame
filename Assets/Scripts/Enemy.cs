using System;
using UnityEngine;

namespace Entities
{
    public class Enemy : LiveEntity
    {
        private EnemyObject objectScript;

        public Enemy(Vector2 position, float maxHealth) : base(maxHealth)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.linkedScript = this;
        }

        protected override void Death()
        {
            UnityEngine.Object.Destroy(attachedObject);
        }

        public override void Update()
        {
            base.Update();
            if(moveState != moveStates.knocked)
            {
                velocity *= 0.4f;
            }
        }
    }
}
