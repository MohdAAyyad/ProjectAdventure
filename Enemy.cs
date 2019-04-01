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

    //Movement
    protected Vector2 enemyStartingPosition;
    protected int enemyMovementDirection;
	protected float enemySpeed;
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
    public void enemyTakeDamage(int damage)
    {
        enemyHealth -= damage;
        enemyHPBar.fillAmount -= damage / enemyHealthMax;
        enemyHasTakenDamage = true;

        if(enemyHealth<=0.0f)
        {
            Destroy(gameObject);
        }

        //If the player attacks the enemy from the back, flip
        if(!enemyPlayerDetected)
        {
            enemyHorizontalFlip();
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
            enemyCurrentState = enemyState.Attacking;
            enemyRigidBody.velocity = new Vector2(0.0f, 0.0f);
            enemyAnimator.SetBool("enemyDetectedPlayer", true);
            enemyAnimator.SetBool("enemyAttackPlayer", true);
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
        Destroy(Instantiate(enemyDeathObject, gameObject.transform.position, gameObject.transform.rotation),1.0f);
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
        enemySpeed = 2.0f;
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

    }


}
