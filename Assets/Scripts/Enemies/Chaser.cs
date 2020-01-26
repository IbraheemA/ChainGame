using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Chaser : Enemy
    {
        public Chaser(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/ChaserObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.linkedScript = this;

            //BEHAVIOUR VALUES
            mass = 5;
            decisionTimerMax = 1.5f;
            decisionTimer = 0;

            //STATS
            maxHealth = 8;
            health = maxHealth;
            moveSpeed = 15;
        }

        public override void Update()
        {
            base.Update();
            StandardSeek(Director.player);
            attachedObject.transform.Translate(attachedObject.transform.InverseTransformDirection(velocity) * Time.deltaTime);
        }
    }
}
