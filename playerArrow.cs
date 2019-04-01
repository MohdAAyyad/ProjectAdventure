using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerArrow : MonoBehaviour
{
    private MCamera playerACamera;
    public GameObject playerAEffect;

    private void Awake()
    {
        playerACamera = GameObject.Find("Main Camera").GetComponent<MCamera>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Enemy"))
        {
            playerACamera.cameraShake = true;
            col.GetComponent<Enemy>().enemyTakeDamage(3);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Destroy(Instantiate(playerAEffect, gameObject.transform.position, gameObject.transform.rotation),0.5f);
    }
}
