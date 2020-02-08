using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class GruntObject : MonoBehaviour {

    public Enemy linkedScript;

    void Update()
    {
        linkedScript.Update();
    }

    public void Attack()
    {
        StartCoroutine("AttackCoroutine");
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        linkedScript.parseCollisionData(Physics2D.BoxCastAll(transform.position, new Vector2(20,20), 0, Vector2.left, 0));
    }
}
