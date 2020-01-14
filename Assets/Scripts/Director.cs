using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class Director : MonoBehaviour {

	void Awake () {

	}

    void Start()
    {
        SpawnPlayer(0, 0);
        SpawnEnemy(0, 0, "null");
    }

    void SpawnPlayer(float X, float Y)
    {
        new Player(new Vector2(X,Y));
    }
    void SpawnEnemy(float X, float Y, string type)
    {
        Enemy toSpawn = new Enemy(new Vector2(X,Y), 20);
    }
}
