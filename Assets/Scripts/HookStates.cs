using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class HookState
{
    public virtual void Update(Player player, PlayerInput input)
    {
        player.hookHeldTimer = Mathf.Min(player.hookHeldTimer + Time.deltaTime, player.hookHeldTimerMax);
        player.currentPos = player.hook.transform.position;
        for (int i = 0; i < player.hookNodes.Count; i++)
        {
            player.currentNodePositions[i] = player.hookNodes[i].transform.position;
        }

        if (input.move.x != 0 || input.move.y != 0)
        {
            player.moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * input.move.x, 10 * input.move.y));
        }

        player.lastHookAngle = player.hookAngle;
        player.hookAngle = player.moveAngle - (input.directHookBack ? Mathf.Sign(player.moveAngle) * 180 : 0);

        //HOOK ROTATION
        float eulersZ = player.anchor.transform.localRotation.eulerAngles.z;
        //TODO: understand this better

        if (player.hookAngle != player.lastHookAngle)
        {
            if (!player.movingLastFrame && player.hookBackLastFrame == input.directHookBack)
            {
                player.moveAngle = player.lastMoveAngle;
                player.hookAngle = player.lastHookAngle;
            }
            else
            {
                player.initialAngle = eulersZ;
                player.targetAngle = player.hookAngle;
                player.rotationPercentage = 0;
            }
        }
        if (!input.lockHookRotation)
        {
            player.rotationPercentage = Mathf.Min(1, player.rotationPercentage + Time.deltaTime / 0.4f);
            player.rotateTarget = Mathf.LerpAngle(player.initialAngle, player.targetAngle, 1 - Mathf.Pow((player.rotationPercentage - 1), 2));
            player.anchor.transform.localRotation = Quaternion.Euler(0, 0, player.rotateTarget);
        }

        //GRAPHICS
        player.hookSource.transform.GetChild(0).gameObject.SetActive(input.lockHookRotation);
        player.hook.transform.GetChild(0).gameObject.SetActive(input.lockHookPropulsion);
    }
    public virtual void AltUpdate(Player player, PlayerInput input)
    {
        player.currentPos = player.hook.transform.position;
        for (int i = 0; i < player.hookNodes.Count; i++)
        {
            player.currentNodePositions[i] = player.hookNodes[i].transform.position;
        }
        player.lastHookAngle = player.hookAngle;
        if (input.hookVector.x != 0 || input.hookVector.y != 0)
        {
            player.hookAngle = Vector2.SignedAngle(Vector2.right, new Vector2(input.hookVector.x, input.hookVector.y));
        }

        //HOOK ROTATION
        float eulersZ = player.anchor.transform.localRotation.eulerAngles.z;
        //TODO: understand this better

        if (player.hookAngle != player.lastHookAngle)
        {
            player.initialAngle = eulersZ;
            player.targetAngle = player.hookAngle;
            player.rotationPercentage = 0;
        }

        if (!input.lockHookRotation)
        {
            player.rotationPercentage = Mathf.Min(1, player.rotationPercentage + Time.deltaTime / 0.4f);
            player.rotateTarget = Mathf.LerpAngle(player.initialAngle, player.targetAngle, 1 - Mathf.Pow((player.rotationPercentage - 1), 2));
            player.anchor.transform.localRotation = Quaternion.Euler(0, 0, player.rotateTarget);
        }

        //GRAPHICS
        player.hookSource.transform.GetChild(0).gameObject.SetActive(input.lockHookRotation);
        player.hook.transform.GetChild(0).gameObject.SetActive(input.lockHookPropulsion);
    }
    public virtual void ParseHookCollisionData(Player player, RaycastHit2D[] col, List<Type> targets, HitProcessor action)
    {
        foreach (RaycastHit2D i in col)
        {
            if (null != i.transform.gameObject.GetComponent<Identifier>())
            {
                LiveEntity target = i.transform.gameObject.GetComponent<Identifier>().linkedScript;

                //TODO: DO THIS BETTER LATER 2 (REPLACE GETTYPE with getsubclass or something)
                if (targets.Contains(target.GetType()))
                {
                    action(player, target, i);
                    //hookStatesList.Last().ProcessHookHit(this, target, i);
                }
            };
        }
    }
    public abstract void Enter(Player player);
    public abstract void Exit(Player player);
    public abstract void ProcessHookHit(Player player, LiveEntity target, RaycastHit2D collision);
    public abstract void ProcessNodeHit(Player player, LiveEntity target, RaycastHit2D collision);
}

