using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Enemy
{

    public GameObject mageBullet;
    private GameObject mageBulletInsta;
    private Vector2 mageForce;
    private Vector2 magePos;
    private bool mageReloading;

    // Start is called before the first frame update
    void Start()
    {
        enemyStart();
        enemyActive = false;
        mageForce = new Vector2(1200.0f, 0.0f);
        enemyAttackTimer = 1.0f;
        mageReloading = false;
        enemyHealth = enemyHealthMax = 60.0f;
    }

    protected override void Update()
    {
        base.Update();
        //Debug.Log(enemyActive);
        if (enemyActive)
        {
            if (enemyHasTakenDamage)
            {
                enemyShowDamage();
            }

            if (!enemyCheckForParalysis())
            {
                //Take a breather between each shot
                if (mageReloading)
                {
                    if (enemyAttackTimer > 0.0f)
                    {
                        enemyAttackTimer -= Time.deltaTime;
                    }
                    else
                    {
                        mageReloading = false;
                        enemyAnimator.SetBool("enemyReloading", false);
                        enemyAttackTimer = 1.0f;
                    }
                }

                switch (enemyCurrentState)
                {
                    case enemyState.Patrolling:
                        enemyPatrol();
                        break;
                    case enemyState.Attacking:
                        mageAttack();
                        break;
                }
            }
            else
            {
                enemyAnimator.speed = 0.0f;
            }


        }
    }

    //-------------------Attack--------------

    private void mageAttack()
    {
        enemyCheckPlayerPosition();
    }


    //-------------------Animator--------------

    private void mageBulletCharge()
    {
        enemyAudioSource.PlayOneShot(enemySounds.mageCharge);
    }
  
    private void mageBulletShoot()
    {
        if (!mageReloading)
        {
            if(enemyAudioSource.isPlaying)
            {
                enemyAudioSource.Stop();
            }
            enemyAudioSource.PlayOneShot(enemySounds.mageAttack);
            //Shoot a mage bullet and go into reloading
            magePos = new Vector2(gameObject.transform.position.x + 0.5f * enemyMovementDirection, gameObject.transform.position.y);
            mageBulletInsta = Instantiate(mageBullet, magePos, gameObject.transform.rotation);
            mageBulletInsta.GetComponent<Rigidbody2D>().AddForce(mageForce * enemyMovementDirection);
        }
    }

    private void mageBulletShootEnd()
    {
        if (!mageReloading)
        {
            mageReloading = true;
            enemyAnimator.SetBool("enemyReloading", true);
        }
    }
}
