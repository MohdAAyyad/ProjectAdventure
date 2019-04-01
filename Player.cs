using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Components
    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    private ParticleSystem playerParticleSystem;
    private SpriteRenderer playerSpriteRenderer;
    private MCamera playerCamera;

    //UI
    public Image playerHPBar;

    //Health
    private float playerHealth;
    private float playerMaxHealth;
    private Vector2 playerStartingPosition;

    //states
    private enum playerStates
    {
        Idle,
        Dash,
        Attacking,
        BowAttack,
        Jumping,
        DownThrust,
        Fall,
        Wall
    }
    private playerStates playerCurrentState;

    //Idle
    private float playerHorizontal;
    private float playerVertical;
    private Vector2 playerSpeed;
    private bool playerFacingRight;
    private int playerDirection;

    //Dash
    private float playerDashTimer;
    private float playerDashScalar;
    private float playerDashJumpModifier;
    private ParticleSystem.ShapeModule playerPaticleShape;
    public Collider2D playerCollider;
    public Collider2D playerDashCollider;

    //Jump
    private bool playerIsGrounded;
    private float playerJumpTimer;
    private float playerJumpTimerSpeedMode;
    private float playerJumpTimerStart;
    public Collider2D playerFeet;

    //Wall
    private Vector2 playerWallJumpDirection;
    private Vector2 playerPreviousWallJumpDirection;
    private bool playerHasWallJumped;

    //Downwards thrust
    private bool playerHasDownThrust;

    //Attack
    private int playerAttackCounter;
    private Vector2 playerAttackCenter;
    private Vector2 playerAttackSize;
    private bool playerContinueCombo;
    private Collider2D[] enemies;
    private int playerAirAttackCounter;
    public LayerMask playerDamage;
    private bool playerHitStop;
    private int playerHitStopCounter;

    //Bow attack
    public GameObject playerArrow;
    private Vector2 playerArrowPos;
    private GameObject playerInstaArrow;
    private Vector2 playerArrowForce;
    private int playerAirBowCounter;

    //Speed system
    public GameObject playerSpeedEmitter;
    private ParticleSystem playerSpeedEmitterPS;
    private int playerSpeedHits;
    private int playerSpeedHitsNeeded;
    private float playerSpeedModifier;
    private float playerSpeedIncreaseTimer;
    private float playerSpeedIncreaseTimerStartValue;
    private bool playerInSpeedMode;

    //Effects
    //++Dust
    public GameObject playerDustEmmitter;
    private GameObject playerInstaDust;
    private Vector2 playerDustPos;
    private float playerDustTimer;
    //++Jump
    public GameObject playerJumpEffect;
    private Vector2 playerJumpEffectPos;
    //++Sword
    public GameObject playerSwordEffect;
    public GameObject playerSwordEffectSpeedMode;
    private GameObject playerSwordEffectToInsta;
    private GameObject playerSwordEffectInsta;
    private Vector2 playerSwordEffectPos;
    private Vector3 playerSwordEffectEulerRot;
    private Vector3 playerSwordEffectScale;



    void Start()
    {
        //Components
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
        playerAnimator = gameObject.GetComponent<Animator>();
        playerParticleSystem = gameObject.GetComponent<ParticleSystem>();
        playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<MCamera>();
        playerPaticleShape = playerParticleSystem.shape;
        playerParticleSystem.Stop();

        //Health
        playerHealth = playerMaxHealth = 100;
        playerStartingPosition = gameObject.transform.position;

        //States
        playerCurrentState = playerStates.Idle;

        //Idle
        playerFacingRight = true;
        playerDirection = 1;
        playerSpeed = new Vector2(4.0f, 0.0f);

        //Dash
        playerDashTimer = 0.3f;
        playerDashScalar = 2.0f;
        playerDashJumpModifier = 1.0f;

        //Jump
        playerIsGrounded = true;
        playerJumpTimerStart = 0.4f;
        playerJumpTimerSpeedMode = 0.2f;
        playerJumpTimer = playerJumpTimerStart;

        //DownThrust
        playerHasDownThrust = false;

        //Wall
        playerWallJumpDirection = new Vector2(-1.0f, 0.0f);
        playerHasWallJumped = false;        

        //Attack
        playerAttackCounter = 0;
        playerAirAttackCounter = 0;
        playerAttackSize = new Vector2(0.5f, 1.0f);
        playerContinueCombo = false;
        playerHitStop = false;
        playerHitStopCounter = 2;

        //Bow
        playerAirBowCounter = 0;

        //Speed System
        playerSpeedHits = 0;
        playerSpeedHitsNeeded = 5;
        playerSpeedModifier = 1.0f;
        playerSpeedIncreaseTimerStartValue = 10.0f;
        playerSpeedIncreaseTimer = playerSpeedIncreaseTimerStartValue;
        playerInSpeedMode = false;
        playerSpeedEmitterPS = playerSpeedEmitter.GetComponent<ParticleSystem>();

        //Effects
        playerDustTimer = 0.8f;
        playerSwordEffectToInsta = playerSwordEffect;

    }

    // Update is called once per frame
    void Update()
    {
        //Pause the game for a split second for the hitstop effect
        if (playerHitStop)
        {
            
            if (playerHitStopCounter > 0)
            {
                playerHitStopCounter -= 1;
            }
            else
            {
                Time.timeScale = 1.0f;
                playerHitStop = false;
                playerHitStopCounter = 2;
            }
        }

        //Move into idle when animation Idle has started playing after a down thrust
        if (playerHasDownThrust)
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle"))
            {
                playerCurrentState = playerStates.Idle;
                playerHasDownThrust = false;
            }
        }

        //Speed System
        if(playerSpeedHits>=playerSpeedHitsNeeded)
        {
            if(playerSpeedModifier <= 1.0f)
            {
                playerSpeedModifier = 2.0f;
                playerAnimator.speed = playerAnimator.speed * 1.5f;
                playerInSpeedMode = true;
                playerSpeedEmitter.SetActive(true);
                playerSpeedEmitterPS.Play();
                playerSpriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                playerSwordEffectToInsta = playerSwordEffectSpeedMode;

            }

            if(playerSpeedIncreaseTimer>0.0f)
            {
                playerSpeedIncreaseTimer -= Time.deltaTime;
            }
            else
            {
                playerSpeedIncreaseTimer = playerSpeedIncreaseTimerStartValue;
                playerSpeedHits = 0;
                playerSpeedModifier = 1.0f;
                playerAnimator.speed = 1.0f;
                playerInSpeedMode = false;
                playerSpeedEmitter.SetActive(false);
                playerSpeedEmitterPS.Stop();
                playerSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                playerSwordEffectToInsta = playerSwordEffect;
            }

        }
        

        //State switching
        switch (playerCurrentState)
        {
            case playerStates.Idle:
                playerIdle();
                break;
            case playerStates.Dash:
                playerDash();
                break;
            case playerStates.Jumping:
                playerJump();
                break;
            case playerStates.DownThrust:
                playerDownThrust();
                break;
            case playerStates.Fall:
                playerFall();
                break;
            case playerStates.Wall:
                playerWall();
                break;
            case playerStates.Attacking:
                playerAttack();
                break;
            case playerStates.BowAttack:
                playerBowAttack();
                break;

        }
    }

    //-------------------Idle--------------

    //In idle the character moves and waits for input
    private void playerIdle()
    {
        //Get axis, apply it to the velocity, and update the animation
        playerHorizontal = Input.GetAxisRaw("Horizontal");
        playerRigidBody.velocity = playerSpeed * playerHorizontal * playerSpeedModifier;
        playerAnimator.SetFloat("playerSpeed", Mathf.Abs(playerSpeed.x * playerHorizontal));

        //Flip
        playerFlip();

        if(Mathf.Abs(playerSpeed.x * playerHorizontal) > 0.0f)
        {
            if(playerDustTimer>0.0f)
            {
                playerDustTimer -= Time.deltaTime;
            }
            else
            {
                playerDustPos = new Vector2(gameObject.transform.position.x - 0.2f * playerDirection, gameObject.transform.position.y - 0.8f);
                playerInstaDust = Instantiate(playerDustEmmitter, playerDustPos, gameObject.transform.rotation);
                playerInstaDust.transform.localScale = new Vector3(playerInstaDust.transform.localScale.x * playerDirection * -1.0f, playerInstaDust.transform.localScale.y, playerInstaDust.transform.localScale.z);
                Destroy(playerInstaDust, 0.5f);
                playerDustTimer = 0.8f;
            }
        }

        //Check for attack
        if(Input.GetButtonDown("CTRL - X"))
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerCurrentState = playerStates.Attacking;
            playerAttackCounter++;//Player attack counter is passed to the animator to play the correct animation
        }

        //Check for jump
        if(Input.GetButtonDown("CTRL - A") && playerIsGrounded)
        {
            //Effect
            playerJumpEffectPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.8f);
            Destroy(Instantiate(playerJumpEffect, playerJumpEffectPos, gameObject.transform.rotation),0.5f);
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.Jumping;
            playerAnimator.SetBool("playerGrounded", false);

        }
        //Check for bow attack
        if(Input.GetButtonDown("CTRL - Y"))
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerCurrentState = playerStates.BowAttack;
        }
        if(Input.GetButtonDown("CTRL - B"))
        {
            playerParticleSystem.Play();
            //Change active collider
            playerCollider.enabled = false;
            playerDashCollider.enabled = true;
            playerAnimator.SetBool("playerDash", true);
            playerCurrentState = playerStates.Dash;
        }
    }

    //-------------------Dash--------------

    private void playerDash()
    {
        if(playerDashTimer>0.0f)
        {
            playerRigidBody.velocity = playerSpeed * playerDirection * playerDashScalar * playerSpeedModifier;
            playerDashTimer -= Time.deltaTime;

            if(Input.GetButtonDown("CTRL - A"))
            {
                //Jump effect
                playerJumpEffectPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.8f);
                Destroy(Instantiate(playerJumpEffect, playerJumpEffectPos, gameObject.transform.rotation), 0.5f);

                //If the player tries to jump during the dash, give him more momentum
                playerDashJumpModifier = 1.4f;
                playerParticleSystem.Stop();
                //Do not stop the particle system of the speed emitter
                if (playerInSpeedMode)
                {
                    playerSpeedEmitterPS.Play();
                }
                playerDashTimer = 0.3f;
                playerAnimator.SetBool("playerDash", false);
                playerAnimator.SetBool("playerGrounded", false);
                playerCurrentState = playerStates.Jumping;
                //Change active collider
                playerCollider.enabled = true;
                playerDashCollider.enabled = false;
            }
        }
        else
        {
            playerParticleSystem.Stop();
            //Do not stop the particle system of the speed emitter
            if(playerInSpeedMode)
            {
                playerSpeedEmitterPS.Play();
            }
            playerDashTimer = 0.3f;
            playerAnimator.SetBool("playerDash", false);
            playerCurrentState = playerStates.Idle;
            //Change active collider
            playerCollider.enabled = true;
            playerDashCollider.enabled = false;
        }
    }

    //-------------------Jump--------------

    private void playerJump()
    {
        
        if (playerJumpTimer>0.0f)
        {
            //Allow the player to control the direction of the character during the jump
            playerHorizontal = Input.GetAxisRaw("Horizontal");
            playerVertical = Input.GetAxisRaw("Vertical");
            //Check if the player has wall jumped
            if (!playerHasWallJumped)
            {
                playerRigidBody.velocity = new Vector2(playerSpeed.x * playerHorizontal, 8.0f * playerDashJumpModifier);
                playerFlip();
            }
            else
            {
                //If the player is jumping off a wall, the inital direction of the jump is based on the normal going out of the wall
                playerRigidBody.velocity = new Vector2(playerSpeed.x  * playerWallJumpDirection.x, 5.0f);
                
            }
            playerJumpTimer -= Time.deltaTime;
        }
        else
        {
            //Once the jump timer runs out, go into falling state
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.Fall;
            playerDashJumpModifier = 1.0f;
            playerAnimator.SetTrigger("playerFall");
        }

        //If the player attacks while in the air, go to attacking state.
        //The animator will move on to the correct animation state
        //playerAirAttackCounter makes sure the player can attack only once in the air
        if (Input.GetButtonDown("CTRL - X") && playerAirAttackCounter == 0)
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerRigidBody.gravityScale = 0.0f;
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.Attacking;
            playerAttackCounter++;
            playerAirAttackCounter++;
        }
        //Same concept for bow attack
        if (Input.GetButtonDown("CTRL - Y") && playerAirBowCounter == 0)
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerRigidBody.gravityScale = 0.0f;
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.BowAttack;
            playerAttackCounter++;
            playerAirBowCounter++;
        }

        if(playerVertical>0.0f && Input.GetButtonDown ("CTRL - X") && !playerHasDownThrust)
        {
            //Go to downthrus state and animation
            playerCurrentState = playerStates.DownThrust;
            playerJumpTimer = playerJumpTimerStart;
            playerAnimator.SetBool("playerDownThrust", true);
            playerHasDownThrust = true;
            //Reset attack counters
            playerAttackCounter = 0;
            playerAirBowCounter = 0;
        }
    }

    //-------------------DownThrust--------------

    private void playerDownThrust()
    {
        //Accelerate downwards
        playerRigidBody.gravityScale = 3.0f;
        playerRigidBody.velocity = new Vector2(playerSpeed.x * playerHorizontal, -12.0f * playerSpeedModifier);

        
            //Overlap box should move with the player
            playerAttackCenter = new Vector2(gameObject.transform.position.x + 0.2f * playerDirection, gameObject.transform.position.y);
            playerAttackSize = new Vector2(1.2f, 1.5f);

            enemies = Physics2D.OverlapBoxAll(playerAttackCenter, playerAttackSize, 0.0f, playerDamage);
            for (int i = 0; i < enemies.Length; i++)
            {
                //SwordEffect
                playerSwordEffectPos = new Vector2(enemies[i].gameObject.transform.position.x - 0.2f, enemies[i].gameObject.transform.position.y - 0.4f);
                playerSwordEffectInsta = Instantiate(playerSwordEffectToInsta, playerSwordEffectPos, Quaternion.Euler(new Vector3(0.0f, 0.0f, 54.051f)));
                Destroy(playerSwordEffectInsta, 0.5f);
                playerCamera.cameraShake = true;

                enemies[i].GetComponent<Enemy>().enemyTakeDamage(1);

                if (!playerInSpeedMode)
                   {
                     playerSpeedHits++;
                   }
                else
                   {
                    playerSpeedIncreaseTimer = playerSpeedIncreaseTimerStartValue;
                   }
            }
    }
    //-------------------Wall--------------

    private void playerWall()
    {
        playerHorizontal = Input.GetAxisRaw("Horizontal");

        if( (playerWallJumpDirection.x > 0.0f && playerHorizontal > 0.0f) || 
            (playerWallJumpDirection.x < 0.0f && playerHorizontal < 0.0f))
        {
            
            //If the player is moving the stick away from the wall, fall
            playerAnimator.SetTrigger("playerFall");
            playerAnimator.SetBool("playerWall", false);
            playerCurrentState = playerStates.Fall;
        }

        if(Input.GetButtonDown("CTRL - A"))
        {
            playerHasWallJumped = true;
            playerAnimator.SetBool("playerWall", false);
            playerAnimator.ResetTrigger("playerFall");
            playerCurrentState = playerStates.Jumping;
        }

    }

    //-------------------Fall--------------

    private void playerFall()
    {
        //Allow directional control during the fall
        playerHorizontal = Input.GetAxisRaw("Horizontal");
        playerVertical = Input.GetAxisRaw("Vertical");
        playerRigidBody.velocity = new Vector2(playerSpeed.x * playerHorizontal, -6.0f * playerSpeedModifier);
        playerFlip();
        playerRigidBody.gravityScale = 2.0f;

        //If the player attacks while in the air, go to attacking state.
        //The animator will move on to the correct animation state
        //playerAirAttackCounter makes sure the player can attack only once in the air
        if (Input.GetButtonDown("CTRL - X") && playerAirAttackCounter == 0)
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerRigidBody.gravityScale = 0.0f;
            playerCurrentState = playerStates.Attacking;
            playerAttackCounter++;
            playerAirAttackCounter++;
        }
        //Same concept for bow attacks
        if (Input.GetButtonDown("CTRL - Y") && playerAirBowCounter == 0)
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerRigidBody.gravityScale = 0.0f;
            playerCurrentState = playerStates.BowAttack;
            playerAttackCounter++;
            playerAirBowCounter++;
        }
        //DownThrust check
        if (playerVertical > 0.0f && Input.GetButtonDown("CTRL - X") && !playerHasDownThrust)
        {
            //Go to downthrus state and animation
            playerCurrentState = playerStates.DownThrust;
            playerJumpTimer = playerJumpTimerStart;
            playerAnimator.SetBool("playerDownThrust", true);
            playerHasDownThrust = true;
            //Reset attack counters
            playerAttackCounter = 0;
            playerAirBowCounter = 0;
        }
    }

    //-------------------Attack--------------

    private void playerAttack()
    {
        //Give the player directional control when attacking
        playerHorizontal = Input.GetAxisRaw("Horizontal");
        playerVertical = Input.GetAxisRaw("Vertical");
        playerFlip();

        //Jump into bow state if the button is pressed
        if (Input.GetButtonDown("CTRL - Y"))
        {
            playerCurrentState = playerStates.BowAttack;
        }

        //The animator will play the correct animation based on the playerAttackCounter
        playerAnimator.SetInteger("playerAttack", playerAttackCounter);

        //If an attack animation is still playing, don't change anything
        if (!playerContinueCombo)
        {
            //We check for both 1 and 2 to accommodate for a directional flip should it happen
            if (playerAttackCounter == 1)
            {
                playerSwordEffectEulerRot = new Vector3(0.0f, 0.0f, 54.051f);
                playerSwordEffectScale = new Vector3(1.0f, 1.0f, 1.0f);
                playerAttackCenter = new Vector2(gameObject.transform.position.x + 0.5f * playerDirection, gameObject.transform.position.y);

                playerAttackSize = new Vector2(1.3f, 1.0f);
            }
            else if(playerAttackCounter == 2)
            {
                playerSwordEffectEulerRot = new Vector3(0.0f, 0.0f, 184.586f);
                playerSwordEffectScale = new Vector3(-1.0f, 1.0f, 1.0f);
                playerAttackCenter = new Vector2(gameObject.transform.position.x + 0.5f * playerDirection, gameObject.transform.position.y);

                playerAttackSize = new Vector2(1.3f, 1.0f);
            }
            else if (playerAttackCounter == 3)
            {
                playerSwordEffectEulerRot = new Vector3(73.93201f, 0.0f, 313.191f);
                playerSwordEffectScale = new Vector3(1.0f, 1.0f, 1.0f);

                playerAttackCenter = new Vector2(gameObject.transform.position.x + 0.5f * playerDirection, gameObject.transform.position.y - 0.2f);

                playerAttackSize = new Vector2(1.5f, 0.8f);
               
            }
        }
        else
        {
            //Move onto the next combo
            if(Input.GetButtonDown("CTRL - X"))
            {
                playerAttackCounter++;
                playerContinueCombo = false;
            }
        }

        //DownThrust check
        if (playerVertical > 0.0f && Input.GetButtonDown("CTRL - X") && !playerHasDownThrust)
        {
            //Go to downthrus state and animation
            playerCurrentState = playerStates.DownThrust;
            playerJumpTimer = playerJumpTimerStart;
            playerAnimator.SetBool("playerDownThrust", true);
            playerHasDownThrust = true;
            //Reset attack counters
            playerAttackCounter = 0;
            playerAirBowCounter = 0;
        }
    }

    //-------------------Bow--------------

        //Update the bow instantiation position and start the animation
    private void playerBowAttack()
    {
        playerArrowPos = new Vector2(gameObject.transform.position.x + 0.5f,  transform.position.y);
        playerAnimator.SetBool("playerBow", true);
    }

    //-------------------Methods--------------

    private void playerFlip()
    {
        if ((playerHorizontal > 0.0f && !playerFacingRight) || (playerHorizontal < 0.0f && playerFacingRight))
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1.0f, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            playerFacingRight = !playerFacingRight;
            playerDirection *= -1;

            if(playerDirection == 1)
            {
                playerPaticleShape.alignToDirection = false;
                playerSpeedEmitter.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
            }
            else
            {
                playerPaticleShape.alignToDirection = true;
                playerSpeedEmitter.transform.eulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
            }
            
        }
    }

    private void playerwallFlip()
    {
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1.0f, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        playerFacingRight = !playerFacingRight;
        playerDirection *= -1;

        if (playerDirection == 1)
        {
            playerPaticleShape.alignToDirection = false;
            playerSpeedEmitter.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        }
        else
        {
            playerPaticleShape.alignToDirection = true;
            playerSpeedEmitter.transform.eulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        }
    }

    public void playerTakeDamage(float damage)
    {
        //Remove some health from the player and reflect that in the health bar
        playerHealth -= damage;
        playerHPBar.fillAmount -= (damage / playerMaxHealth);


        //**********************Placeholder should be removed for a proper respawn system**************************
        //If the player's health is less than zero return to the beginning of the scene
        if(playerHealth <= 0.0f)
        {
            gameObject.transform.position = playerStartingPosition;
            playerHealth = playerMaxHealth;
            playerHPBar.fillAmount = 1.0f;
        }
    }

    //-------------------Animator methods--------------


    //****************Attack****************

    private void playerApplyDamage()
    {
        //Create an overlap box to damage enemies
        enemies = Physics2D.OverlapBoxAll(playerAttackCenter, playerAttackSize, 0.0f, playerDamage);
        for (int i = 0;  i< enemies.Length; i++)
        {
            //Sword Effect
            playerSwordEffectPos = new Vector2(enemies[i].gameObject.transform.position.x - 0.2f, enemies[i].gameObject.transform.position.y - 0.4f) ;
            playerSwordEffectInsta = Instantiate(playerSwordEffectToInsta, playerSwordEffectPos, Quaternion.Euler(playerSwordEffectEulerRot));
            playerSwordEffectInsta.gameObject.transform.localScale = playerSwordEffectScale;
            Destroy(playerSwordEffectInsta, 0.5f);
            enemies[i].GetComponent<Enemy>().enemyTakeDamage(5);
                playerCamera.cameraShake = true; //Shake the camera
                playerHitStop = true;
                Time.timeScale = 0.0f;

            //If not in speed mode, increase the number of hits.
            //Otherwise reset the speed mode timer
            if (!playerInSpeedMode)
            {
                playerSpeedHits++;
            }
            else
            {
                playerSpeedIncreaseTimer = playerSpeedIncreaseTimerStartValue;
            }
        }
        playerContinueCombo = true;

    }

    private void playerNoCombo()
    {
        //If the player does not do the combo in time, reset the combo meter
            playerContinueCombo = false;
            playerAttackCounter = 0;
            playerAnimator.SetInteger("playerAttack", playerAttackCounter);
            playerCurrentState = playerStates.Idle;
    }

    private void playerNoComboAir()
    {
        //If the player does not do the combo in time, reset the combo meter
            playerContinueCombo = false;
            playerAttackCounter = 0;
            playerAnimator.SetInteger("playerAttack", playerAttackCounter);
            playerCurrentState = playerStates.Fall;
    }

    //****************Bow****************

    private void playerShootArrow()
    {
        playerArrowForce = new Vector2(1200.0f * playerDirection, 0.0f);
        playerInstaArrow = Instantiate(playerArrow, playerArrowPos, gameObject.transform.rotation);
        playerInstaArrow.gameObject.transform.localScale = new Vector3(playerInstaArrow.gameObject.transform.localScale.x * playerDirection, playerInstaArrow.gameObject.transform.localScale.y, playerInstaArrow.gameObject.transform.localScale.z);
        playerInstaArrow.GetComponent<Rigidbody2D>().AddForce(playerArrowForce);
        playerNoCombo();
    }

    private void playerShootArrowAir()
    {
        playerArrowForce = new Vector2(1200.0f * playerDirection, 0.0f);
        playerInstaArrow = Instantiate(playerArrow, playerArrowPos, gameObject.transform.rotation);
        playerInstaArrow.gameObject.transform.localScale = new Vector3(playerInstaArrow.gameObject.transform.localScale.x * playerDirection, playerInstaArrow.gameObject.transform.localScale.y, playerInstaArrow.gameObject.transform.localScale.z);
        playerInstaArrow.GetComponent<Rigidbody2D>().AddForce(playerArrowForce);
        playerNoComboAir();
    }

    private void playerArrowHasBeenShot()
    {
        playerAnimator.SetBool("playerBow", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerAttackCenter, playerAttackSize);
    }


    //-------------------Collisions--------------
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.otherCollider == playerFeet)
        {
            //col.collider.bounds
            playerIsGrounded = true;
            playerRigidBody.gravityScale = 1.0f;
            playerJumpTimer = playerJumpTimerStart;
            playerHasWallJumped = false;
            playerAirAttackCounter = 0;
            playerAirBowCounter = 0;
            playerAnimator.SetBool("playerGrounded", true);
            playerAnimator.SetBool("playerFallFromIdle", false);
            playerAnimator.SetBool("playerWall", false);

            //If the player is touching the ground after a down thrust, wait for the idle animation to start before going into that state
            if (playerHasDownThrust)
            {
                playerAnimator.SetBool("playerDownThrust", false);
            }
            else
            {
                //Otherwise, just go to idle
                playerCurrentState = playerStates.Idle;
            }

        }

        //If you hit the wall while jumping or falling, go into wall mode
        else if (col.gameObject.tag.Equals("Wall") && (playerCurrentState == playerStates.Jumping || playerCurrentState == playerStates.Fall))
        {
           
            playerWallJumpDirection = col.GetContact(0).normal;
            playerPreviousWallJumpDirection = playerWallJumpDirection * -1.0f; //The jump onto the wall is always opposite to the wall's normal

            //If the player doesn't hit the wall from the head, go into wall state...
            if (playerWallJumpDirection.y == 0.0f)
            {
                //Flip the player when attached to a wall
                if(playerPreviousWallJumpDirection.x>0.0f && !playerFacingRight)
                {
                    playerwallFlip();
                }
                else if(playerPreviousWallJumpDirection.x < 0.0f && playerFacingRight)
                {
                    playerwallFlip();
                }

                playerHasWallJumped = false;
                playerAnimator.SetBool("playerWall", true);
                playerJumpTimer = playerJumpTimerStart;
                playerDashJumpModifier = 1.0f;
                playerCurrentState = playerStates.Wall;
                playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
                playerRigidBody.gravityScale = 0.5f;

            }
            //Otherwise, go into fall state
            else
            {
                playerAnimator.SetBool("playerFall", true);
                playerCurrentState = playerStates.Fall;
                playerJumpTimer = playerJumpTimerStart;
                playerDashJumpModifier = 1.0f;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.otherCollider == playerFeet)
        {
            playerIsGrounded = false;
            if(playerCurrentState!=playerStates.Jumping)
            {
                //Go into fall state if you "move" off the edge
                playerAnimator.SetBool("playerFallFromIdle",true);
                //In case the player dashes off the edge
                playerAnimator.SetBool("playerDash", false);
                playerParticleSystem.Stop();
                playerCurrentState = playerStates.Fall;
            }
           
        }
        //If you slide off the wall, go into falling state
        else if(col.gameObject.tag.Equals("Wall") && playerCurrentState == playerStates.Wall)
        {
            if((playerWallJumpDirection.x > 0.0f || playerWallJumpDirection.x < 0.0f))
            {
                
                playerAnimator.SetBool("playerFall", true);
                playerAnimator.SetBool("playerWall", false);
                playerCurrentState = playerStates.Fall;
                playerJumpTimer = playerJumpTimerStart;
                playerDashJumpModifier = 1.0f;
                playerHasWallJumped = false;
            }
        }
    }
}
