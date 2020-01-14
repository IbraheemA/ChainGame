using System;
using UnityEngine;

namespace Entities
{
    public class Enemy : LiveEntity
    {
        private EnemyObject objectScript;
        private GameObject enemyObject;

        public Enemy(Vector2 position, float maxHealth) : base(maxHealth)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.enemy = this;
        }

        protected override void Death()
        {
            UnityEngine.Object.Destroy(enemyObject);
        }

        public override void Update()
        {
        }
    }
}
