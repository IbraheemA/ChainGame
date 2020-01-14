using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class PlayerObject : MonoBehaviour {

    private int moveX, moveY;
    private bool moving, movingLastFrame;
    private float xSpeed, ySpeed, moveSpeed, shootSpeed;
    private float initialAngle, targetAngle, rotateTarget, rotationPercentage;
    private float moveAngle, hookAngle, lastMoveAngle;
    private float loadTimer;
    private hState hookState;
    private GameObject hook, anchor;
    public Player player;

    enum hState
    {
        fired, loading, loaded
    }

    void Awake () {
        //VERY TEMP
        player = new Player();

        moveX = 0; moveY = 0; moving = false; movingLastFrame = false; hookState = hState.loaded;
        xSpeed = 0; ySpeed = 0;
        moveSpeed = 20f;
        loadTimer = 0;
        shootSpeed = 0; lastMoveAngle = 0;
        moveAngle = 0; hookAngle = 0;
        initialAngle = 180; targetAngle = 180;
        rotateTarget = 0;
        //TODO: Possibly make anchor and hook finding nicer?
        anchor = transform.GetChild(0).gameObject;
        hook = anchor.transform.GetChild(0).gameObject;
    }

	void Update () {

        //MOVEMENT
        moveX = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        moveY = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        moving = moveX != 0 || moveY != 0;
        float speedMod = (moveX != 0 && moveY != 0) ? 1 / Mathf.Sqrt(2) : 1;
        float appliedSpeed = speedMod * moveSpeed;

        xSpeed = (moveX != 0) ? xSpeed + moveX * 1.5f : Mathf.Sign(xSpeed) * Mathf.Max(0, Mathf.Abs(xSpeed) - 1);
        xSpeed = Mathf.Clamp(xSpeed, -appliedSpeed, appliedSpeed);
        ySpeed = (moveY != 0) ? ySpeed + moveY * 1.5f : Mathf.Sign(ySpeed) * Mathf.Max(0, Mathf.Abs(ySpeed) - 1);
        ySpeed = Mathf.Clamp(ySpeed, -appliedSpeed, appliedSpeed);

        transform.Translate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0);

        if (moveX != 0 || moveY != 0) {
            moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * moveX, 10 * moveY));
            moving = true;
        }
        else
        {
            moving = false;
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

        if (Input.GetKey("space") && hookState == hState.loaded)
        {
            shootSpeed = 9;
            hookState = hState.fired;
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

        hook.transform.localPosition = new Vector2(Mathf.Max(hook.transform.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0);

        movingLastFrame = moving;
        lastMoveAngle = moveAngle;
    }
}
