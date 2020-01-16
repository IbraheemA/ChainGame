using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class Director : MonoBehaviour {

    public static Player player;
    public static Director self;

	void Awake () {
        self = this;
	}

    void Start()
    {
        player = SpawnPlayer(0, 0);
        SpawnEnemy(30, 0, "Grunt");
    }

    Player SpawnPlayer(float X, float Y)
    {
        return new Player(new Vector2(X, Y));
    }
    Enemy SpawnEnemy(float X, float Y, string typeToSpawn)
    {
        Enemy toSpawn;
        switch (typeToSpawn) {
            case "Grunt":
                toSpawn = new Grunt(new Vector2(X, Y));
                break;
            default:
                toSpawn = new Grunt(new Vector2(X, Y));
                break;
        }
        return toSpawn;
    }
}
