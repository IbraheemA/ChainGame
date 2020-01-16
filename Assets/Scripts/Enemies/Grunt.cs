using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Grunt : Enemy
    {
        public Grunt(Vector2 position, float maxHealth) : base(position, maxHealth)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.linkedScript = this;
        }
    }
}
