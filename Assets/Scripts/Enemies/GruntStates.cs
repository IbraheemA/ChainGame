using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class GruntActiveState : EnemyActiveState
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
        ((Grunt)entity).ChaseDecision(Director.player);
    }
}

public class GruntAttackingState : EnemyActiveState
{
    private float timer;
    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Enter(LiveEntity entity)
    {
        timer = 1;
        entity.velocity = Vector2.zero;
    }
    public override void Exit(LiveEntity entity)
    {

    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
        if(timer <= 0)
        {
            Exit(entity);
            entity.state = new GruntActiveState();
            entity.state.Enter(entity);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}

/*
public class GruntLaunchedState : EnemyLaunchedState
{
    public GruntLaunchedState(float timer, float hitStunTimer) : base(timer, hitStunTimer)
    {
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = new GruntStunnedState(hitStunTimer);
        entity.state.Enter(entity);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
    }
}

public class GruntStunnedState : EnemyStunnedState
{
    public GruntStunnedState(float timer) : base(timer)

    {
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = new GruntActiveState();
        entity.state.Enter(entity);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
    }
}*/
