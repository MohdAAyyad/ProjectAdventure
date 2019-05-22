using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHealer : Enemy
{
    //Attack
    private Vector2 healerCenter;
    private float healerRadius;
    private Collider2D[] healerTargets;
    private float healAmount;
    private bool healerHasHealed;
    public LayerMask healerMask;

    //Reloading
    private bool healerReloading;

    //Healing effect
    public GameObject healEffect;

    // Start is called before the first frame update
    void Start()
    {
        enemyStart();
        //Attack
        healerRadius = 5.0f;
        healerReloading = false;
        enemyAttackTimer = 2.0f;
        healAmount = 10;
        healerHasHealed = false;
        enemyHealth = enemyHealthMax = 80.0f;

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (enemyActive)
        {
            base.Update();
            //Show damage
            if (enemyHasTakenDamage)
            {
                enemyShowDamage();
            }
            if (!enemyCheckForParalysis())
            {
                //Check for reloading
                if (healerReloading)
                {
                    if (enemyAttackTimer > 0.0f)
                    {
                        enemyAttackTimer -= Time.deltaTime;
                    }
                    else
                    {
                        healerReloading = false;
                        healerHasHealed = false;
                        enemyAnimator.SetBool("enemyReloading", false);
                        enemyAttackTimer = 2.0f;
                    }
                }
                //State machine
                switch (enemyCurrentState)
                {
                    case enemyState.Patrolling:
                        enemyPatrol();
                        break;
                    case enemyState.Attacking:
                        healerAttack();
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

    private void healerAttack()
    {
        enemyCheckPlayerPosition();
    }

    //-------------------Animator methods--------------

    private void healerShoot()
    {
        //Heal all enemies within the heal radius
        healerCenter = gameObject.transform.position;
        if (!healerHasHealed)
        {
            enemyAudioSource.PlayOneShot(enemySounds.healerEffect);
            healerTargets = Physics2D.OverlapCircleAll(healerCenter, healerRadius,healerMask);
            for (int i = 0; i < healerTargets.Length; i++)
            {
                healerTargets[i].GetComponent<Enemy>().enemyHeal(healAmount);
                //Create the heal effect
                Destroy(Instantiate(healEffect, healerTargets[i].transform.position, gameObject.transform.rotation),1.0f);

            }
            //Only heal once
            healerHasHealed = true;
        }

    }

    private void healerReloadNow()
    {
        //Go to reloading state
        healerReloading = true;
        enemyAnimator.SetBool("enemyReloading", true);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(healerCenter, healerRadius);
    }

}
