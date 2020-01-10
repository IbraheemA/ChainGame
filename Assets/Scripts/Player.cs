using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int moveX, moveY;
    private bool moving, loaded, pathSet, movingLastFrame;
    private float xSpeed, ySpeed, moveSpeed, shootSpeed, rotateSpeed, maxRotateSpeed;
    private float swingMomentum, initialAngle, finalAngle, rotateTarget, rotationPercentage;
    private float moveAngle, hookAngle, lastMoveAngle;
    private GameObject hook, anchor;

    void Awake () {
        moveX = 0; moveY = 0; moving = false; movingLastFrame = false; loaded = true; pathSet = false;
        xSpeed = 0; ySpeed = 0;
        moveSpeed = 10f;
        shootSpeed = 0; lastMoveAngle = 0;
        moveAngle = 0; hookAngle = 0;
        initialAngle = 0; finalAngle = 0;
        swingMomentum = 0; rotateTarget = 0;
        //TODO: Possibly make anchor and hook finding nicer?
        anchor = transform.GetChild(0).gameObject;
        hook = anchor.transform.GetChild(0).gameObject;
    }

	void Update () {
        moveX = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        moveY = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        moving = moveX != 0 || moveY != 0;
        float speedMod = (moveX != 0 && moveY != 0) ? 1 / Mathf.Sqrt(2) : 1;
        
        float hookAngleIn360 = (hookAngle + 360) % 360;
        float eulersZ = anchor.transform.localRotation.eulerAngles.z;
        float eulersIn360 = (eulersZ + 360) % 360;
        float angleDiff = (hookAngleIn360 - eulersIn360 + 360) % 360;
        float rotationTargetDirection = (angleDiff < 180) ? 1 : -1; //TODO: understand this better

        if (moveX != 0 || moveY != 0) { moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * moveX, 10 * moveY)); }
        if (moveAngle != lastMoveAngle) { pathSet = false; }
        lastMoveAngle = moveAngle;
        hookAngle = moveAngle - Mathf.Sign(moveAngle) * 180;
        if (!movingLastFrame) { swingMomentum = Mathf.Max(swingMomentum, 0); }
        movingLastFrame = true;

        //TODO: understand this better
        float diff = (hookAngle - eulersZ + 180) % 360 - 180;
        diff = Mathf.Abs(diff < -180 ? diff + 360 : diff);

        if (!pathSet)
        {
            if (diff == 0)
            {
                lastMoveAngle = moveAngle;
            }
            else
            {
                pathSet = true;
                initialAngle = eulersZ;
                finalAngle = hookAngle;
                rotationPercentage = 0;
            }
        }
        else
        {
            rotationPercentage = Mathf.Min(1, rotationPercentage + Time.deltaTime / 0.4f);
            if(rotationPercentage == 1) { pathSet = false; }
        }
        rotateTarget = Mathf.LerpAngle(initialAngle, finalAngle, 1 - Mathf.Pow((rotationPercentage - 1), 2));
        anchor.transform.localRotation = Quaternion.Euler(0, 0, rotateTarget);

        if (Input.GetKey("space"))
        {
            if (loaded)
            {
                shootSpeed = 0.2f;
            }
        }
        else
        {
            shootSpeed -= 0.02f;
        }
        hook.transform.localPosition = new Vector2(Mathf.Clamp(hook.transform.localPosition.x + shootSpeed, 0.2f, 1.5f), 0);

        float appliedSpeed = speedMod * moveSpeed;

        xSpeed = (moveX != 0) ? xSpeed + moveX * 1.5f : Mathf.Sign(xSpeed)*Mathf.Max(0, Mathf.Abs(xSpeed) - 1);
        xSpeed = Mathf.Clamp(xSpeed, -appliedSpeed, appliedSpeed);
        ySpeed = (moveY != 0) ? ySpeed + moveY * 1.5f : Mathf.Sign(ySpeed) * Mathf.Max(0, Mathf.Abs(ySpeed) - 1);
        ySpeed = Mathf.Clamp(ySpeed, -appliedSpeed, appliedSpeed);

        transform.Translate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0);
    }
}
