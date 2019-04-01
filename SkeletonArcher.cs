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
        enemyHealth = enemyHealthMax = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(enemyActive);
        if(enemyActive)
        {
            if (enemyHasTakenDamage)
            {
                enemyShowDamage();
            }

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
    }

    //-------------------Attack--------------

    private void archerAttack()
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
            else if(Mathf.Abs(enemyDistanceFromPlayer.x) > enemyRaycastLookForPlayerLength + 5.0f)
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
    private void archerArrowShoot()
    {
        if (!archerReloading)
        {
            //Shoot an arrow and into reloading
            archerPos = new Vector2(gameObject.transform.position.x + 0.5f * enemyMovementDirection, gameObject.transform.position.y);
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
