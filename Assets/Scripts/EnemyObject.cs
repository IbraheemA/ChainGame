using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class EnemyObject : MonoBehaviour {

    // Use this for initialization
    public Enemy enemy;
    private void Awake()
    {
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.name == "Hook")
        {
            collision.gameObject.GetComponentInParent<PlayerObject>().player.processHit(enemy, collision);
        }
    }
}
