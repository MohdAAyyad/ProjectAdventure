using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    private Player playerRef;
    private float playerDirection;

    //Shields
    public PlayerShield shield;
    private PlayerShield shieldInsta;
    private PlayerShield shieldInsta2;
    private float shieldCooldown;
    [HideInInspector]
    public bool shieldSummoned; //Used by the player script to know if shields were summoned
    public bool shieldActive;
    private Vector2 shieldPos;
    public Image shieldIcon;

    //Abilities manager
    [HideInInspector]
    public int ID1;//Sword abilities
    [HideInInspector]
    public int ID2;//Bow abilities
    [HideInInspector]
    public bool ability1Used = false; // thia represents RT  sword abillity 
    [HideInInspector]
    public bool ability1OnCooldown;
    [HideInInspector]
    public AudioClip ability1Sound;
    [HideInInspector]
    public bool ability2Used;
    [HideInInspector]
    public bool ability2OnCooldown;
    [HideInInspector]
    public AudioClip ability2Sound;
    [HideInInspector]
    public GameObject ability1; //Currently equipped ability1
    [HideInInspector]
    public GameObject ability2; //Currently equipped ability2

    public Image abl1Icon;
    public Image abl2Icon;

    //Change Abilities
    private float DPADX;
    private bool ability1Change;
    private bool ability2Change;


    //---Ability 1----//

    //Common
    private bool animatorCommenceAbility;
    [HideInInspector]
    public bool abilityHasEnded;

    //Down Thrust Explosion (DTE)
    private float DTECooldown;
    private bool DTEUsed;
    public GameObject DTEobject;
    public Sprite DTEicon;

    //Sword Burst (SB)
    private float SBCooldown;
    private float SBfirerate;
    private Vector2 SBforce;
    private bool SBUsed;
    private GameObject SBobjectInsta;
    private Vector2 SBobjectPos;
    public GameObject SBobject;
    public Sprite SBicon;

    //Fade Attack (FA)
    private bool FAused;
    private bool colorHasFaded;
    private Color playerColor;
    private float FAcooldown;
    private float movementTimer;
    private Vector2 movementVector;
    private int FAdirection;
    private int FAdamage;
    private Collider2D[] enemiesToDamage;
    private Vector2 hitBoxSize;
    private Vector2 hitBoxCenter;
    private Vector2 playerInitialPos;
    public GameObject FAtrailObject;
    private GameObject FAobjectInsta;
    public GameObject FAeffect;
    public Sprite FAicon;


    //---Ability 2----//
    //Electric Arrow
    private float EACooldown;
    private bool EAUsed;
    public GameObject EAobject;
    public Sprite EAicon;

    //Bomb Arrow
    private float BACooldown;
    private bool BAUsed;
    public GameObject BAobject;
    public Sprite BAicon;

    //Piercing Arrow
    private float PACooldown;
    private bool PAUsed;
    public GameObject PAobject;
    public Sprite PAicon;

    private void Awake()
    {
        playerRef = gameObject.GetComponent<Player>();
        
    }

    void Start()
    {

        //Common
        animatorCommenceAbility = false;
        abilityHasEnded = false;


        ID1 = 1; //Placeholder. Player starts with DTE
        ability1 = DTEobject;
        ability1OnCooldown = false;
        ID2 = 1; //Placeholder. Player starts with EA
        //ability2Sound = playerRef.playerSound.EA;
        ability2OnCooldown = false;

        shieldCooldown = 0.0f;
        shieldIcon.fillAmount = 1.0f;
        PlayerShield.shieldCount = 0;
        shieldSummoned = false;
        shieldActive = false;
        shieldPos = new Vector2(gameObject.transform.position.x + 1.2f, gameObject.transform.position.y);

        //DTE
        DTECooldown = 0.0f;

        //SB
        SBCooldown = 0.0f;
        SBforce = new Vector2(500.0f, 0.0f);
        SBfirerate = 0.0f;
        playerDirection = playerRef.playerDirection;

        //FA
        FAused = false;
        colorHasFaded = false;
        movementTimer = 0.08f;
        hitBoxSize = new Vector2(0.0f, 0.0f);
        FAdirection = 1;
        FAdamage = 20;
        

        //Ability Change
        DPADX = 0.0f;
        ability1Change = false;
        ability2Change = false;

        //UI
        abl1Icon.fillAmount = 1.0f;
        abl1Icon.sprite = DTEicon;

        abl2Icon.fillAmount = 1.0f;
        abl2Icon.sprite = EAicon;
    }

    void Update()
    {


        //Check which ability is called
        //Manage cooldowns

        if (ability1Used)
        {
            switch (ID1)
            {
                //DTE
                case 1:
                    DTEUsed = true;
                    DTECooldown = 10.0f;
                    ability1Used = false;
                    ability1OnCooldown = true;
                    break;
                //SB
                case 2:
                    SBUsed = true;
                    SBCooldown = 10.0f;
                    SBfirerate = 0.0f; //Used for summoning swords in bursts. Starts at zero to summon a sword immediately
                    ability1Used = false;
                    ability1OnCooldown = true;
                    break;
                //FA
                case 3:
                    FAcooldown = 10.0f;
                    FAused = true;
                    colorHasFaded = false;
                    playerColor = playerRef.playerSpriteRenderer.color;
                    movementTimer = 0.08f;
                    FAdirection = playerRef.playerDirection;
                    movementVector = new Vector2(100.0f * FAdirection, 0.0f);
                    playerInitialPos = gameObject.transform.position;
                    hitBoxCenter = new Vector2(gameObject.transform.position.x + 3.5f * FAdirection, gameObject.transform.position.y);
                    FAobjectInsta = Instantiate(FAtrailObject, gameObject.transform.position, gameObject.transform.rotation);
                    FAobjectInsta.transform.parent = gameObject.transform;
                    ability1Used = false;
                    ability1OnCooldown = true;

                    playerRef.playerAnimator.SetBool("playerAblEnd", false);
                    playerRef.playerAnimator.SetInteger("playerAblID", 3);
                    playerRef.playerIFrames = true;

                    break;
            }
        }

        else if (ability2Used)
        {
            switch (ID2)
            {
                //EA
                case 1:
                    EAUsed = true;
                    ability2Used = false;
                    EACooldown = 10.0f;
                    ability2OnCooldown = true;
                    ability2Sound = playerRef.playerSound.EA; //Sound for EA
                    break;
                //BA  
                case 2:
                    BAUsed = true;
                    ability2Used = false;
                    BACooldown = 10.0f;
                    ability2OnCooldown = true;
                    ability2Sound = playerRef.playerSound.BAbowSound; //Placeholder. Sound for EA
                    break;
                //PA  
                case 3:
                    PAUsed = true;
                    ability2Used = false;
                    PACooldown = 10.0f;
                    ability2OnCooldown = true;
                    ability2Sound = playerRef.playerSound.BAbowSound; //Placeholder. Sound for EA
                    break;
            }
        }

        

        //Check if shields are up
        shieldStatusCheckUp(Input.GetButtonDown("CTRL - L3"));

        //------------Ability 1------------//

        //Down Thrust Explosion (DTE)

        playerDTE(DTEUsed);
        playerSB(SBUsed);
        playerFA(FAused);


        //------------Ability 2------------//

        //Electric Arrow (EA)

        playerEA(EAUsed);
        playerBA(BAUsed);
        playerPA(PAUsed);


        //--------------------------------------------------Change Abilities-----------------------------------------------//

        DPADX = Input.GetAxisRaw("CTRL - DPADX");

        //Change ability 1
        if (DPADX>0.0f && !ability1Change)
        {
            playerChangeAbl(1,ID1);
            ability1Change = true;
        }
        //Change ability 2
        else if(DPADX<0.0f && !ability2Change)
        {
            playerChangeAbl(2, ID2);
            ability2Change = true;
        }
        else if(DPADX == 0.0f) //Only allow the player to change abilities again when they let go of the DPAD Axis
        {
            ability1Change = false;
            ability2Change = false;
        }

    }

    //Change the abilities of the player
    private void playerChangeAbl(int ability, int ablID)
    {
        ablID++; //Increase ablID to change the ability

        //ablID must be less or equal to 3
        if(ablID>3)
        {
            ablID = 1;
        }

        //Sword ability
        if (ability == 1)
        {
            switch (ablID)
            {
                case 1:
                    ID1 = 1;
                    ability1 = DTEobject;
                    ability1Sound = playerRef.playerSound.DTE;
                    abl1Icon.sprite = DTEicon;
                    break;
                case 2:
                    ID1 = 2;
                    ability1 = SBobject;
                    ability1Sound = playerRef.playerSound.DTE;
                    abl1Icon.sprite = SBicon;
                    break;
                case 3: 
                    ID1 = 3;
                    ability1 = FAtrailObject;
                    ability1Sound = playerRef.playerSound.DTE;
                    abl1Icon.sprite = FAicon;
                    break;
            }
        }
        //Bow ability
        else if (ability == 2)
        {
            switch (ablID)
            {
                case 1:
                    ID2 = ablID;
                    ability2 = EAobject;
                    ability2Sound = playerRef.playerSound.EA;
                    abl2Icon.sprite = EAicon;
                    break;
                case 2:
                    ID2 = ablID;
                    ability2 = BAobject;
                    ability2Sound = playerRef.playerSound.BAbowSound;
                    abl2Icon.sprite = BAicon;
                    break;
                case 3:
                    ID2 = ablID;
                    ability2 = PAobject;
                    ability2Sound = playerRef.playerSound.EA;
                    abl2Icon.sprite = PAicon;
                    break;

            }
        }
    }

    //--------------------------------------------------Sword Abilities-----------------------------------------------//

    //---------------DTE-----------//
    private void playerDTE(bool dteActive)
    {
        if (dteActive)
        {
            if (DTECooldown > 0.0f)
            {
                DTECooldown -= Time.deltaTime;
                abl1Icon.fillAmount = -0.1f * DTECooldown + 1.0f; //linear mapping between time and fill amount

            }
            else
            {
                abl1Icon.fillAmount = 1.0f;
                DTECooldown = 0.0f;
                DTEUsed = false;
                ability1OnCooldown = false;
            }
        }
    }

    //---------------SB-----------//

    private void playerSB(bool sbActive)
    {
        if(sbActive)
        {
            if(SBCooldown>0.0f)
            {
                SBfireSwords();
                SBCooldown -= Time.deltaTime;
                abl1Icon.fillAmount = -0.1f * SBCooldown + 1.0f;
            }
            else
            {
                abl1Icon.fillAmount = 1.0f;
                SBCooldown = 10.0f;
                SBUsed = false;
                ability1OnCooldown = false;
            }
        }
    }

    private void SBfireSwords()
    {
        if (SBfirerate > 0.0f)
        {
            SBfirerate -= Time.deltaTime;
        }
        else
        {
            playerDirection = playerRef.playerDirection; //Update direction
            SBobjectPos = new Vector2(gameObject.transform.position.x - 1.5f * playerDirection, gameObject.transform.position.y);
            SBobjectInsta = Instantiate(SBobject, SBobjectPos, gameObject.transform.rotation);
            SBobjectInsta.transform.localScale = new Vector3(SBobjectInsta.transform.localScale.x * playerDirection, SBobjectInsta.transform.localScale.y, SBobjectInsta.transform.localScale.z); //Flip the sword if needed
            if(SBforce.x >0.0f && playerDirection<0.0f)
            {
                SBforce = new Vector2(SBforce.x * playerDirection, 0.0f);
            }
            else if((SBforce.x < 0.0f && playerDirection > 0.0f))
            {
                SBforce = new Vector2(SBforce.x * -1.0f, 0.0f);
            }

            SBobjectInsta.GetComponent<Rigidbody2D>().AddForce(SBforce);
            SBfirerate = 1.0f;
        }
    }

    //---------------FA-----------//

    private void playerFA(bool FAactive)
    {
        //Manage cooldown and call FAattack which passes in a boolean that's flipped by the animator
        if(FAactive)
        {
            if(FAcooldown>0.0f)
            {
                FAattack(animatorCommenceAbility);
                FAcooldown -= Time.deltaTime;
                abl1Icon.fillAmount = -0.1f * FAcooldown + 1.0f;
            }
            else
            {
                abl1Icon.fillAmount = 1.0f;
                FAcooldown = 10.0f;
                FAused = false;
                ability1OnCooldown = false;
            }
        }
    }

    private void FAattack(bool commenceAttack)
    {
        //If the animation has started, proceed
        if(commenceAttack)
        {
            //Fade out the player's sprite
            if(playerRef.playerSpriteRenderer.color.a >0.0f && !colorHasFaded)
            {
                playerColor.a -= 0.1f;
                playerRef.playerSpriteRenderer.color = playerColor;
            }
            else
            {
                //Once the sprite has completely faded, move the player 
                if (movementTimer > 0.0f)
                {
                    if(movementTimer>=0.08f) //Player the sound only once
                    {
                        playerRef.playerAudioSource.PlayOneShot(playerRef.playerSound.FAactivated);
                    }
                    movementTimer -= Time.deltaTime;
                    playerRef.playerRigidBody.velocity = movementVector;
                   

                    if(movementTimer<=0.0f)
                    {
                        //Once the timer stops, calcualte the hitbox and stop the player
                        colorHasFaded = true;
                        playerRef.playerRigidBody.velocity = new Vector2(0.0f, 0.0f); //Trying to minimize the amount of new vectors being created
                        hitBoxSize = new Vector2(Mathf.Abs(gameObject.transform.position.x - playerInitialPos.x), 1.0f);

                    }
                }
                else if(colorHasFaded)
                {
                   //Fade in the player at the end of the movement
                    playerColor.a += 0.1f;
                    playerRef.playerSpriteRenderer.color = playerColor;
                    if(playerRef.playerSpriteRenderer.color.a >=1.0f)
                    {
                        //Once the player has faded in, damage the enemies
                        colorHasFaded = false;
                        enemiesToDamage = Physics2D.OverlapBoxAll(hitBoxCenter, hitBoxSize, 0.0f, playerRef.playerDamage);
                        for(int i = 0; i<enemiesToDamage.Length;i++)
                        {
                            enemiesToDamage[i].gameObject.GetComponent<Enemy>().enemyTakeDamageAbility("PA", FAdamage, null);
                            Destroy(Instantiate(FAeffect, enemiesToDamage[i].gameObject.transform.position, FAeffect.gameObject.transform.rotation), 0.5f);
                        }
                        //End FA
                        //Destroy the trail object and disalbe the iFrames
                        Destroy(FAobjectInsta);
                        playerRef.playerIFrames = false;

                        abilityEnd();
                    }
                }
            }
        }
    }
    

    //--------------------------------------------------BOW Abilities-----------------------------------------------//

    //---------------EA-----------//

    private void playerEA(bool eaActive)
    {
        if(eaActive)
        {
            if (EACooldown > 0.0f)
            {
                EACooldown -= Time.deltaTime;
                abl2Icon.fillAmount = -0.1f * EACooldown + 1.0f; //linear mapping between time and fill amount

            }
            else
            {
                abl2Icon.fillAmount = 1.0f;
                EACooldown = 0.0f;
                EAUsed = false;
                ability2OnCooldown = false;
            }
        }
    }

    //---------------BA-----------//

    private void playerBA(bool baActive)
    {
        if (baActive)
        {
            if (BACooldown > 0.0f)
            {
                BACooldown -= Time.deltaTime;
                abl2Icon.fillAmount = -0.1f * BACooldown + 1.0f; //linear mapping between time and fill amount

            }
            else
            {
                abl2Icon.fillAmount = 1.0f;
                BACooldown = 0.0f;
                BAUsed = false;
                ability2OnCooldown = false;
            }
        }
    }

    //---------------PA-----------//

    private void playerPA(bool paActive)
    {
        if (paActive)
        {
            if (PACooldown > 0.0f)
            {
                PACooldown -= Time.deltaTime;
                abl2Icon.fillAmount = -0.1f * PACooldown + 1.0f; //linear mapping between time and fill amount

            }
            else
            {
                abl2Icon.fillAmount = 1.0f;
                PACooldown = 0.0f;
                PAUsed = false;
                ability2OnCooldown = false;
            }
        }
    }


    //--------------------------------------------------Animator----------------------------------------------------//

    //The animation has started
    private void flipCommenceAttack()
    {
        animatorCommenceAbility = true;
    }
    //The ability has ended
    private void abilityEnd()
    {
        //Leave the player ability state
        animatorCommenceAbility = false;
        abilityHasEnded = true;
        //Go back to Idle animation state
        playerRef.playerAnimator.SetBool("playerAblEnd", true);
        playerRef.playerAnimator.SetInteger("playerAblID", 0);
        Debug.Log("Ability end");
    }

    //--------------------------------------------------Shields----------------------------------------------------//

    //Called by the player. Shields will be destroyed to avoid damage
    public void DestroyShield()
    {
        if (shieldInsta)
        {
            Destroy(shieldInsta.gameObject);
        }
        else if (shieldInsta2)
        {
            Destroy(shieldInsta2.gameObject);
        }

        if(PlayerShield.shieldCount<=1)
        {
            shieldActive = false;
        }
    }

    private void shieldStatusCheckUp(bool userInput)
    {
        if (userInput)
        {
            if (shieldCooldown <= 0.0f)
            {
                playerRef.playerAudioSource.PlayOneShot(playerRef.playerSound.Shield);
                //If the player has no active shields, summon them and start the cooldown
                if (PlayerShield.shieldCount == 0 && !shieldSummoned)
                {
                    //Summon one shield to the right of the player and another to left of the player
                    shieldPos = new Vector2(gameObject.transform.position.x + 1.2f, gameObject.transform.position.y);
                    shieldInsta = Instantiate(shield, shieldPos, gameObject.transform.rotation);
                    shieldInsta.playerRef = playerRef;
                    shieldInsta.direction = -1.0f;

                    shieldPos = new Vector2(gameObject.transform.position.x - 1.2f, gameObject.transform.position.y);
                    shieldInsta2 = Instantiate(shield, shieldPos, gameObject.transform.rotation);
                    shieldInsta2.playerRef = playerRef;
                    shieldInsta2.direction = 1.0f;

                    //Start shield cooldown
                    PlayerShield.shieldCount += 2;
                    shieldSummoned = true;
                    shieldActive = true;
                    shieldCooldown = 10.0f;
                    shieldIcon.fillAmount = 0.0f;

                }
                //If the cooldown is over and the player has shields summoned, destroy the existing ones and then create two new ones
                else if (PlayerShield.shieldCount > 0 && !shieldSummoned)
                {
                    if (shieldInsta)
                    {
                        Destroy(shieldInsta.gameObject);
                    }
                    if (shieldInsta2)
                    {
                        Destroy(shieldInsta2.gameObject);
                    }

                    shieldPos = new Vector2(gameObject.transform.position.x + 1.2f, gameObject.transform.position.y);
                    shieldInsta = Instantiate(shield, shieldPos, gameObject.transform.rotation);
                    shieldInsta.playerRef = playerRef;
                    shieldInsta.direction = -1.0f;

                    shieldPos = new Vector2(gameObject.transform.position.x - 1.2f, gameObject.transform.position.y);
                    shieldInsta2 = Instantiate(shield, shieldPos, gameObject.transform.rotation);
                    shieldInsta2.playerRef = playerRef;
                    shieldInsta2.direction = 1.0f;

                    PlayerShield.shieldCount += 2;
                    shieldSummoned = true;
                    shieldActive = true;
                    shieldCooldown = 10.0f;
                    shieldIcon.fillAmount = 0.0f;
                }
            }
        }

        //Shields
        //If shields were summoned, start the cooldown
        if (shieldSummoned)
        {
            if (shieldCooldown > 0.0f)
            {
                shieldCooldown -= Time.deltaTime;
                shieldIcon.fillAmount = -0.1f * shieldCooldown + 1.0f; //linear mapping between time and fill amount

            }
            else
            {
                shieldIcon.fillAmount = 1.0f;
                shieldCooldown = 0.0f;
                shieldSummoned = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(hitBoxCenter, hitBoxSize);
}

}
