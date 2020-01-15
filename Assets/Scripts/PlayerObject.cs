using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class PlayerObject : MonoBehaviour {

    private int moveX, moveY;
    private bool movingLastFrame;
    private float moveSpeed;
    public float shootSpeed { get; private set; }
    public Vector2 hookVelocity, hookPositionLastFrame;
    private float initialAngle, targetAngle, rotateTarget, rotationPercentage;
    private float hookSize;
    private float moveAngle, hookAngle, lastMoveAngle;
    private float loadTimer;
    public hState hookState { get; private set; }
    public GameObject hook { get; private set; }
    public GameObject anchor { get; private set; }
    public Player linkedScript;

    public enum hState
    {
        fired, loading, loaded
    }

    void Awake () {

        moveX = 0; moveY = 0; movingLastFrame = false; hookState = hState.loaded;
        moveSpeed = 20f;
        loadTimer = 0;
        shootSpeed = 0; lastMoveAngle = 0;
        moveAngle = 0; hookAngle = 0;
        initialAngle = 180; targetAngle = 180;
        rotateTarget = 0;
        //TODO: Possibly make anchor and hook finding nicer?
        anchor = transform.GetChild(0).gameObject;
        hook = anchor.transform.GetChild(0).gameObject;
        hookSize = hook.GetComponent<CircleCollider2D>().radius;
    }

    void Start()
    {
        linkedScript.velocity = new Vector2(0, 0);
    }

    void Update () {

        //MOVEMENT
        moveX = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        moveY = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        float speedMod = (moveX != 0 && moveY != 0) ? 1 / Mathf.Sqrt(2) : 1;
        float appliedSpeed = speedMod * moveSpeed;

        Vector2 v = linkedScript.velocity;
        v.x = (moveX != 0) ? v.x + moveX * 1.5f : Mathf.Sign(v.x) * Mathf.Max(0, Mathf.Abs(v.x) - 1);
        v.x = Mathf.Clamp(v.x, -appliedSpeed, appliedSpeed);
        v.y = (moveY != 0) ? v.y + moveY * 1.5f : Mathf.Sign(v.y) * Mathf.Max(0, Mathf.Abs(v.y) - 1);
        v.y = Mathf.Clamp(v.y, -appliedSpeed, appliedSpeed);

        linkedScript.velocity = v;

        Vector2 currentPos = hook.transform.position;
        transform.Translate(linkedScript.velocity.x * Time.deltaTime, linkedScript.velocity.y * Time.deltaTime, 0);

        if (linkedScript.moveState != LiveEntity.moveStates.stunned)
        {
            if (moveX != 0 || moveY != 0)
            {
                moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * moveX, 10 * moveY));
                linkedScript.moveState = LiveEntity.moveStates.active;
            }
            else
            {
                linkedScript.moveState = LiveEntity.moveStates.stationary;
            }
        }

        hookAngle = moveAngle - Mathf.Sign(moveAngle) * 180;

        //HOOK ROTATION
        float eulersZ = anchor.transform.localRotation.eulerAngles.z;
        //TODO: understand this better
        float diff = (hookAngle - eulersZ + 180) % 360 - 180;
        diff = Mathf.Abs(diff < -180 ? diff + 360 : diff);

        if (moveAngle != lastMoveAngle)
        {
            if (!movingLastFrame)
            {
                moveAngle = lastMoveAngle;
            }
            else
            {
                initialAngle = eulersZ;
                targetAngle = hookAngle;
                rotationPercentage = 0;
            }
        }
        rotationPercentage = Mathf.Min(1, rotationPercentage + Time.deltaTime / 0.4f);
        rotateTarget = Mathf.LerpAngle(initialAngle, targetAngle, 1 - Mathf.Pow((rotationPercentage - 1), 2));
        anchor.transform.localRotation = Quaternion.Euler(0, 0, rotateTarget);

        //HOOK PROPULSION
        if (hook.transform.localPosition.x == 0.2f && hookState == hState.fired)
        {
            hookState = hState.loading;
            loadTimer = 0.1f;
        }

        if (hookState == hState.loaded)
        {
            shootSpeed = 0;
            if (Input.GetKey("space")) {
                shootSpeed = 9;
                hookState = hState.fired;
            }
        }
        else
        {
            shootSpeed -= 60 *Time.deltaTime * (!Input.GetKey("space") ? 1: 0.35f);
        }

        if (hookState == hState.loading && (loadTimer <= 0 || !Input.GetKey("space")))
        {
            hookState = hState.loaded;
        }
        else
        {
            loadTimer -= Time.deltaTime;
        }

        Transform ht = hook.transform;
        Vector2 nextPos = ht.TransformPoint(new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0));
        Vector2 circleDirection = nextPos - currentPos;
        if (hookState == PlayerObject.hState.fired)
        {
            linkedScript.parseHookCollisionData(Physics2D.CircleCastAll(currentPos, hookSize, circleDirection, shootSpeed * Time.deltaTime));
        }
        ht.localPosition = new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0);

        //TRACKING
        hookVelocity = new Vector2(hook.transform.position.x - hookPositionLastFrame.x, hook.transform.position.y - hookPositionLastFrame.y);
        movingLastFrame = (linkedScript.moveState == LiveEntity.moveStates.active);
        lastMoveAngle = moveAngle;
        hookPositionLastFrame = hook.transform.position;
    }
}
