using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher : Enemy
{

    public GameObject archerArrow;
    private GameObject archerArrowInsta;
    private Vector2 archerForce;
    private Vector2 archerPos;
    private bool archerReloading;


    // Start is called before the first frame update
    void Start()
    {
        enemyStart();
        enemyActive = false;
        archerForce = new Vector2(1200.0f,0.0f);
        enemyAttackTimer = 1.0f;
        archerReloading = false;
        enemyHealth = enemyHealthMax = 50.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Debug.Log(enemyActive);
        if(enemyActive)
        {
            base.Update();
            if (enemyHasTakenDamage)
            {
                enemyShowDamage();
            }

            if (!enemyCheckForParalysis())
            {
                //Take a breather between each shot
                if (archerReloading)
                {
                    if (enemyAttackTimer > 0.0f)
                    {
                        enemyAttackTimer -= Time.deltaTime;
                    }
                    else
                    {
                        archerReloading = false;
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
                        archerAttack();
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

    private void archerAttack()
    {
        //Check for the player's position periodically
        enemyCheckPlayerPosition();
    }


    //-------------------Animator--------------
    private void archerArrowShoot()
    {
        if (!archerReloading)
        {
            //Shoot an arrow and into reloading
            archerPos = new Vector2(gameObject.transform.position.x + 0.5f * enemyMovementDirection, gameObject.transform.position.y);
            enemyAudioSource.PlayOneShot(enemySounds.skeletonBow);
            archerArrowInsta = Instantiate(archerArrow, archerPos, gameObject.transform.rotation);
            archerArrowInsta.GetComponent<Rigidbody2D>().AddForce(archerForce * enemyMovementDirection);
        }
    }

    private void archerArrowShootEnd()
    {
        if (!archerReloading)
        {
            archerReloading = true;
            enemyAnimator.SetBool("enemyReloading", true);
        }
    }
}
