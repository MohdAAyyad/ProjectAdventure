using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerArrow : MonoBehaviour
{
    private MCamera playerACamera;
    private Sounds arrowSounds;
    private AudioSource arrowAudioSource;
    public GameObject playerAEffect;

    private void Awake()
    {
        playerACamera = GameObject.Find("Main Camera").GetComponent<MCamera>();
        arrowSounds = GameObject.Find("AudioManager").GetComponent<Sounds>();
        arrowAudioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Enemy"))
        {
            arrowAudioSource.PlayOneShot(arrowSounds.Arrow);
            playerACamera.cameraShake = true;
            col.GetComponent<Enemy>().enemyTakeDamage(3);
            Destroy(gameObject);
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

    private void OnDestroy()
    {
        Destroy(Instantiate(playerAEffect, gameObject.transform.position, gameObject.transform.rotation),0.5f);
    }
}
