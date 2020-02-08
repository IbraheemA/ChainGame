using UnityEngine;

public class PlayerInput
{
    public bool directHookBack, lockHookRotation;
    public Vector2 move;

    public void GetInput()
    {
        move = Vector2.zero;
        move.x = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        move.y = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        directHookBack = Input.GetKey("q");
        lockHookRotation = Input.GetKey("w");
    }
}