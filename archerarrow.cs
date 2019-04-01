using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class archerarrow : MonoBehaviour
{
    private Animator archerarrowAnimator;
    private Rigidbody2D archerarrowRB;
    private Collider2D archerarrowCollider;
    private float archerarrowDamage;
    // Start is called before the first frame update
    void Start()
    {
        archerarrowAnimator = gameObject.GetComponent<Animator>();
        archerarrowRB = gameObject.GetComponent<Rigidbody2D>();
        archerarrowCollider = gameObject.GetComponent<Collider2D>();
        archerarrowDamage = 7.0f;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            archerarrowRB.velocity = new Vector2(0.0f, 0.0f);
            archerarrowRB.gravityScale = 0.0f;
            archerarrowAnimator.SetTrigger("arrowExplosion");
            col.GetComponent<Player>().playerTakeDamage(archerarrowDamage);
            archerarrowCollider.enabled = false;
        }
        else
        {
            archerarrowRB.velocity = new Vector2(0.0f, 0.0f);
            archerarrowRB.gravityScale = 0.0f;
            archerarrowAnimator.SetTrigger("arrowExplosion");
            archerarrowCollider.enabled = false;
        }
    }

    private void archerArrowDestroySelf()
    {
        Destroy(gameObject);
    }
}
