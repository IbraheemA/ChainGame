using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class GruntActiveState : ActiveState
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
        GameObject ao = entity.attachedObject;
        base.Update(entity);
        entity.ChaseDecision(Director.player);
    }
}

/*
public class GruntLaunchedState : LaunchedState
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

public class GruntStunnedState : StunnedState
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