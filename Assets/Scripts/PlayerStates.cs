using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlayerState : State
{
    public override void Update(LiveEntity entity)
    {
    }
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
        base.Update(entity);
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
        base.Update(entity);
        PlayerInput input = new PlayerInput();
        input.GetInput();
        ((Player)entity).hookStatesList.Last().Update((Player)entity, input);
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
        Player player = (Player)entity;
        //INPUTS
        PlayerInput input = new PlayerInput();
        input.GetInput();
        float speedMod = (input.move.x != 0 && input.move.y != 0) ? 1 / Mathf.Sqrt(2) : 1;
        float appliedSpeed = speedMod * player.Stats["moveSpeed"];

        //MOVEMENT
        Vector2 v = entity.velocity;
        v.x = (input.move.x != 0) ? v.x + input.move.x * 1.5f : Mathf.Sign(v.x) * Mathf.Max(0, Mathf.Abs(v.x) - 60 * Time.deltaTime);
        v.x = Mathf.Clamp(v.x, -appliedSpeed, appliedSpeed);
        v.y = (input.move.y != 0) ? v.y + input.move.y * 1.5f : Mathf.Sign(v.y) * Mathf.Max(0, Mathf.Abs(v.y) - 60 * Time.deltaTime);
        v.y = Mathf.Clamp(v.y, -appliedSpeed, appliedSpeed);

        player.velocity = v;

        player.hookStatesList.Last().Update(player, input);

        //TRACKING
        player.movingLastFrame = (input.move.x != 0 || input.move.y != 0);
        player.hookBackLastFrame = input.directHookBack;
        player.lastMoveAngle = player.moveAngle;
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