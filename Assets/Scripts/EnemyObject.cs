﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class EnemyObject : MonoBehaviour {

    public Enemy linkedScript;

    void Awake()
    {
    }

    void Update()
    {
        linkedScript.Update();
        transform.Translate(linkedScript.velocity * Time.deltaTime);
    }
}
