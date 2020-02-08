using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : State
{

}

public class InControlState : PlayerState
{
    public bool inControl = true;
    public override void Enter(LiveEntity entity)
    {

    }

    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Exit(LiveEntity entity)
    {

    }
    public override void Update(LiveEntity entity)
    {

    }
}

public class NoControlState : PlayerState
{
    public bool inControl = false;
    public override void Enter(LiveEntity entity)
    {

    }

    public override void MakeDecision(LiveEntity entity)
    {

    }

    public override void Exit(LiveEntity entity)
    {

    }
    public override void Update(LiveEntity entity)
    {

    }
}

public class ActiveState : InControlState
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
    }
}

public class LaunchedState : NoControlState
{
    protected float timer, hitStunTimer;

    public LaunchedState(float timer, float hitStunTimer)
    {
        this.timer = timer;
        this.hitStunTimer = hitStunTimer;
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = new StunnedState(hitStunTimer);
        entity.state.Enter(entity);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
        if (timer <= 0)
        {
            Exit(entity);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}

public class StunnedState : NoControlState
{
    protected float timer;

    public StunnedState(float timer)
    {
        this.timer = timer;
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = (State)Activator.CreateInstance(entity.StatesList["active"]);
        entity.state.Enter(entity);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
        entity.velocity *= 0.4f;
        if (timer <= 0)
        {
            Exit(entity);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}