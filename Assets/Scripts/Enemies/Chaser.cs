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
            attackRadius = 2.5f;

            //STATS
            Stats["maxHealth"] = 8;
            Stats["moveSpeed"] = 15;
            Stats["damage"] = 3;
            Stats["health"] = Stats["maxHealth"];
        }

        public override void Update()
        {
            base.Update();
        }

        public override bool ProcessHit(LiveEntity target, GameObject hitBox)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            if (!target.invincible && hitBox.GetComponent<Collider2D>().IsTouching(target.attachedObject.GetComponent<Collider2D>()))
            {
                target.ApplyKnockback(vectorToTarget.normalized * 600, 0.02f, 0.007f, 0.1f);
                target.TakeDamage(Stats["damage"]);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Attack(LiveEntity target)
        {
            LookAt(target);
            state.Exit(this);
            state = new ChaserAttackingState(target);
            state.Enter(this);
        }
    }
}
