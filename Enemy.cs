using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Enemy : MonoBehaviour {

    //UI
    public Image enemyHPBar;

    //Health
	protected float enemyHealth;
    protected float enemyHealthMax;
    public GameObject enemyDeathObject;
    public ExpDrop enemyEXPDrop;
    public int enemyEXPValue;
    private ExpDrop enemyEXPInsta;
    private GameObject enemyAbilityEffect;

    //Linecasting
    //Layermask used to make the enemy ignore himself, and other enemie when raycasting
    [SerializeField] protected LayerMask enemyMovementLayerMask;
    public LayerMask enemyLookForPlayerLayerMask;
    [Range(0.0f, 10.0f)] [SerializeField]
    protected float enemyRaycastLookForPlayerLength;
    protected bool enemyPlayerDetected;

    //Line Cast
    protected Vector3 enemyLineCastPos;
	protected bool enemyIsGroundedCast;
    protected bool enemyIsBlockedCast;
    protected float enemyTimeToFlip;
    protected bool enemyDontFlipNow;

	//Components
	protected SpriteRenderer enemySpriteRenderer;
	protected Rigidbody2D enemyRigidBody;
	protected Transform enemyTransform;
	protected float enemyWidth;
	protected float enemyHeight;
	protected Animator enemyAnimator;
    protected AudioSource enemyAudioSource;
    protected Sounds enemySounds;

    //Movement
    protected Vector2 enemyStartingPosition;
    protected int enemyMovementDirection;
	public float enemySpeed = 2.0f;
	protected Vector3 enemyRotation;

	//Player
	protected GameObject enemyFindPlayer;
	protected Vector3 enemyPlayerPosition;
	protected Vector2 enemyDistanceFromPlayer;
    protected Vector2 enemyDirectionToPlayer;
    protected float enemyAttackTimer;
    protected bool enemyActive;
    protected float enemyTimerToCheckPlayerPos;
    protected float enemyDamageInflicted;


    protected enum enemyState
		//Patroling: searches for player
		//Attacking: Attacks the player
	{
		Patrolling,
		Attacking
	};
	protected enemyState enemyCurrentState;

	[Range(0.0f, 5.0f)]
	[SerializeField] protected float enemyRaycastBlockedLength;

    protected float enemyTakeDamageTime;
    protected bool enemyHasTakenDamage;


    //-----Effects-----//
    //++Paralysis
    protected bool paralyzed;
    protected float paralyzedTimer;
    public GameObject paralyzedSymbol;
    protected GameObject paralyzedSymbolInsta;
    private Vector2 paralyzedSymbolPos;

    //++Bleeding
    protected bool bleeding;
    protected float bleedingTimer;
    public GameObject bleedingSymbol;
    protected GameObject bleedingSymbolInsta;
    private Vector2 bleedingSymbolPos;


    protected virtual void Update()
    {
        enemyCheckForBleeding(bleeding);
    }

    //Generic movement line cast
    virtual protected void enemyMovementLineCast()
	{

        //Line case position should be flipped with the enemy
        if (enemyMovementDirection == -1)
        {
            enemyLineCastPos = enemyTransform.position + Vector3.left * enemyWidth;
            enemyIsBlockedCast = Physics2D.Linecast(enemyLineCastPos, enemyLineCastPos + enemyTransform.right * enemyMovementDirection * enemyRaycastBlockedLength, enemyMovementLayerMask);
            //Debug.DrawLine(enemyLineCastPos, enemyLineCastPos + enemyTransform.right * enemyMovementDirection * enemyRaycastBlockedLength);
        }
        else
        {
            enemyLineCastPos = enemyTransform.position + Vector3.right * enemyWidth;
            enemyIsBlockedCast = Physics2D.Linecast(enemyLineCastPos, enemyLineCastPos - enemyTransform.right * enemyMovementDirection * enemyRaycastBlockedLength, enemyMovementLayerMask);
            //Debug.DrawLine(enemyLineCastPos, enemyLineCastPos - enemyTransform.right * enemyMovementDirection * enemyRaycastBlockedLength);
        }
        // Returns true when the line collides with a collider.
        // The layer mask at the end makes sure the line does not return true if it collides with the object's own collider.
        enemyIsGroundedCast = Physics2D.Linecast(enemyLineCastPos, enemyLineCastPos - enemyTransform.up * enemyRaycastBlockedLength, enemyMovementLayerMask);
        
        //Debug.DrawLine(enemyLineCastPos, enemyLineCastPos - enemyTransform.up * enemyRaycastBlockedLength);
        

    }

    virtual protected void enemyDetectPlayerLineCast()
    {
        //Line cast position should be flipped with the enemy
        if (enemyMovementDirection == -1)
        {
            enemyLineCastPos = enemyTransform.position + Vector3.left * enemyWidth;
            enemyPlayerDetected = Physics2D.Linecast(enemyLineCastPos, enemyLineCastPos + (enemyTransform.right * enemyMovementDirection * enemyRaycastLookForPlayerLength), enemyLookForPlayerLayerMask);
            Debug.DrawLine(enemyLineCastPos, enemyLineCastPos + (enemyTransform.right * enemyMovementDirection * enemyRaycastLookForPlayerLength), Color.red);

        }
        else
        {
            enemyLineCastPos = enemyTransform.position + Vector3.right * enemyWidth;
            enemyPlayerDetected = Physics2D.Linecast(enemyLineCastPos, enemyLineCastPos + (-enemyTransform.right * enemyMovementDirection * enemyRaycastLookForPlayerLength), enemyLookForPlayerLayerMask);
            Debug.DrawLine(enemyLineCastPos, enemyLineCastPos + (-enemyTransform.right * enemyMovementDirection * enemyRaycastLookForPlayerLength), Color.red);
        }
      //  Debug.Log((enemyTransform.right * enemyMovementDirection * enemyRaycastLookForPlayerLength));

    }


  
    //Flip
	virtual protected void enemyHorizontalFlip()
	{
		//Rotate the enemy around the Y axis by 180 degrees
        enemyMovementDirection *= -1;
        enemyRotation.y += 180;
		enemyTransform.eulerAngles = enemyRotation;

        //Flip the hp bar's fill origin
        if(enemyHPBar.fillOrigin == (int)Image.OriginHorizontal.Right)
        {
            enemyHPBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            enemyHPBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
        
	}

    //Invisible
    virtual protected void enemyNoLongerOnCamera()
    {
        //Basic enemies changes direction once they're off screen
        enemyPlayerPosition = enemyFindPlayer.gameObject.transform.position;
        enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;
        //Enemy looking right and player is to the left of the enemy
        if (enemyMovementDirection == 1 && enemyDistanceFromPlayer.x > 0)
        {
            enemyHorizontalFlip();
        }
        //Enemy looking to the left and player is t othe right of the enemy
        else if (enemyMovementDirection == -1 && enemyDistanceFromPlayer.x < 0)
        {
            enemyHorizontalFlip();
        }
    }

    //Take Damage
    public virtual void enemyTakeDamage(float damage)
    {
        enemyHealth -= damage;
        enemyHPBar.fillAmount -= damage / enemyHealthMax;
        enemyHasTakenDamage = true;

        if(enemyHealth<=0.0f)
        {
            Destroy(gameObject);
        }

        //If the player attacks the enemy from the back, flip (if you're not paralyzed)
        if(!enemyPlayerDetected && !paralyzed)
        {
            enemyHasDetectedPlayer();
        }
    }

    //Take Damage without flipping
    public void enemyTakeDamageFromTrap(int damage)
    {
        enemyHealth -= damage;
        enemyHPBar.fillAmount -= damage / enemyHealthMax;
        enemyHasTakenDamage = true;

        if (enemyHealth <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    protected void enemyShowDamage()
    {
            if (enemyTakeDamageTime > 0.0f)
            {
                enemySpriteRenderer.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                enemyTakeDamageTime -= Time.deltaTime;
            }
            else
            {
                enemyTakeDamageTime = 0.15f;
                enemySpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                enemyHasTakenDamage = false;
            }
    }

    virtual protected void enemyFlipDelay()
    {
    
            if (enemyTimeToFlip > 0.0f)
            {
                enemyTimeToFlip -= Time.deltaTime;
                enemyRigidBody.velocity = new Vector2(0.0f, 0.0f);
                enemyAnimator.SetBool("enemyFlip", true);
            }
            else
            {
                enemyTimeToFlip = 1.5f;
                enemyDontFlipNow = true;
                enemyAnimator.SetBool("enemyFlip", false);
                enemyHorizontalFlip();
            }
        
    }

    //Genertic patrolling behaviod
    protected virtual void enemyPatrol()
    {
        //Move and look for the player
        enemyRigidBody.velocity = new Vector2(enemySpeed * enemyMovementDirection, 0.0f);
        enemyMovementLineCast();
        enemyDetectPlayerLineCast();


        //If the enemy loses ground or is blocked, wait a bit then flip
        if (!enemyIsGroundedCast || enemyIsBlockedCast)
        {
            enemyDontFlipNow = false;
        }

        if (!enemyDontFlipNow)
        {
            enemyFlipDelay();
        }



        //Once the player is detected, stop in place and start shooting
        if (enemyPlayerDetected)
        {
            enemyHasDetectedPlayer();
        }

    }

    //Generic. Go into attacking state once player is detected. Should be overriden if a different behavior is desired
    protected virtual void enemyHasDetectedPlayer()
    {
        enemyCurrentState = enemyState.Attacking;
        enemyRigidBody.velocity = new Vector2(0.0f, 0.0f);
        enemyAnimator.SetBool("enemyDetectedPlayer", true);
        enemyAnimator.SetBool("enemyAttackPlayer", true);
    }

    //Keep checking where the player's position is and accordingly change states
    protected virtual void enemyCheckPlayerPosition()
    {
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

    //Healed
    public void enemyHeal(float healAmount)
    {
        //Check if the enemy needs to be healed to begin with
        if(enemyHealth<enemyHealthMax)
        {
            enemyHealth += healAmount;
            if (enemyHealth>enemyHealthMax)
            {
                enemyHealth = enemyHealthMax;
            }
            enemyHPBar.fillAmount = enemyHealth/enemyHealthMax;
        }
    }

    //-----Effects-----//

    public virtual void enemyTakeDamageAbility(string abilityName, int damage, GameObject abilityEffect)
    {
        enemyTakeDamage(damage);
        switch (abilityName)
        {
            case "EA":
                paralyzed = true;
                paralyzedSymbolPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.8f);
                if (paralyzedSymbolInsta == null)
                {
                    paralyzedSymbolInsta = Instantiate(paralyzedSymbol, paralyzedSymbolPos, gameObject.transform.rotation);
                    paralyzedSymbolInsta.transform.parent = gameObject.transform;
                }
                break;
            case "BA":
                enemyAbilityEffect = abilityEffect;
                break;
            case "PA":
                bleeding = true;
                bleedingSymbolPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.8f);
                if(bleedingSymbolInsta == null)
                {
                    bleedingSymbolInsta = Instantiate(bleedingSymbol, bleedingSymbolPos, gameObject.transform.rotation);
                    bleedingSymbolInsta.transform.parent = gameObject.transform;
                }
                break;
        }
    }

    protected virtual bool enemyCheckForParalysis()
    {
        if(paralyzed)
        {
            if(paralyzedTimer>0.0f)
            {
                paralyzedTimer -= Time.deltaTime;
            }
            else
            {
                paralyzed = false;
                paralyzedTimer = 3.5f;
                enemyAnimator.speed = 1.0f;
                Destroy(paralyzedSymbolInsta);
            }

        }

        return paralyzed;
    }

    protected virtual void enemyCheckForBleeding(bool bleedingActive)
    {
        if (bleedingActive)
        {
            if (bleedingTimer > 0.0f)
            {
                bleedingTimer -= Time.deltaTime;
                enemyTakeDamage(0.2f);
            }
            else
            {
                bleeding = false;
                bleedingTimer = 2.0f;
                Destroy(bleedingSymbolInsta);
            }
        }
    }


    //When the enemies become invisible, they don't need to do anything
    protected void OnBecameVisible()
    {
        enemyActive = true;
    }

    protected void OnBecameInvisible()
    {
        enemyActive = false;
    }

    protected void OnDestroy()
    {
        if(paralyzedSymbolInsta)
        {
            Destroy(paralyzedSymbolInsta);
        }
        if(enemyAbilityEffect!=null)
        {
            if (enemyAbilityEffect.GetComponent<BAEffect>().explode())
            {
                //Do nothing. Basically, if a bomb is attached to you, let it explode when dying.
                enemyAbilityEffect = null;
            }
        }
        Destroy(Instantiate(enemyDeathObject, gameObject.transform.position, gameObject.transform.rotation),1.0f);
        enemyEXPInsta = Instantiate(enemyEXPDrop, gameObject.transform.position, gameObject.transform.rotation);
        //EXP
        enemyEXPInsta.expValue = enemyEXPValue;
        enemyEXPInsta.playerRef = enemyFindPlayer.GetComponent<Player>();

    }

    //-------------------------------


    //Start function
    protected void enemyStart()
    {
        //Components
        enemyTransform = gameObject.transform;
        enemyRotation = enemyTransform.eulerAngles;
        enemyStartingPosition = enemyTransform.position;
        enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        enemyRigidBody = gameObject.GetComponent<Rigidbody2D>();
        enemyAnimator = gameObject.GetComponent<Animator>();
        enemyWidth = enemySpriteRenderer.bounds.extents.x;
        enemyHeight = enemySpriteRenderer.bounds.extents.y;
        enemyAudioSource = gameObject.GetComponent<AudioSource>();
        enemySounds = GameObject.Find("AudioManager").GetComponent<Sounds>();



        //States
        enemyCurrentState = enemyState.Patrolling;

        //Player
        if (enemyFindPlayer = GameObject.Find("Player"))
        {
            enemyPlayerPosition = enemyFindPlayer.GetComponent<Transform>().position;
        }
        else
        {
            Debug.Log("Player was not found");
        }
        enemyDistanceFromPlayer = enemyTransform.position - enemyPlayerPosition;
        enemyActive = false;
        enemyAttackTimer = 1.0f;
        enemyPlayerDetected = false;
        enemyTimerToCheckPlayerPos = 0.0f;

        //Line cast

        //Linecast start position is from the enemy's position to the left of it by the amount of its width and as high as its height
        enemyLineCastPos = enemyTransform.position - enemyTransform.right * enemyWidth + enemyTransform.up * enemyHeight;
        enemyIsGroundedCast = false;

        //Direction

        //Check which direction the enemy starts facing by checking is local scale
        if (enemyTransform.localScale.x < 0.0f)
        {
            enemyMovementDirection = 1;//Multiplied with velocity. 1 is facing right, -1 is facing left
        }
        else
        {
            enemyMovementDirection = -1;
        }

        enemyTimeToFlip = 1.5f;
        enemyDontFlipNow = true;

        //Getting hit
        enemyTakeDamageTime = 0.15f;
        enemyHasTakenDamage = false;

        //Effects
        //++Paralysis
        paralyzed = false;
        paralyzedTimer = 3.5f;

        //++Bleeding
        bleeding = false;
        bleedingTimer = 2.0f;

    }


}
