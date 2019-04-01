using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{

    private bool skeletonReloading;
    private Collider2D player;
    private Vector2 skeletonAttackCenter;
    private Vector2 skeletonAttackSize;
    private bool skeletonIsAttacking;
    public LayerMask skeletonAttackMask;

    private void Start()
    {
        enemyStart();
        enemyDamageInflicted = 7.0f;
        skeletonIsAttacking = false;
        enemyHealth = enemyHealthMax = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyHasTakenDamage)
        {
            enemyShowDamage();
        }

        //Take a breather between each shot
        if (skeletonReloading)
        {
            //If the player moes far, break the reloading and charge if you're not already attacking
            enemyPlayerPosition = enemyFindPlayer.transform.position;
            enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;

            if (Mathf.Abs(enemyDistanceFromPlayer.x) > 2.5f && !skeletonIsAttacking)
            {
                skeletonIsAttacking = false;
                skeletonReloading = false;
                enemyAnimator.SetBool("enemyReloading", false);
                enemyAttackTimer = 1.0f;
            }
            //Wait a bit between each attack
            else if (enemyAttackTimer > 0.0f)
            {
                enemyAttackTimer -= Time.deltaTime;
            }
            else
            {
                skeletonReloading = false;
                enemyAnimator.SetBool("enemyReloading", false);
                enemyAttackTimer = 1.0f;
            }
        }

        //Check which state we're in
        switch (enemyCurrentState)
        {
            case enemyState.Patrolling:
                enemyPatrol();
                break;
            case enemyState.Attacking:
                skeletonAttack();
                break;
        }

    }

    //-------------------Attack--------------

    private void skeletonAttack()
    {
        //Check for the player's position periodically
        if (enemyTimerToCheckPlayerPos <= 0.0f)
        {
            enemyPlayerPosition = enemyFindPlayer.transform.position;
            enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;
            enemyDirectionToPlayer = -enemyDistanceFromPlayer.normalized;

            //Is the player to the left or to the right of the enemy?
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


            //See where the player is and determine if you should move towards him or start attacking immediately
            enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;
            if (Mathf.Abs(enemyDistanceFromPlayer.x) <= 2.5f && !skeletonIsAttacking)
            {
                enemyRigidBody.velocity = new Vector2(0.0f, 0.0f);
                skeletonIsAttacking = true;
                enemyAnimator.SetBool("enemyAttackPlayer", true);
       
            }
            else if(!skeletonIsAttacking)
            {
                enemyRigidBody.velocity = new Vector2(enemySpeed * 2.5f, 0.0f) * enemyDirectionToPlayer;
                enemyAnimator.SetBool("enemyAttackPlayer", false);
            }

                enemyTimerToCheckPlayerPos = 0.5f;
        }
        else
        {
            enemyTimerToCheckPlayerPos -= Time.deltaTime;
        }
    }

    //-------------------Animator--------------

    private void skeletonSwordAttack()
    {
        //Create an overlap box to damage the player
        skeletonAttackCenter = new Vector2(gameObject.transform.position.x + 1.0f * enemyMovementDirection, gameObject.transform.position.y);
        skeletonAttackSize = new Vector2(2.5f, 1.0f);
        if (player = Physics2D.OverlapBox(skeletonAttackCenter, skeletonAttackSize, 0.0f, skeletonAttackMask))
        {
            player.GetComponent<Player>().playerTakeDamage(enemyDamageInflicted);
        }

    }

    //Check if you need to move again if the player has moved away
    private void skeletonSwordAttackEnd()
    {
        enemyPlayerPosition = enemyFindPlayer.transform.position;
        enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;
        if (Mathf.Abs(enemyDistanceFromPlayer.x) > 2.5f)
        {
            enemyAnimator.SetBool("enemyAttackPlayer", false);
            skeletonIsAttacking = false;
        }
        else
        {
            skeletonReloading = true;
            enemyAnimator.SetBool("enemyReloading", true);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(skeletonAttackCenter, skeletonAttackSize);
    }

}
