using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDTE : MonoBehaviour
{
    [HideInInspector]
    public float dteDirection;
    private float dteSpeed;

    void Start()
    {
        dteSpeed = 0.05f;
    }

    void Update()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + dteSpeed * dteDirection, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Enemy"))
        {
            col.GetComponent<Enemy>().enemyTakeDamage(20);
        }
    }

    private void dteDestroySelf()
    {
        Destroy(gameObject);
    }
}
