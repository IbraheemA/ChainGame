using Entities;
using UnityEngine;

public abstract class HookState
{
    public virtual void Update(Player player, PlayerInput input)
    {
        if (input.move.x != 0 || input.move.y != 0)
        {
            player.moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * input.move.x, 10 * input.move.y));
        }

        player.lastHookAngle = player.hookAngle;
        player.hookAngle = player.moveAngle - (input.directHookBack ? Mathf.Sign(player.moveAngle) * 180 : 0);

        //HOOK ROTATION
        float eulersZ = player.anchor.transform.localRotation.eulerAngles.z;
        //TODO: understand this better
        float diff = (player.hookAngle - eulersZ + 180) % 360 - 180;
        diff = Mathf.Abs(diff < -180 ? diff + 360 : diff);

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
        if (input.lockHookRotation)
        {
            player.hook.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            player.hook.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public abstract void Enter(Player player);
    public abstract void Exit(Player player);
    public abstract void ProcessHit(Player player, LiveEntity target, RaycastHit2D collision);
}

public class HookLoadedState : HookState
{
    public override void Enter(Player player)
    {

    }
    public override void Exit(Player player)
    {

    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (Input.GetKey("space"))
        {
            Exit(player);
            player.hookState = new HookFiredState();
            player.hookState.Enter(player);
        }
    }
    public override void ProcessHit(Player player, LiveEntity target, RaycastHit2D collision)
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

    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (player.hook.transform.localPosition.x == 0.2f && shootSpeed < 0)
        {
            Exit(player);
            player.hookState = new HookLoadingState();
            player.hookState.Enter(player);
        }
        else
        {
            shootSpeed -= 60 * Time.deltaTime * (!Input.GetKey("space") ? 1 : 0.35f);
        }

        Transform ht = player.hook.transform;
        Vector2 currentPos = ht.position;
        Vector2 nextPos = ht.TransformPoint(new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0));
        player.hookVelocity = nextPos - currentPos;
        player.parseHookCollisionData(Physics2D.CircleCastAll(currentPos, player.hookSize, player.hookVelocity, shootSpeed * Time.deltaTime));
        ht.localPosition = new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0);
    }

    public override void ProcessHit(Player player, LiveEntity target, RaycastHit2D collision)
    {
        if (!target.invincible)
        {
            target.TakeDamage(player.damage);
            Transform t = target.attachedObject.transform;
            Vector2 knockback = (-collision.normal + Mathf.Sign(shootSpeed) * player.hookVelocity.normalized).normalized / 2 * player.hookKnockback * player.hookMass;
            //Vector2 knockback = collision.normal * -hookKnockback;
            target.ApplyKnockback(knockback, 0.2f, 0.2f, 0.05f);
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

    }
    public override void Update(Player player, PlayerInput input)
    {
        base.Update(player, input);
        if (timer <= 0)
        {
            Exit(player);
            player.hookState = new HookLoadedState();
            player.hookState.Enter(player);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    public override void ProcessHit(Player player, LiveEntity target, RaycastHit2D collision)
    {

    }
}