using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA : MonoBehaviour
{
    private MCamera EAcamera;
    private float EAtimeToDie;
    private int EAenemyDamagedCount;
    public GameObject EAeffect;

    private void Awake()
    {
        EAcamera = GameObject.Find("Main Camera").GetComponent<MCamera>();
        EAtimeToDie = 1.5f;
        EAenemyDamagedCount = 3;
    }

    private void Update()
    {
        if(EAtimeToDie>0.0f)
        {
            EAtimeToDie -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Enemy"))
        {
            if (EAenemyDamagedCount > 0)
            {
                EAcamera.cameraShake = true;
                col.GetComponent<Enemy>().enemyTakeDamageAbility("EA", 5,null);
                Destroy(Instantiate(EAeffect, col.gameObject.transform.position, gameObject.transform.rotation), 0.5f);
                EAenemyDamagedCount--; //Only three enemies can be paralyzed at a time
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (!(col.gameObject.tag.Equals("Shield")
                  || col.gameObject.tag.Equals("A")
                  || col.gameObject.tag.Equals("X")
                  || col.gameObject.tag.Equals("B")
                  || col.gameObject.tag.Equals("W")
                  || col.gameObject.tag.Equals("RT")
                  || col.gameObject.tag.Equals("LB")
                  || col.gameObject.tag.Equals("C")
                  || col.gameObject.tag.Equals("Chest")
                  || col.gameObject.tag.Equals("CollectExp"))) 
        {
            Destroy(gameObject);
        }
    }
}
