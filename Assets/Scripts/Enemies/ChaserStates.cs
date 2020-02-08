﻿using System.Collections;
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
        entity.StandardSeek(Director.player);
    }
}

/*public class ChaserLaunchedState : EnemyLaunchedState
{
    public ChaserLaunchedState(float timer, float hitStunTimer) : base(timer, hitStunTimer)
    {
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = new ChaserStunnedState(hitStunTimer);
        entity.state.Enter(entity);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
    }
}

public class ChaserStunnedState : EnemyStunnedState
{
    public ChaserStunnedState(float timer) : base(timer)

    {
    }
    public override void Enter(LiveEntity entity)
    {

    }
    public override void Exit(LiveEntity entity)
    {
        entity.state = new ChaserActiveState();
        entity.state.Enter(entity);
    }
    public override void Update(LiveEntity entity)
    {
        base.Update(entity);
    }
}*/