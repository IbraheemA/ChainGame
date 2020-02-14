using System.Collections.Generic;
using UnityEngine;
using Entities;

public class Director : MonoBehaviour {

    public static Player player;
    public static bool playerIsAlive = true;
    public static Director self;
    private float spawnTimerMax = 3f;
    private float spawnTimer = 0;
    public GameObject playerHealthbar;

	void Awake () {
        self = this;
        playerHealthbar = GameObject.Find("Healthbar");
	}

    void Start()
    {
        player = SpawnPlayer(0, 0);
        SpawnEnemy(30, 0, "Grunt");
    }

    void Update()
    {
        playerHealthbar.transform.localScale = new Vector3(Mathf.Max(0, player.Stats["health"] / player.Stats["maxHealth"]), playerHealthbar.transform.localScale.y, playerHealthbar.transform.localScale.z);
        if(spawnTimer <= 0)
        {
            //MAKE THIS PICK VALID SPAWN POINTS (EDGES OF SCREEN AND NOT ON PLAYER)
            List<string> names = new List<string> { "Grunt", "Chaser" };
            int index = Mathf.Min((int)Mathf.Floor(Random.Range(0, names.Count)), names.Count);
            SpawnEnemy(Random.Range(-30, 30), Random.Range(-20, 20), names[index]);
            spawnTimer = spawnTimerMax;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
        
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
            case "Chaser":
                toSpawn = new Chaser(new Vector2(X, Y));
                break;
            default:
                toSpawn = new Grunt(new Vector2(X, Y));
                break;
        }
        return toSpawn;
    }


}
