using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class GruntActiveState : EnemyActiveState
{
    private float decisionTimer;
    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Enter(LiveEntity entity)
    {
        decisionTimer = 0;
    }
    public override void Exit(LiveEntity entity)
    {

    }
    public override void Update(LiveEntity entity)
    {
        Grunt grunt = (Grunt)entity;
        base.Update(grunt);
        GameObject ao = grunt.attachedObject;
        if (Director.playerIsAlive)
        {
            LiveEntity target = Director.player;
            Vector2 vectorToTarget = (target.attachedObject.transform.position - ao.transform.position);
            decisionTimer -= Time.deltaTime * Mathf.Max(1, grunt.aggroRadius / (vectorToTarget.magnitude));
            if ((ao.transform.position - target.attachedObject.transform.position).magnitude < grunt.attackRadius)
            {
                grunt.Attack(target);
            }
            else
            {
                if (decisionTimer <= 0)
                {
                    grunt.StandardSeek(target);
                    decisionTimer = entity.decisionTimerMax;
                }
            }
        }
    }
}

public class GruntAttackingState : EnemyActiveState
{
    //private float timer;
    private int frame;
    private LiveEntity target;
    private bool successfulHit;
    private GameObject hitBox;

    public GruntAttackingState(LiveEntity target)
    {
        this.target = target;
    }
    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Enter(LiveEntity entity)
    {
        //timer = 1;
        successfulHit = false;
        frame = 60;
        entity.velocity = Vector2.zero;
        hitBox = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/ConeHitBox"), entity.attachedObject.transform);
    }
    public override void Exit(LiveEntity entity)
    {
        Object.Destroy(hitBox);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
        if(frame <= 0)
        {
            Exit(entity);
            entity.state = new GruntActiveState();
            entity.state.Enter(entity);
        }
        else if(frame >= 15 && frame <= 30)
        {
            if (!successfulHit && Director.playerIsAlive)
            {
                successfulHit = ((Grunt)entity).ProcessHit(target, hitBox);
            }
        }

        //timer -= Time.deltaTime;
        frame--;
    }
}