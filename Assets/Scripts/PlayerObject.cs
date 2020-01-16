using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class PlayerObject : MonoBehaviour {


    public Player linkedScript;



    void Awake () {

    }

    void Start()
    {
        linkedScript.velocity = new Vector2(0, 0);
    }

    void Update () {
        linkedScript.Update();
    }
}
