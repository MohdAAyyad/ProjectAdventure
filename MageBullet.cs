using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBullet : MonoBehaviour
{
    private GameObject magebulletFindPlayer;
    private Vector2 magebulletDirection;
    private Rigidbody2D magebulletRigidBody;
    private Animator magebulletAnimator;
    private Collider2D magebulletCollider;
    private float magebulletTimer;
    private float magebulletRotationAmount;
    private float magebulletRotationSpeed;
    private bool magebulletActive;
    private float magebulletDamage;

    // Start is called before the first frame update
    void Start()
    {
        if (magebulletFindPlayer = GameObject.Find("Player"))
        {
            magebulletDirection = magebulletFindPlayer.transform.position - gameObject.transform.position;
        }
        else
        {
            Debug.Log("mage bullet could not find the player");
        }
        magebulletRigidBody = gameObject.GetComponent<Rigidbody2D>();
        magebulletAnimator = gameObject.GetComponent<Animator>();
        magebulletCollider = gameObject.GetComponent<Collider2D>();
        magebulletRotationAmount = 0.0f;
        magebulletRotationSpeed = 200.0f;
        magebulletDamage = 7.0f;
        magebulletTimer = 3.0f;
        magebulletActive = true;
    }

    void FixedUpdate()
    {

        if (magebulletActive)
        {
            //Update the MB direction based on the player's position
            magebulletDirection = magebulletFindPlayer.transform.position - gameObject.transform.position;
            magebulletDirection.Normalize();

            if (gameObject.transform.localScale.x > 0.0f)
            {
                //Update the angular velocity
                magebulletRotationAmount = Vector3.Cross(magebulletDirection, -transform.right).z;

                magebulletRigidBody.angularVelocity = -magebulletRotationAmount * magebulletRotationSpeed;
                magebulletRigidBody.velocity = transform.right * -3.0f;
            }
            else
            {
                magebulletRotationAmount = Vector3.Cross(magebulletDirection, transform.right).z;
                magebulletRigidBody.angularVelocity = -magebulletRotationAmount * magebulletRotationSpeed;
                magebulletRigidBody.velocity = transform.right * 3.0f;
            }
        }
    }

    private void Update()
    {
        if(magebulletTimer>0.0f)
        {
            magebulletTimer -= Time.deltaTime;
        }
        else
        {
            magebulletActive = false;
            magebulletRigidBody.velocity = new Vector2(0.0f, 0.0f);
            magebulletAnimator.SetTrigger("magebulletExplode");
            magebulletCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            magebulletActive = false;
            magebulletRigidBody.velocity = new Vector2(0.0f, 0.0f);
            col.GetComponent<Player>().playerTakeDamage(magebulletDamage);
            magebulletAnimator.SetTrigger("magebulletExplode");
            magebulletCollider.enabled = false;
        }
        else
        {
            magebulletActive = false;
            magebulletRigidBody.velocity = new Vector2(0.0f, 0.0f);
            magebulletAnimator.SetTrigger("magebulletExplode");
            magebulletCollider.enabled = false;
        }
    }

    private void magebulletDestroySelf()
    {
        Destroy(gameObject);
    }

}
