using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class EnemyObject : MonoBehaviour {

    public Enemy enemy;

    void Awake()
    {
    }

    void Update()
    {
        enemy.Update();
    }
}
