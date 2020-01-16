using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Chaser : Enemy
    {
        private float aggroRadius;
        public Chaser(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.linkedScript = this;

            //BEHAVIOUR VALUES
            decisionTimerMax = 1.5f;
            decisionTimer = 0;
            aggroRadius = 15;

            //STATS
            maxHealth = 20;
            health = maxHealth;
            moveSpeed = 15;
        }

        public override void Update()
        {
            base.Update();
            StandardSeek(Director.player);
            attachedObject.transform.Translate(velocity * Time.deltaTime);
        }
    }
}
