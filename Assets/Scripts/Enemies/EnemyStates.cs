﻿using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void Enter(LiveEntity entity);
    public abstract void Exit(LiveEntity entity);
    public abstract void Update(LiveEntity entity);
    public abstract void MakeDecision(LiveEntity entity);
}

public abstract class EnemyState : State
{
    //public abstract void Update(LiveEntity entity);
}

public class EnemyInControlState : EnemyState
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

public class EnemyNoControlState : EnemyState
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

public class EnemyActiveState : EnemyInControlState
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

public class EnemyLaunchedState : EnemyNoControlState
{
    protected float timer, hitStunTimer;

    public EnemyLaunchedState(float timer, float hitStunTimer)
    {
        this.timer = timer;
        this.hitStunTimer = hitStunTimer;
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = new EnemyStunnedState(hitStunTimer);
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

public class EnemyStunnedState : EnemyNoControlState
{
    protected float timer;

    public EnemyStunnedState(float timer)
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