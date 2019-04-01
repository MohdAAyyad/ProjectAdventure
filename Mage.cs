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
        enemyHealth = enemyHealthMax = 25.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(enemyActive);
        if (enemyActive)
        {
            if (enemyHasTakenDamage)
            {
                enemyShowDamage();
            }

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
    }

    //-------------------Attack--------------

    private void mageAttack()
    {
        //Check for the player's position periodically
        if (enemyTimerToCheckPlayerPos <= 0.0f)
        {
            enemyPlayerPosition = enemyFindPlayer.transform.position;
            enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;

            if (enemyDistanceFromPlayer.x < 0.0f && enemyMovementDirection == -1)
            {
                enemyHorizontalFlip();
            }
            else if (enemyDistanceFromPlayer.x > 0.0f && enemyMovementDirection == 1)
            {
                enemyHorizontalFlip();
            }
            //If the player is far away, go back to patrolling
            else if (Mathf.Abs(enemyDistanceFromPlayer.x) > enemyRaycastLookForPlayerLength + 5.0f)
            {
                enemyCurrentState = enemyState.Patrolling;
                enemyAnimator.SetBool("enemyDetectedPlayer", false);
                enemyAnimator.SetBool("enemyAttackPlayer", false);
                enemyAnimator.SetBool("enemyReloading", false);
            }

            enemyTimerToCheckPlayerPos = 0.5f;
        }
        else
        {
            enemyTimerToCheckPlayerPos -= Time.deltaTime;
        }
    }


    //-------------------Animator--------------
    private void mageBulletShoot()
    {
        if (!mageReloading)
        {
            //Shoot an arrow and into reloading
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
