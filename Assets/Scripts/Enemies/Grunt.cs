using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Grunt : Enemy
    {
        private GruntObject objectScript;
        public Grunt(Vector2 position) : base()
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/EnemyObject"), position, Quaternion.identity);
            objectScript = attachedObject.GetComponent<GruntObject>();
            objectScript.linkedScript = this;
            attachedObject.GetComponent<Identifier>().linkedScript = this;
            StatesList.Add("active", typeof(GruntActiveState));
            StatesList.Add("launched", typeof(EnemyLaunchedState));
            StatesList.Add("stunned", typeof(EnemyStunnedState));

            state = (State)Activator.CreateInstance(StatesList["active"]);

            //BEHAVIOUR VALUES
            mass = 10;
            decisionTimerMax = 1.5f;
            aggroRadius = 15;
            attackRadius = 3;

            //STATS
            Stats["maxHealth"] = 20;
            Stats["moveSpeed"] = 10;
            Stats["health"] = Stats["maxHealth"];
            Stats["damage"] = 8;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Attack(LiveEntity target)
        {
            LookAt(target);
            state.Exit(this);
            state = new GruntAttackingState(target);
            state.Enter(this);
        }

        public override bool ProcessHit(LiveEntity target, GameObject hitBox)
        {
            Vector2 vectorToTarget = (target.attachedObject.transform.position - attachedObject.transform.position);
            if (!target.invincible && hitBox.GetComponent<Collider2D>().IsTouching(target.attachedObject.GetComponent<Collider2D>()))
            {
                target.ApplyKnockback(vectorToTarget.normalized * 400, 0.05f, 0.02f, 0.2f);
                target.TakeDamage(Stats["damage"]);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
