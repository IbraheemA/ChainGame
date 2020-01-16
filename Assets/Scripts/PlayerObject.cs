using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class PlayerObject : MonoBehaviour {

    public Player linkedScript;

    void Update()
    {
        linkedScript.Update();
    }
}
