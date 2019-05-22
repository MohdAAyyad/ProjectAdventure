using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Components
    [HideInInspector]
    public Rigidbody2D playerRigidBody;
    [HideInInspector]
    public Animator playerAnimator;
    private ParticleSystem playerParticleSystem;
    [HideInInspector]
    public SpriteRenderer playerSpriteRenderer;
    private MCamera playerCamera;
    private PlayerAbilities playerAbl;
    [HideInInspector]
    public Sounds playerSound;
    [HideInInspector]
    public AudioSource playerAudioSource;
    public AudioSource playerAttackAudioSource; //using a different audio source for the attack sounds to use a lower volume

    //UI
    public Image playerHPBar;
    public RawImage playerPotion1;
    public RawImage playerPotion2;
    public RawImage playerPotion3;
    public RectTransform playerPausePanel;
    public LevelManager playerLM;
    public Text playerExpText;
    public Text playerExpAddText;
    private bool gamePaused;

    //EXP
    public static int playerExp = 0;
    private bool playerExpIsIncreasing;
    public static int playerNewExp;
    private float playerExpTimer;
    private int playerAccumulativeExpAdd;

    //Health
    private float playerHealth;
    private float playerMaxHealth;
    private Vector3 playerStartingPosition;
    [HideInInspector]
    public bool playerIFrames;
    private bool playerHasTakenDamage;
    private float playerTakeDamageTimer;
    private bool playerIsHealing;
    [HideInInspector]
    public bool playerHealNow;
    private int playerPotionsCount;
    private bool playerHealItemInstantiated;
    public PlayerHeal playerHealItem;
    private PlayerHeal playerHealItemInsta;

    //Status
    private float playerStatusTimer;
    private string playerStatus;
    private bool playerStatusHasSymbol;
    private GameObject playerStatusInsta;
    private Vector2 playerStatusInstaPos;
    public GameObject playerPoisonStatus;


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
        Wall,
        SAbility //Sword ability
    }
    private playerStates playerCurrentState;

    //Axis
    private float playerHorizontal;
    private float playerVertical;
    private float playerAblTrigger1;
    private float playerAblTrigger2;

    //Idle
    private Vector2 playerSpeed;
    private bool playerFacingRight;
    [HideInInspector]
    public int playerDirection; //Public, used in player abilities

    //Dash
    private float playerDashTimer;
    private float playerDashScalar;
    private float playerDashJumpModifier;
    private float playerUntilNextDash;
    private bool playerHasDashed;
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

    //Ability1
    //++DTE
    private bool DTE;

    //Ability2
    private bool specialArrow;

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
        playerSound = GameObject.Find("AudioManager").GetComponent<Sounds>();
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
        playerAnimator = gameObject.GetComponent<Animator>();
        playerParticleSystem = gameObject.GetComponent<ParticleSystem>();
        playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<MCamera>();
        playerPaticleShape = playerParticleSystem.shape;
        playerParticleSystem.Stop();
        playerAbl = gameObject.GetComponent<PlayerAbilities>();
        playerAudioSource = gameObject.GetComponent<AudioSource>();


        //Health
        playerHealth = playerMaxHealth = 100;
        playerIFrames = false;
        playerHasTakenDamage = false;
        playerTakeDamageTimer = 0.15f;
        playerHealItemInstantiated = false;
        playerPotionsCount = 3;
        playerIsHealing = false;
        playerHealNow = false;
        gameObject.transform.position = playerStartingPosition = GManager.checkpointPos;
        playerStartingPosition = gameObject.transform.position;
        playerHPBar.fillAmount = 1.0f;

        //UI
        gamePaused = false;
        playerPausePanel.gameObject.SetActive(false);
        Time.timeScale = 1.0f;

        //EXP
        playerExpIsIncreasing = false;
        playerExpTimer = 0.05f;
        playerAccumulativeExpAdd = 0;

        //Status
        playerStatusTimer = 0.0f;
        playerStatus = "";
        playerStatusHasSymbol = false;
        playerStatusInstaPos = gameObject.transform.position;

        //States
        playerCurrentState = playerStates.Idle;

        //Idle
        playerFacingRight = true;
        playerDirection = 1;
        playerSpeed = new Vector2(5.0f, 0.0f);

        //Dash
        playerDashTimer = 0.3f;
        playerDashScalar = 2.0f;
        playerDashJumpModifier = 1.0f;
        playerUntilNextDash = 0.5f;
        playerHasDashed = false;

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
        playerHitStopCounter = 4;

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

        //Abilities
        DTE = false;

        //Effects
        playerDustTimer = 0.8f;
        playerSwordEffectToInsta = playerSwordEffect;

    }

    void Update()
    {
        playerPauseGame(Input.GetButtonDown("CTRL - START"));

        if (!gamePaused)
        {
            playerEXPupdateUI(playerExpIsIncreasing);

            //Check status of the player (poisoned, paralyzed...etc)
            if (playerStatus != "")
            {
                playerCheckStatus();
            }

            if (playerHasTakenDamage)
            {
                playerShowDamage();
            }

            //Pause the game for a split second for the hitstop effect
            playerApplyHitStop(playerHitStop);
            //Check for Speed Mode
            playerSpeedMode();

            //Move into idle when animation Idle has started playing after a down thrust
            if (playerHasDownThrust)
            {
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle"))
                {
                    //Make sure the attack animation doesn't run once you hit the ground
                    playerAnimator.SetInteger("playerAttack", 0);
                    playerCurrentState = playerStates.Idle;
                    playerHasDownThrust = false;
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
                case playerStates.SAbility:
                    playerSAbility();
                    break;

            }
        }
    }

    //-------------------Idle--------------

    //In idle the character moves and waits for input
    private void playerIdle()
    {
        //If the palyer is healing, they cannot do anything else until either the healing is cancelled or finished
        if (!playerIsHealing)
        {
            if (Input.GetButtonDown("CTRL - LB") && playerHorizontal == 0.0f)
            { 
                playerIsHealing = true;
            }
            //Get axis, apply it to the velocity, and update the animation
            playerHorizontal = Input.GetAxisRaw("Horizontal");
            playerAblTrigger1 = Input.GetAxisRaw("CTRL - RT");
            playerAblTrigger2 = Input.GetAxisRaw("CTRL - LT");
            playerRigidBody.velocity = playerSpeed * playerHorizontal * playerSpeedModifier;
            playerAnimator.SetFloat("playerSpeed", Mathf.Abs(playerSpeed.x * playerHorizontal));

            //Flip
            playerFlip();

            if (Mathf.Abs(playerSpeed.x * playerHorizontal) > 0.0f)
            {
                if (playerDustTimer > 0.0f)
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
            if (Input.GetButtonDown("CTRL - X"))
            {

                playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
                playerCurrentState = playerStates.Attacking;
                playerAttackCounter++;//Player attack counter is passed to the animator to play the correct animation
                playerAttackAudioSource.PlayOneShot(playerSound.swordNoImpact);
            }
            //If the palyer presses RT in idle and the current ability is set to SB, then activate it
            else if (playerAblTrigger1 > 0.0f && playerAbl.ID1 == 2 && !playerAbl.ability1OnCooldown)
            {
                playerAbl.ability1Used = true;
            }
            else if(playerAblTrigger1 > 0.0f && playerAbl.ID1 == 3 && !playerAbl.ability1OnCooldown)
            {
                playerAbl.ability1Used = true;
                playerAnimator.SetInteger("playerAblID", 3);
                playerCurrentState = playerStates.SAbility;
            }

            //Check for jump
            if (Input.GetButtonDown("CTRL - A") && playerIsGrounded)
            {
                //Effect
                playerJumpEffectPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.8f);
                Destroy(Instantiate(playerJumpEffect, playerJumpEffectPos, gameObject.transform.rotation), 0.5f);
                playerJumpTimer = playerJumpTimerStart;
                playerCurrentState = playerStates.Jumping;
                playerAnimator.SetBool("playerGrounded", false);

            }
            //Check for bow attacks
            if (Input.GetButtonDown("CTRL - Y"))
            {
                playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
                playerCurrentState = playerStates.BowAttack;
            }
            //Check for special bow attack
            else if(Mathf.Abs(playerAblTrigger2)>0.0f && playerAbl.ID2>0 && !playerAbl.ability2OnCooldown)
            {
                 playerAbl.ability2Used = true;
                 specialArrow = true; //Checked when instantiating the arrow
                 playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
                 playerCurrentState = playerStates.BowAttack;
            }
            if (Input.GetButtonDown("CTRL - B") && !playerHasDashed)
            {
                playerParticleSystem.Play();
                //Change active collider
                playerCollider.enabled = false;
                playerDashCollider.enabled = true;
                playerAnimator.SetBool("playerDash", true);
                playerCurrentState = playerStates.Dash;
                playerHasDashed = true;
                playerIFrames = true;
            }

            if (playerHasDashed)
            {
                if (playerUntilNextDash > 0.0f)
                {
                    playerUntilNextDash -= Time.deltaTime;
                }
                else
                {
                    playerUntilNextDash = 0.5f;
                    playerHasDashed = false;
                }
            }
        }
        else
        {
            if(playerHealNow)
            {
                playerAudioSource.PlayOneShot(playerSound.heal);

                //Invoked at the end of the healing effect by its own script
                playerPotionsCount--;
                playerHealth += 40.0f;

                if(playerHealth>playerMaxHealth)
                {
                    playerHealth = playerMaxHealth;
                }

                playerHPBar.fillAmount = playerHealth / playerMaxHealth;
                playerHealNow = false;
                playerHealItemInstantiated = false;

                //Reflect the number of potions left on the UI
                if(playerPotionsCount == 2)
                {
                    playerPotion3.enabled = false;
                }

                else if (playerPotionsCount == 1)
                {
                    playerPotion2.enabled = false;
                }

                else if (playerPotionsCount == 0)
                {
                    playerPotion1.enabled = false;
                }
            }
            else if(Input.GetButton("CTRL - LB"))
            {
                if(!playerHealItemInstantiated && playerPotionsCount>0.0f && playerHealth<playerMaxHealth)
                {
                    playerHealItemInsta = Instantiate(playerHealItem, gameObject.transform.position, gameObject.transform.rotation).GetComponent<PlayerHeal>();
                    playerHealItemInsta.playerRef = this;
                    playerHealItemInstantiated = true;
                }
            }
            else if(Input.GetButtonUp("CTRL - LB"))
            {
                //Interrupt the healing process by destroying the effect
                if (playerHealItemInsta)
                {
                    Destroy(playerHealItemInsta.gameObject);
                }
                playerIsHealing = false;
                playerHealItemInstantiated = false;
            }
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
                playerIFrames = false;
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
            playerIFrames = false;
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
            playerAblTrigger1 = Input.GetAxisRaw("CTRL - RT");
            playerAblTrigger2 = Input.GetAxisRaw("CTRL - LT");
            //Check if the player has wall jumped
            if (!playerHasWallJumped)
            {
                playerRigidBody.velocity = new Vector2(playerSpeed.x * playerHorizontal, 8.0f * playerDashJumpModifier);
                playerFlip();
            }
            else
            {
                //If the player is jumping off a wall, the inital direction of the jump is based on the normal going out of the wall
                playerRigidBody.velocity = new Vector2(playerSpeed.x  * playerWallJumpDirection.x * 1.5f, 5.0f);
                
            }
            playerJumpTimer -= Time.deltaTime;
        }
        else
        {
            //Once the jump timer runs out, go into falling state
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.Fall;
            playerDashJumpModifier = 1.0f;
            playerAnimator.SetBool("playerFall",true);
        }

        //If the player attacks while in the air, go to attacking state.
        //The animator will move on to the correct animation state
        //playerAirAttackCounter makes sure the player can do only one combo in the air
        if (Input.GetButtonDown("CTRL - X") && playerAirAttackCounter == 0)
        {
            
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerRigidBody.gravityScale = 0.0f;
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.Attacking;
            playerAttackCounter++;
            playerAirAttackCounter++;
            playerAttackAudioSource.PlayOneShot(playerSound.swordNoImpact);
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
        //Check for special bow attack
        else if (Mathf.Abs(playerAblTrigger2) > 0.0f && playerAbl.ID2 > 0 && !playerAbl.ability2OnCooldown)
        {
            playerAbl.ability2Used = true;
            specialArrow = true; //Checked when instantiating the arrow
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            playerRigidBody.gravityScale = 0.0f;
            playerJumpTimer = playerJumpTimerStart;
            playerCurrentState = playerStates.BowAttack;
            playerAttackCounter++;
            playerAirBowCounter++;
        }

        if (playerVertical>0.0f && Input.GetButtonDown ("CTRL - X") && !playerHasDownThrust) 
        {
            //Go to downthrus state and animation
            playerCurrentState = playerStates.DownThrust;
            playerJumpTimer = playerJumpTimerStart;
            playerAnimator.SetBool("playerDownThrust", true);
            playerAnimator.SetBool("playerGrounded", true);
            playerHasDownThrust = true;
            //Reset attack counters
            playerAttackCounter = 0;
            playerAirBowCounter = 0;
            playerDashJumpModifier = 1.0f;
            playerAnimator.SetInteger("playerAttack", playerAttackCounter);
        }
        //If the player RT and the assigned ability is DTE then go into a downthrust to use the ability
        else if(Mathf.Abs(playerAblTrigger1) > 0.0f && playerAbl.ID1 == 1 && !playerAbl.ability1OnCooldown)
        {
            playerAbl.ability1Used = true;
            DTE = true;
            //Go to downthrust state and animation
            playerCurrentState = playerStates.DownThrust;
            playerJumpTimer = playerJumpTimerStart;
            playerAnimator.SetBool("playerDownThrust", true);
            playerAnimator.SetBool("playerGrounded", true);
            playerHasDownThrust = true;
            //Reset attack counters
            playerAttackCounter = 0;
            playerAirBowCounter = 0;
            playerAnimator.SetInteger("playerAttack", playerAttackCounter);
        }
        //Check for SB
        else if (playerAblTrigger1 > 0.0f && playerAbl.ID1 == 2 && !playerAbl.ability1OnCooldown)
        {
            playerAbl.ability1Used = true;
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
            playerAnimator.SetBool("playerFall",true);
            playerAnimator.SetBool("playerWall", false);
            playerCurrentState = playerStates.Fall;
        }

        if(Input.GetButtonDown("CTRL - A"))
        {
            playerHasWallJumped = true;
            playerAnimator.SetBool("playerWall", false);
            playerAnimator.SetBool("playerFall",false);
            playerCurrentState = playerStates.Jumping;
        }

    }

    //-------------------Fall--------------

    private void playerFall()
    {
        //If the player uses an ability and falls immediately afterwards (i.e. off the edge for example) reset abilityHasEnded
        playerAbl.abilityHasEnded = false;
        //Allow directional control during the fall
        playerHorizontal = Input.GetAxisRaw("Horizontal");
        playerVertical = Input.GetAxisRaw("Vertical");
        playerAblTrigger1 = Input.GetAxisRaw("CTRL - RT");
        playerAblTrigger2 = Input.GetAxisRaw("CTRL - LT");
        playerRigidBody.velocity = new Vector2(playerSpeed.x * playerHorizontal, -6.0f);
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
            playerAttackAudioSource.PlayOneShot(playerSound.swordNoImpact);
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
        //Check for special bow attack
        else if (Mathf.Abs(playerAblTrigger2) > 0.0f && playerAbl.ID2 > 0 && !playerAbl.ability2OnCooldown)
        {
            playerAbl.ability2Used = true;
            specialArrow = true; //Checked when instantiating the arrow
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
            playerAnimator.SetBool("playerFall", false);
            playerHasDownThrust = true;
            //Reset attack counters
            playerAttackCounter = 0;
            playerAirBowCounter = 0;
        }
        //DTE check
        else if (Mathf.Abs(playerAblTrigger1) > 0.0f && playerAbl.ID1 == 1 && !playerAbl.ability1OnCooldown)
        {
           // Debug.Log("Ability");
            playerAbl.ability1Used = true;
            DTE = true;
            //Go to downthrus state and animation
            playerCurrentState = playerStates.DownThrust;
            playerJumpTimer = playerJumpTimerStart;
            playerAnimator.SetBool("playerDownThrust", true);
            playerAnimator.SetBool("playerFall", false);
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
        playerAblTrigger1 = Input.GetAxisRaw("CTRL - RT");
        playerFlip();

        //Cancel into bow state if the button is pressed
        if (Input.GetButtonDown("CTRL - Y"))
        {
            playerCurrentState = playerStates.BowAttack;
        }
        //Check for SB
        else if (playerAblTrigger1 > 0.0f && playerAbl.ID1 == 2 && !playerAbl.ability1OnCooldown)
        {
            playerAbl.ability1Used = true;
        }
        else if (playerAblTrigger1 > 0.0f && playerAbl.ID1 == 3 && !playerAbl.ability1OnCooldown)
        {
            playerAbl.ability1Used = true;
            playerAnimator.SetInteger("playerAblID", 3);
            playerCurrentState = playerStates.SAbility;
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
                playerAttackAudioSource.PlayOneShot(playerSound.swordNoImpact);
                playerAttackCounter++;
                playerContinueCombo = false;
            }
        }

        //DownThrust check
        if (playerVertical > 0.0f && Input.GetButtonDown("CTRL - X") && !playerHasDownThrust &&!playerIsGrounded)
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

    //-----------------SAbility-----------

    private void playerSAbility()
    {
        if(playerAbl.abilityHasEnded)
        {
            playerCurrentState = playerStates.Idle;
            playerAbl.abilityHasEnded = false;
        }
    }


    //-------------------Bow--------------

    //Update the bow instantiation position and start the animation
    private void playerBowAttack()
    {
        playerArrowPos = new Vector2(gameObject.transform.position.x + 0.5f,  transform.position.y);
        playerAnimator.SetBool("playerBow", true);
        playerAblTrigger1 = Input.GetAxisRaw("CTRL - RT");
       
        //SB check
        if (playerAblTrigger1 > 0.0f && playerAbl.ID1 == 2 && !playerAbl.ability1OnCooldown)
        {
            playerAbl.ability1Used = true;
        }
        else if (playerAblTrigger1 > 0.0f && playerAbl.ID1 == 3 && !playerAbl.ability1OnCooldown)
        {
            playerAbl.ability1Used = true;
            playerCurrentState = playerStates.SAbility;
        }
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
        if (!playerIFrames)
        {
            //If the palyer has a shield summoned, destroy one of the two shilds instead of damaging the player
            if (playerAbl.shieldActive)
            {
                playerAbl.DestroyShield();
            }
            else
            {
                //Remove some health from the player and reflect that in the health bar if iframes are not active
                playerHealth -= damage;
                playerHPBar.fillAmount -= (damage / playerMaxHealth);
                playerCamera.cameraShake = true;
                playerHasTakenDamage = true;
                //If the player gets damaged while healing, interrupt the healing animation
                if (playerHealItemInsta)
                {
                    Destroy(playerHealItemInsta.gameObject);
                    playerIsHealing = false;
                    playerHealItemInstantiated = false;
                }
            }

        }


        //**********************Placeholder should be removed for a proper respawn system**************************
        //If the player's health is less than zero reload the level
        if(playerHealth <= 0.0f)
        {
            playerLM.loadScene("Level 1"); //Bad practice, for demo purposes
        }
    }


    public void playerTakeDamageAndStatus(float damage, string status, float time)
    {
        if(!playerIFrames)
        {
            if (playerAbl.shieldActive)
            {
                playerAbl.DestroyShield();
            }
            else
            {
                playerTakeDamage(damage);
                if (playerStatus == "")
                {
                    playerStatus = status;
                    playerStatusTimer = time;
                }
            }
        }
    }

    private void playerCheckStatus()
    {
        if (playerStatusTimer > 0.0f)
        {
            switch (playerStatus)
            {
                case "Poison":
                    playerTakeDamage(0.01f);
                    if(!playerStatusHasSymbol)
                    {
                        //Instantiate the poison symbol and make it a child of the player
                        playerStatusInstaPos = new Vector2(gameObject.transform.position.x + 0.3f, gameObject.transform.position.y + 0.8f);
                        playerStatusInsta = Instantiate(playerPoisonStatus, playerStatusInstaPos, gameObject.transform.rotation);
                        playerStatusInsta.transform.parent = gameObject.transform;
                        playerStatusHasSymbol = true;
                    }
                    break;
            }
            playerStatusTimer -= Time.deltaTime;
        }
        else
        {
            playerStatusTimer = 0.0f;
            playerStatus = "";
            playerStatusHasSymbol = false;

            if(playerStatusInsta !=null)
            {
                Destroy(playerStatusInsta);
            }
        }
    }

    private void playerShowDamage()
    {
        if (playerTakeDamageTimer > 0.0f)
        {
            playerSpriteRenderer.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            playerTakeDamageTimer -= Time.deltaTime;
        }
        else
        {
            playerTakeDamageTimer = 0.15f;

            if (!playerInSpeedMode)
            {
                playerSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                playerSpriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            }
            playerHasTakenDamage = false;
        }
    }

    public void playerUpdateCheckpoint(Vector3 newPos)
    {
        //If this is a new checkpoint, save it and heal the player
        if (playerStartingPosition != newPos)
        {
            playerAudioSource.PlayOneShot(playerSound.checkpoint);
            playerStartingPosition = newPos;

            playerHealth += 20.0f;

            if (playerHealth > playerMaxHealth)
            {
                playerHealth = playerMaxHealth;
            }

            playerHPBar.fillAmount = playerHealth / playerMaxHealth;
        }
    }

    //Called from the EXP drop objects to increase palyer EXP
    public void playerIncreaseEXP(int value)
    {
        //Debug.Log(value);
        //Add to the new exp value
        playerNewExp = playerNewExp  + value;
        //Accumulative exp makes sure the UI is showing the correct increase in case of multiple drops
        playerAccumulativeExpAdd += value;
        playerExpAddText.text = "+ " + playerAccumulativeExpAdd.ToString();
        //Start increaasing the EXP
        playerExpIsIncreasing = true;
    }

    //Add to the player EXP and show it on the UI
    private void playerEXPupdateUI(bool update)
    {
        if(update)
        {

            if (playerNewExp > playerExp)
            {
                if (playerExpTimer > 0.0f)
                {
                    playerExpTimer -= Time.deltaTime;
                }
                else
                {
                    playerExp += 1;
                    playerExpText.text = playerExp.ToString();
                    playerAccumulativeExpAdd--;
                    playerExpAddText.text = "+ " + playerAccumulativeExpAdd.ToString();
                    playerExpTimer = 0.05f;
                }
            }
            else
            {
                playerExpIsIncreasing = false;
                playerExpAddText.text = "";

            }
        }
    }

    //Pause the game if the palyer presses Start. The button press is the boolean being passed.
    private void playerPauseGame(bool paused)
    {
        if(paused)
        {
            gamePaused = !gamePaused;

            if (gamePaused)
            {
                playerPausePanel.gameObject.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {
                playerPausePanel.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
    }

    //Update the palyer's speed mode
    private void playerSpeedMode()
    {
        if (playerSpeedHits >= playerSpeedHitsNeeded)
        {
            if (playerSpeedModifier <= 1.0f)
            {
                playerSpeedModifier = 2.0f;
                playerAnimator.speed = playerAnimator.speed * 1.5f;
                playerInSpeedMode = true;
                playerSpeedEmitter.SetActive(true);
                playerSpeedEmitterPS.Play();
                playerSpriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                playerSwordEffectToInsta = playerSwordEffectSpeedMode;

            }

            if (playerSpeedIncreaseTimer > 0.0f)
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
    }

    //Pause the game for a split second for the hitstop effect
    private void playerApplyHitStop(bool hitstop)
    {
        if(hitstop)
        {
            if (playerHitStopCounter > 0)
            {
                playerHitStopCounter -= 1;
            }
            else
            {
                Time.timeScale = 1.0f;
                playerHitStop = false;
                playerHitStopCounter = 4;
            }
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

            //Play impact sound
            playerAudioSource.PlayOneShot(playerSound.swordAttack);

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

        if (!specialArrow)
        {
            playerAudioSource.PlayOneShot(playerSound.Bow);
            playerInstaArrow = Instantiate(playerArrow, playerArrowPos, gameObject.transform.rotation);
        }
        else
        {
            playerAudioSource.PlayOneShot(playerAbl.ability2Sound);
            //If a special arrow is used, summon the special arrow
            playerInstaArrow = Instantiate(playerAbl.ability2, playerArrowPos, gameObject.transform.rotation);
            specialArrow = false;
        }
        playerInstaArrow.gameObject.transform.localScale = new Vector3(playerInstaArrow.gameObject.transform.localScale.x * playerDirection, playerInstaArrow.gameObject.transform.localScale.y, playerInstaArrow.gameObject.transform.localScale.z);
        playerInstaArrow.GetComponent<Rigidbody2D>().AddForce(playerArrowForce);
        playerNoCombo();
    }

    private void playerShootArrowAir()
    {
        playerArrowForce = new Vector2(1200.0f * playerDirection, 0.0f);
        //Summon standard arrow
        if (!specialArrow)
        {
            playerAudioSource.PlayOneShot(playerSound.Bow);
            playerInstaArrow = Instantiate(playerArrow, playerArrowPos, gameObject.transform.rotation);
        }
        else            //If a special arrow is used, summon the special arrow
        {
            playerAudioSource.PlayOneShot(playerAbl.ability2Sound);
            playerInstaArrow = Instantiate(playerAbl.ability2, playerArrowPos, gameObject.transform.rotation);
            specialArrow = false;
        }
        playerInstaArrow.gameObject.transform.localScale = new Vector3(playerInstaArrow.gameObject.transform.localScale.x * playerDirection, playerInstaArrow.gameObject.transform.localScale.y, playerInstaArrow.gameObject.transform.localScale.z);
        playerInstaArrow.GetComponent<Rigidbody2D>().AddForce(playerArrowForce);
        playerNoComboAir();
    }

    private void playerArrowHasBeenShot()
    {
        playerAnimator.SetBool("playerBow", false);
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerAttackCenter, playerAttackSize);
    }
    */


    //-------------------Collisions--------------
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("DeathPlane"))
        {
            playerTakeDamage(playerMaxHealth);
        }
    }

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
            playerAnimator.SetBool("playerFall", false);

            //If the player is touching the ground after a down thrust, wait for the idle animation to start before going into that state
            if (playerHasDownThrust)
            {
                playerAttackAudioSource.PlayOneShot(playerSound.swordDownThrust);
                //If DTE is used, summon the DTE objects on the right and left of the player
                if(DTE)
                {
                    playerCamera.cameraShake = true;
                    Instantiate(playerAbl.ability1, new Vector3(gameObject.transform.position.x + 0.5f,gameObject.transform.position.y,gameObject.transform.position.z), gameObject.transform.rotation).GetComponent<PlayerDTE>().dteDirection = 1.0f;
                    Instantiate(playerAbl.ability1, new Vector3(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation).GetComponent<PlayerDTE>().dteDirection = -1.0f;
                    DTE = false;
                }
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
            playerFeet.enabled = false; //Disable the playerFeet collider. This forces the ground collider to have a collision enter. Fixes a bug where the player jumps onto the wall from a very short distance
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
                playerAnimator.SetBool("playerFallFromIdle", false);
                playerAnimator.SetBool("playerWall", true);
                playerAnimator.SetBool("playerFall", false);
                playerJumpTimer = playerJumpTimerStart;
                playerDashJumpModifier = 1.0f;
                playerCurrentState = playerStates.Wall;
                playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
                playerRigidBody.gravityScale = 0.5f;
                playerFeet.enabled = true;

            }
            //Otherwise, go into fall state
            else
            {
                playerFeet.enabled = false;
                playerAnimator.SetBool("playerFall", true);
                playerCurrentState = playerStates.Fall;
                playerJumpTimer = playerJumpTimerStart;
                playerDashJumpModifier = 1.0f;
                playerFeet.enabled = true;
            }
        }
        else if(col.gameObject.tag.Equals("Wall") && playerCurrentState == playerStates.DownThrust)
        {
            if (playerHasDownThrust)
            {
                playerAttackAudioSource.PlayOneShot(playerSound.swordDownThrust);
                //If DTE is used, summon the DTE objects on the right and left of the player
                if (DTE)
                {
                    playerCamera.cameraShake = true;
                    Instantiate(playerAbl.ability1, new Vector3(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation).GetComponent<PlayerDTE>().dteDirection = 1.0f;
                    Instantiate(playerAbl.ability1, new Vector3(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation).GetComponent<PlayerDTE>().dteDirection = -1.0f;
                    DTE = false;
                }
                playerAnimator.SetBool("playerDownThrust", false);
                playerAnimator.SetBool("playerGrounded", true);
            }
            else
            {
                //Otherwise, just go to idle
                playerCurrentState = playerStates.Idle;
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
                playerAnimator.SetBool("playerGrounded", false);
                playerAnimator.SetBool("playerFall", true);
                //In case the player dashes off the edge
                playerAnimator.SetBool("playerDash", false);
                playerParticleSystem.Stop();
                playerCurrentState = playerStates.Fall;
                //If the player dashes off the edge, turn off the dash collider
                playerCollider.enabled = true;
                playerDashCollider.enabled = false;
                playerIFrames = false;
                //If the player uses an ability and goes off the edge
                 playerAbl.abilityHasEnded = false; 
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
                playerCollider.enabled = true;
                playerDashCollider.enabled = false;
                playerJumpTimer = playerJumpTimerStart;
                playerDashJumpModifier = 1.0f;
                playerHasWallJumped = false;
            }
        }
    }
}
