﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Grunt : Enemy
    {
        private float aggroRadius;
        public Grunt(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<EnemyObject>();
            objectScript.linkedScript = this;

            //BEHAVIOUR VALUES
            decisionTimerMax = 1.5f;
            decisionTimer = 0;
            aggroRadius = 15;

            //STATS
            maxHealth = 8;
            health = maxHealth;
            moveSpeed = 10;
        }

        public override void Update()
        {
            base.Update();
            StepSeek(Director.player, aggroRadius);
            attachedObject.transform.Translate(velocity * Time.deltaTime);
        }
    }
}
