using UnityEngine;

public class PlayerInput
{
    public bool directHookBack, lockHookRotation, lockHookPropulsion, firing;
    public Vector2 move, hookVector;

    public void GetInputKeyboard1()
    {
        move = Vector2.zero;
        move.x = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        move.y = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        directHookBack = Input.GetKey("q");
        lockHookRotation = Input.GetKey("w");
        lockHookPropulsion = Input.GetKey("e");
        firing = Input.GetKey("space");
    }

    public void GetInput()
    {
        move = Vector2.zero;
        move.x = (Input.GetKey("d") ? 1 : 0) - (Input.GetKey("a") ? 1 : 0);
        move.y = (Input.GetKey("w") ? 1 : 0) - (Input.GetKey("s") ? 1 : 0);
        hookVector.x = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
        hookVector.y = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
        directHookBack = Input.GetKey("i");
        lockHookRotation = Input.GetKey("o");
        lockHookPropulsion = Input.GetKey("p");
        firing = Input.GetKey("space");
    }
}