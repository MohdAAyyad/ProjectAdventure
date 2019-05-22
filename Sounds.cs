using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sounds : MonoBehaviour
{
    public static Sounds Instance;

     // fucking pleb
    //Audio source
    // private AudioSource PlayerAudioSource;

    public AudioSource music1;
    public AudioSource music2;
    private bool changeMusic;
    private bool music2HasStarted;

    //Audio Clips

    //++UI
    public AudioClip buttonClick;

    //++Player
    public AudioClip swordAttack;
    public AudioClip swordNoImpact;
    public AudioClip swordDownThrust;
    public AudioClip Bow;
    public AudioClip Arrow;
    public AudioClip EA;
    public AudioClip DTE;
    public AudioClip Shield;
    public AudioClip gettingHit;
    public AudioClip heal;
    public AudioClip step1;
    public AudioClip checkpoint;
    public AudioClip BAexplosion;
    public AudioClip BAbowSound;
    public AudioClip PAimpactSound;
    public AudioClip FAactivated;

    //++Enemies
    public AudioClip healerEffect;
    public AudioClip skeletonSword;
    public AudioClip skeletonBow;
    public AudioClip mageCharge;
    public AudioClip mageAttack;
    public AudioClip wolfGrowl;

    private void Start()
    {
        Cursor.visible = false;
        changeMusic = false;
        music2HasStarted = false;
    }

    private void Update()
    {
        if(changeMusic)
        {
            if (music1.volume > 0.0f)
            {
                music1.volume -= 0.01f;
            }
            else
            {
                music2.volume += 0.01f;
                if (!music2HasStarted)
                {
                    music2.Play();
                    music2HasStarted = true;
                }
            }
            if(music2.volume>=1.0f)
            {
                changeMusic = false;
            }
        }
    }

    public void audioManagerChangeMusic()
    {
        changeMusic = true;
    }
}
