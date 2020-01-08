using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int moveX, moveY;
    private bool moving, loaded;
    private float xSpeed, ySpeed, moveSpeed, shootSpeed;
    private float moveAngle, hookAngle;
    private GameObject hook, anchor;

    void Awake () {
        moveX = 0; moveY = 0; moving = false; loaded = true;
        xSpeed = 0; ySpeed = 0;
        moveSpeed = 30f;
        shootSpeed = 0;
        moveAngle = 0; hookAngle = 0;
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
        if (moving)
        {
            hookAngle = moveAngle - Mathf.Sign(moveAngle)*180;
            float eulersZ = anchor.transform.localRotation.eulerAngles.z;
            float hookAngleIn360 = (hookAngle + 360) % 360;
            float eulersIn360 = (eulersZ + 360) % 360;
            float rotateAmount = ((hookAngleIn360 - eulersIn360 + 360) % 360 < 180) ? 1 : -1; //TODO: understand this better
            //float rotateAmount = ((eulersIn360 < hookAngleIn360) ? 1 : -1) * ((Mathf.Abs(eulersIn360 - hookAngleIn360) < 180) ? 1 : -1);

            float rotateTarget;
            rotateTarget = eulersZ + 3 * rotateAmount;
            
            anchor.transform.localRotation = Quaternion.Euler(0,0,rotateTarget);
        }

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
