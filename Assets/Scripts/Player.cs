using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int moveX, moveY;
    private bool moving, loaded, pathSet, movingLastFrame;
    private float xSpeed, ySpeed, moveSpeed, shootSpeed, rotateSpeed, maxRotateSpeed;
    private float swingMomentum, sinShift, sinStretch, swingDistance;
    private float moveAngle, hookAngle, lastMoveAngle;
    private GameObject hook, anchor;

    void Awake () {
        moveX = 0; moveY = 0; moving = false; movingLastFrame = false; loaded = true; pathSet = false;
        xSpeed = 0; ySpeed = 0;
        moveSpeed = 10f;
        shootSpeed = 0; rotateSpeed = 0; maxRotateSpeed = 300; lastMoveAngle = 0;
        moveAngle = 0; hookAngle = 0;
        sinShift = 0; sinStretch = 0; swingDistance = 0;
        swingMomentum = 0;
        //TODO: Possibly make anchor and hook finding nicer?
        anchor = transform.GetChild(0).gameObject;
        hook = anchor.transform.GetChild(0).gameObject;
    }

	void Update () {
        moveX = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        moveY = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        moving = moveX != 0 || moveY != 0;
        float speedMod = (moveX != 0 && moveY != 0) ? 1 / Mathf.Sqrt(2) : 1;

        //Debug.Log("angle: " + moveAngle + " last: " + lastMoveAngle);
        
        float hookAngleIn360 = (hookAngle + 360) % 360;
        float eulersZ = anchor.transform.localRotation.eulerAngles.z;
        float eulersIn360 = (eulersZ + 360) % 360;
        float angleDiff = (hookAngleIn360 - eulersIn360 + 360) % 360;
        float rotationTargetDirection = (angleDiff < 180) ? 1 : -1; //TODO: understand this better

        //PUT ON THE BURNER FOR NOW

        if (moving)
        {
            if (moveAngle != lastMoveAngle) { pathSet = false; }
            moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * moveX, 10 * moveY));
            lastMoveAngle = moveAngle;
            hookAngle = moveAngle - Mathf.Sign(moveAngle) * 180;
            if (!movingLastFrame) { swingMomentum = Mathf.Max(swingMomentum, 0); }
            movingLastFrame = true;

            //TODO: understand this better
            float diff = (hookAngle - eulersZ + 180) % 360 - 180;
            diff = Mathf.Abs(diff < -180 ? diff + 360 : diff);

            //Debug.Log(diff);
            if (!pathSet)
            {
                if (diff == 0)
                {
                    lastMoveAngle = moveAngle;
                }
                else
                {
                    pathSet = true;
                    if (Mathf.Abs(rotateSpeed) < 5)
                    {
                        sinShift = Mathf.Min(0.3f,0.01f*diff);
                    }
                    else
                    {
                        //BROKEN DECELERATION; PERHAPS FIX LATER
                        //sinShift = Mathf.Asin(((Mathf.Sign(rotateSpeed) == rotationTargetDirection) ? 1 : -1) * Mathf.Abs(rotateSpeed));

                        //replacement; also need to fix, but less fundamentally broken
                        if (Mathf.Sign(rotateSpeed) == rotationTargetDirection)
                        {
                            sinShift = Mathf.Asin(Mathf.Abs(rotateSpeed));
                        }
                        else
                        {
                            sinShift = Mathf.Min(0.3f, 0.01f * diff);
                        }
                    }

                    swingDistance = diff;
                    sinStretch = (Mathf.PI - sinShift) / swingDistance;

                    //need to fix
                    sinStretch *= rotationTargetDirection;
                    sinShift *= rotationTargetDirection;

                    Debug.Log("activate! sinShift: " + sinShift + " sinStretch: " + sinStretch);
                }
            }
            rotateSpeed = Mathf.Sin(sinStretch * (swingDistance - diff) + sinShift);

            Debug.Log("rotateSpeed: " + rotateSpeed + " swingDistance: " + swingDistance+ " sD - diff: " + (swingDistance - diff));
        }
        else
        {
            pathSet = false;
            movingLastFrame = false;
            float slowDownAmount = 0.2f*(Mathf.Pow(0.01f,-0.8f+0.03f*Mathf.Abs(swingMomentum))+2);
            rotateSpeed = Mathf.Sign(rotateSpeed) * Mathf.Max(Mathf.Abs(rotateSpeed) - slowDownAmount, 0);//slowDownCap);
            swingMomentum -= 5*Time.deltaTime;
            //Debug.Log(swingMomentum);
        }

        swingMomentum = Mathf.Min(Mathf.Abs(rotateSpeed) / 30 - 3.5f, 100);
        anchor.transform.localRotation = Quaternion.Euler(0, 0, eulersZ + rotateSpeed* maxRotateSpeed * Time.deltaTime);
        
        //TEMP
        /*
        if (moving)
        {
            moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * moveX, 10 * moveY));
            hookAngle = moveAngle - Mathf.Sign(moveAngle) * 180;
        }
        anchor.transform.localRotation = Quaternion.Euler(0, 0, hookAngle);
        */
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