public class HookLoadedState : HookState
{
    public override void Enter(Player player)
    {

    }
    public override void Exit(Player player)
    {
        player.hookStatesList.Remove(this);
    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (input.firing && !input.lockHookPropulsion)
        {
            Exit(player);
            player.hookStatesList.Add(new HookFiredState());
            player.hookStatesList.Last().Enter(player);
        }
    }
    public override void ProcessHookHit(Player player, LiveEntity target, RaycastHit2D collision)
    {

    }

    public override void ProcessNodeHit(Player player, LiveEntity target, RaycastHit2D collision)
    {

    }
}

public class HookFiredState : HookState
{
    public float shootSpeed;
    public override void Enter(Player player)
    {
        shootSpeed = 9;
    }
    public override void Exit(Player player)
    {
        player.hookStatesList.Remove(this);
    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (input.lockHookPropulsion && player.hookHeldTimer == player.hookHeldTimerMax)
        {
            shootSpeed = 0;
            player.hookStatesList.Add(new HookHeldState());
            player.hookStatesList.Last().Enter(player);
        }
        else
        {
            if (player.hook.transform.localPosition.x == 0.2f && shootSpeed < 0)
            {
                Exit(player);
                player.hookStatesList.Add(new HookLoadingState());
                player.hookStatesList.Last().Enter(player);
            }
            else
            {
                shootSpeed -= 60 * Time.deltaTime * (input.firing ? 0.35f : 1);
            }

            Transform ht = player.hook.transform;
            ht.localPosition = new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0);
            Vector2 nextPos = (Vector2)ht.position + (player.velocity * Time.deltaTime);
            player.hookVelocity = nextPos - player.currentPos;
            ParseHookCollisionData(player, Physics2D.CircleCastAll(player.currentPos, player.hookSize, player.hookVelocity, player.hookVelocity.magnitude), player.targets, ProcessHookHit);
            int count = player.hookNodes.Count;
            for (int i = 0; i < count; i++)
            {
                Transform htn = player.hookNodes[i].transform;
                Vector2 currentPosI = player.currentNodePositions[i];
                float pos = Mathf.Lerp(0.2f, ht.localPosition.x, ((float)i) / (float)count);
                htn.localPosition = new Vector2(pos, 0);
                Vector2 nextPosI = (Vector2)htn.position + (player.velocity * Time.deltaTime);
                Vector2 nodeVelocity = nextPosI - currentPosI;
                ParseHookCollisionData(player, Physics2D.CircleCastAll(currentPosI, player.hookSize*0.8f, nodeVelocity, nodeVelocity.magnitude), player.targets, ProcessNodeHit);
                //float pos = Mathf.Max(0.2f, ht.localPosition.x - (float)i * 0.22f);

            }
        }
    }

    public override void ProcessHookHit(Player player, LiveEntity target, RaycastHit2D collision)
    {
        if (!target.invincible)
        {
            target.TakeDamage(player.Stats["damage"]);
            Transform t = target.attachedObject.transform;
            Vector2 knockback = (-collision.normal + Mathf.Sign(shootSpeed) * player.hookVelocity.normalized).normalized / 2 * player.hookKnockback * Mathf.Clamp(player.hookVelocity.magnitude, 1, 2) * player.hookMass;
            //Vector2 knockback = collision.normal * -hookKnockback;
            target.ApplyKnockback(knockback, 0.2f, 0.05f, 0.2f);
        }
    }

    public override void ProcessNodeHit(Player player, LiveEntity target, RaycastHit2D collision)
    {
        if (!target.invincible)
        {
            target.TakeDamage(player.Stats["damage"]/2);
            Transform t = target.attachedObject.transform;
            Vector2 knockback = (-collision.normal).normalized / 2 * player.hookKnockback * Mathf.Min(player.hookVelocity.magnitude, 2) * player.hookMass;
            //Vector2 knockback = collision.normal * -hookKnockback;
            target.ApplyKnockback(knockback, 0.15f, 0.03f, 0.1f);
        }
    }
}

