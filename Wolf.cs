using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Wolf : Enemy
{
    //Components
    private TrailRenderer wolfTrail;

    //Commence attack
    private bool wolfCommenceAttack;
    private Vector2 wolfCommenceSignalPos;
    public GameObject wolfCommenceSignal;
    
    //Attack
    private Vector2 wolfAttackCenter;
    private Vector2 wolfAttackSize;
    private float wolfAttackDirection;
    public LayerMask wolfMask;
    private Collider2D player;

    //Reloading
    private bool wolfReloading;

    void Start()
    {
        enemyStart();
        wolfCommenceAttack = false;
        wolfAttackSize = new Vector2(1.5f,1.0f);
        enemyAttackTimer = 0.7f;
        enemyDamageInflicted = 10.0f;
        wolfTrail = gameObject.GetComponent<TrailRenderer>();
        enemyHealth = enemyHealthMax = 80.0f;
        wolfAttackDirection = -1.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(enemyActive)
        {
            base.Update();
            if (enemyHasTakenDamage)
            {
                enemyShowDamage();
            }
            if (!enemyCheckForParalysis())
            {
                if (wolfReloading)
                {
                    if (enemyAttackTimer > 0.0f)
                    {
                        enemyAttackTimer -= Time.deltaTime;
                    }
                    else
                    {
                        wolfReloading = false;
                        enemyAnimator.SetBool("enemyReloading", false);
                        enemyAttackTimer = 0.7f;
                    }
                }

                switch (enemyCurrentState)
                {
                    case enemyState.Patrolling:
                        enemyPatrol();
                        break;
                    case enemyState.Attacking:
                        wolfAttack();
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

    private void wolfAttack()
    {
        enemyCheckPlayerPosition();
    }

    protected override void  enemyCheckPlayerPosition()
    {
        //Removed the flipping when the player is on the opposite side of the wolf
        if (enemyTimerToCheckPlayerPos <= 0.0f)
        {
            enemyPlayerPosition = enemyFindPlayer.transform.position;
            enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;

            //If the player is far away, go back to patrolling
            if (Mathf.Abs(enemyDistanceFromPlayer.x) > enemyRaycastLookForPlayerLength + 10.0f)
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


    //-------------------Animator methods--------------

        //Instantiate the attack signal above the wolf
    private void wolfAttackSignal()
    {
        enemyAudioSource.PlayOneShot(enemySounds.wolfGrowl);
        wolfCommenceSignalPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f);
        Destroy(Instantiate(wolfCommenceSignal, wolfCommenceSignalPos, gameObject.transform.rotation), 0.5f);  
    }

    //Move to the opposite side of where the player is facing
    private void wolfAttackMove()
    {
        if (enemyDistanceFromPlayer.x < 0.0f)
        {
            wolfAttackDirection = -1.0f;
        }
        else
        {
            wolfAttackDirection = 1.0f;
        }
        enemySpriteRenderer.enabled = false;
        gameObject.transform.position = new Vector3(enemyPlayerPosition.x - 1.0f * wolfAttackDirection, enemyPlayerPosition.y, enemyPlayerPosition.z);

    }

    //Flip and start the attack animation
    private void wolfAttackNow()
    {

        enemySpriteRenderer.enabled = true;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1.0f, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        enemyAnimator.SetBool("wolfCommenceAttack", true);

        //Flip the hp bar's fill origin
        if (enemyHPBar.fillOrigin == (int)Image.OriginHorizontal.Right)
        {
            enemyHPBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            enemyHPBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    //Create the overlap box
    private void wolfAttackBox()
    {
        wolfAttackCenter = new Vector2(gameObject.transform.position.x - 1.0f * gameObject.transform.localScale.x, gameObject.transform.position.y);
        if(player = Physics2D.OverlapBox(wolfAttackCenter,wolfAttackSize,0.0f,wolfMask))
        {
            player.GetComponent<Player>().playerTakeDamage(enemyDamageInflicted);
        }
    }

    //Go into reloading state
    private void wolfAttackEnd()
    {
        enemyAnimator.SetBool("wolfCommenceAttack", false);
        enemyAnimator.SetBool("enemyReloading", true);
        wolfReloading = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wolfAttackCenter, wolfAttackSize);
    }
}
