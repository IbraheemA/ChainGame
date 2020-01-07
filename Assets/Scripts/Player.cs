using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int moveX, moveY;
    private bool moving;
    private float xSpeed, ySpeed, moveSpeed;
    private float moveAngle, hookAngle;
    private GameObject hook, anchor;

    void Awake () {
        moveX = 0; moveY = 0; moving = false;
        xSpeed = 0; ySpeed = 0;
        moveSpeed = 7f;
        moveAngle = 0; hookAngle = 0;
        //TODO: FIX
        anchor = GameObject.Find("Anchor");
        hook = GameObject.Find("Hook");
    }

	void Update () {
        moveX = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        moveY = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        moving = moveX != 0 || moveY != 0;
        float speedMod = (moveX != 0 && moveY != 0) ? 1 / Mathf.Sqrt(2) : 1;

        moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10*moveX, 10*moveY));
        if (moving)
        {
            hookAngle = moveAngle + 180;
            anchor.transform.localRotation = Quaternion.Euler(0, 0, hookAngle);
        }

        if (Input.GetKey("space"))
        {
            hook.transform.localPosition = new Vector2(1,0);
        }
        else
        {
            hook.transform.localPosition = new Vector2(0.2f, 0);
        }

        float appliedSpeed = speedMod * moveSpeed;

        xSpeed = (moveX != 0) ? xSpeed + moveX * 0.5f : Mathf.Sign(xSpeed)*Mathf.Max(0, Mathf.Abs(xSpeed) - 0.35f);
        xSpeed = Mathf.Clamp(xSpeed, -appliedSpeed, appliedSpeed);
        ySpeed = (moveY != 0) ? ySpeed + moveY * 0.5f : Mathf.Sign(ySpeed) * Mathf.Max(0, Mathf.Abs(ySpeed) - 0.35f);
        ySpeed = Mathf.Clamp(ySpeed, -appliedSpeed, appliedSpeed);

        transform.Translate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0);
    }
}
