using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Chaser : Enemy
    {
        private ChaserObject objectScript;
        public Chaser(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/ChaserObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<ChaserObject>();
            objectScript.linkedScript = this;
            attachedObject.GetComponent<Identifier>().linkedScript = this;
            StatesList.Add("active", typeof(ChaserActiveState));
            StatesList.Add("launched", typeof(EnemyLaunchedState));
            StatesList.Add("stunned", typeof(EnemyStunnedState));

            state = (State)Activator.CreateInstance(StatesList["active"]);

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
        }

        protected override void Attack(Entity target)
        {

        }
    }
}
