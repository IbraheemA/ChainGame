using System;
using UnityEngine;

namespace Entities
{
    public class Enemy : Entity
    {
        private EnemyObject objectScript;
        private GameObject enemyObject;

        public Enemy(Vector2 position, float maxHealth) : base(maxHealth)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), new Vector3(position.x, position.y, 0), Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.enemy = this;
        }
    }
}
