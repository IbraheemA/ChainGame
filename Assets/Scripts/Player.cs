using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int moveX, moveY;
    private bool moving, loaded, movingLastFrame;
    private float xSpeed, ySpeed, moveSpeed, shootSpeed, rotateSpeed, maxRotateSpeed;
    private float swingMomentum;
    private float moveAngle, hookAngle;
    private GameObject hook, anchor;

    void Awake () {
        moveX = 0; moveY = 0; moving = false; movingLastFrame = false; loaded = true;
        xSpeed = 0; ySpeed = 0;
        moveSpeed = 10f;
        shootSpeed = 0; rotateSpeed = 0; maxRotateSpeed = 300;
        moveAngle = 0; hookAngle = 0;
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

        moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10*moveX, 10*moveY));

        hookAngle = moveAngle - Mathf.Sign(moveAngle) * 180;
        float hookAngleIn360 = (hookAngle + 360) % 360;
        float eulersZ = anchor.transform.localRotation.eulerAngles.z;
        float eulersIn360 = (eulersZ + 360) % 360;
        float angleDiff = (hookAngleIn360 - eulersIn360 + 360) % 360;
        float rotationAccelerationDirection = (angleDiff < 180) ? 1 : -1; //TODO: understand this better

        if (moving)
        {
            if (!movingLastFrame) {swingMomentum = Mathf.Max(swingMomentum,0);}
            movingLastFrame = true;
            //swingMomentum = Mathf.Min(30, swingMomentum + Mathf.Abs(rotateSpeed)/1000);
            swingMomentum = Mathf.Abs(rotateSpeed)/30-3.5f;

            rotateSpeed += Mathf.Min(Mathf.Abs(rotateSpeed) / 10 + 1, 3.5f) * rotationAccelerationDirection;

            float diff = (hookAngle - eulersZ + 180) % 360 - 180;
            diff = Mathf.Abs(diff < -180 ? diff + 360 : diff);
            //TODO: understand this better

            if (diff < 90 && swingMomentum < 12) {
                if (Mathf.Sign(rotateSpeed) == Mathf.Sign(rotationAccelerationDirection))
                {
                    //Debug.Log("activating 1!" + swingMomentum);
                    rotateSpeed -= ((angleDiff < 15) ? rotateSpeed / 8 : ((angleDiff < 40) ? (rotateSpeed) / 30 : (rotateSpeed) / 40)) + Mathf.Sign(rotateSpeed) * ((swingMomentum > 5) ? 2 : 0);
                }
                else
                {
                    //Debug.Log("activating2!");
                    rotateSpeed -= ((angleDiff < 15) ? rotateSpeed / 3 : ((angleDiff < 40) ? (rotateSpeed) / 15 : (rotateSpeed) / 20)) + Mathf.Sign(rotateSpeed) * ((swingMomentum > 5) ? 4 : 0);
                }
            }
            Debug.Log(diff);
        }
        else
        {
            if (movingLastFrame)
            {
                movingLastFrame = false;
            }
            float slowDownAmount = 0.2f*(Mathf.Pow(0.01f,-0.6f+0.03f*Mathf.Abs(swingMomentum))+2);
            rotateSpeed = Mathf.Sign(rotateSpeed) * Mathf.Max(Mathf.Abs(rotateSpeed) - slowDownAmount, 0);//slowDownCap);
            swingMomentum -= 5*Time.deltaTime;
            //Debug.Log(swingMomentum);
        }
        anchor.transform.localRotation = Quaternion.Euler(0, 0, eulersZ + rotateSpeed * Time.deltaTime);

        if (Input.GetKey("space"))
        {
            if (loaded)
            {
                shootSpeed = 0.2f;
                //hook.transform.localPosition.x = new Vector2(1,0);
            }
            //hook.transform.localPosition = new Vector2(1,0);
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
