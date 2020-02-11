using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class ChaserActiveState : EnemyActiveState
{
    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {

    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
        Chaser chaser = (Chaser)entity;
        LiveEntity target = Director.player;
        if (Director.playerIsAlive)
        {
            if ((chaser.attachedObject.transform.position - target.attachedObject.transform.position).magnitude < chaser.attackRadius)
            {
                chaser.Attack(target);
            }
            else
            {
                entity.StandardSeek(target);
            }
        }
    }
}

public class ChaserAttackingState : EnemyActiveState
{
    //private float timer;
    private int frame;
    private LiveEntity target;
    private bool successfulHit;
    private GameObject hitBox;

    public ChaserAttackingState(LiveEntity target)
    {
        this.target = target;
    }
    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Enter(LiveEntity entity)
    {
        successfulHit = false;
        frame = 30;
        entity.velocity = Vector2.zero;
        hitBox = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/ChaserAttackHitbox"), entity.attachedObject.transform);
    }
    public override void Exit(LiveEntity entity)
    {
        Object.Destroy(hitBox);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
        if (frame <= 0)
        {
            Exit(entity);
            entity.state = new ChaserActiveState();
            entity.state.Enter(entity);
        }
        else if (frame >= 5 && frame <= 20)
        {
            if (!successfulHit && Director.playerIsAlive)
            {
                successfulHit = ((Chaser)entity).ProcessHit(target, hitBox);
            }
        }

        //timer -= Time.deltaTime;
        frame--;
    }
}