public class HookHeldState : HookState
{
    //private float timer;
    public override void Enter(Player player)
    {
        
    }
    public override void Exit(Player player)
    {
        player.hookStatesList.Remove(this);
    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (!input.lockHookPropulsion || player.hookHeldTimer <= 0)
        {
            Exit(player);
        }
        else {
            player.hookHeldTimer -= 2*Time.deltaTime;
            Transform ht = player.hook.transform;
            Vector2 nextPos = (Vector2)ht.position + (player.velocity * Time.deltaTime);
            player.hookVelocity = nextPos - player.currentPos;
            ParseHookCollisionData(player, Physics2D.CircleCastAll(player.currentPos, player.hookSize, player.hookVelocity, player.hookVelocity.magnitude), player.targets, ProcessHookHit);
            int count = player.hookNodes.Count;
            for (int i = 0; i < count; i++)
            {
                Transform htn = player.hookNodes[i].transform;
                Vector2 currentPosI = player.currentNodePositions[i];
                Vector2 nextPosI = (Vector2)htn.position + (player.velocity * Time.deltaTime);
                Vector2 nodeVelocity = nextPosI - currentPosI;
                ParseHookCollisionData(player, Physics2D.CircleCastAll(currentPosI, player.hookSize * 0.8f, nodeVelocity, nodeVelocity.magnitude), player.targets, ProcessNodeHit);

            }
        }
    }

    public override void ProcessHookHit(Player player, LiveEntity target, RaycastHit2D collision)
    {
        if (!target.invincible)
        {
            target.TakeDamage(player.Stats["damage"]);
            Transform t = target.attachedObject.transform;
            Vector2 knockback = (-collision.normal + player.hookVelocity).normalized / 2 * player.hookKnockback * Mathf.Min(player.hookVelocity.magnitude, 2) * player.hookMass;
            //Vector2 knockback = collision.normal * -hookKnockback;
            target.ApplyKnockback(knockback, 0.2f, 0.05f, 0.2f);
        }
    }

    public override void ProcessNodeHit(Player player, LiveEntity target, RaycastHit2D collision)
    {
        if (!target.invincible)
        {
            target.TakeDamage(player.Stats["damage"] / 2);
            Transform t = target.attachedObject.transform;
            Vector2 knockback = (-collision.normal + player.hookVelocity).normalized / 2 * player.hookKnockback * Mathf.Clamp(player.hookVelocity.magnitude, 1, 2) * player.hookMass;
            //Vector2 knockback = collision.normal * -hookKnockback;
            target.ApplyKnockback(knockback, 0.15f, 0.03f, 0.1f);
        }
    }
}

public class HookLoadingState : HookState
{
    private float timer;
    public override void Enter(Player player)
    {
        timer = 0.1f;
    }
    public override void Exit(Player player)
    {
        player.hookStatesList.Remove(this);
    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (timer <= 0)
        {
            Exit(player);
            player.hookStatesList.Add(new HookLoadedState());
            player.hookStatesList.Last().Enter(player);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    public override void ProcessHookHit(Player player, LiveEntity target, RaycastHit2D collision)
    {

    }

    public override void ProcessNodeHit(Player player, LiveEntity target, RaycastHit2D collision)
    {

    }
}