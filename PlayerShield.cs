using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public static int shieldCount = 0;
    [HideInInspector]
    public Player playerRef;

    private Vector2 shieldCenter;
    private Vector2 shieldPos;
    private SpriteRenderer shieldSprite;
    private bool summoned;
    private float shieldAngle;
    private float shieldRadius;
    [HideInInspector]
    public float direction;//Used to determine the direction of the rotation


    void Start()
    {
        shieldAngle = 0.0f;
        shieldRadius = 1.2f;
        shieldPos = gameObject.transform.position;
        shieldSprite = gameObject.GetComponent<SpriteRenderer>();
        shieldSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        summoned = true;
    }

    void Update()
    {
        //Rotate around the player
        shieldCenter = playerRef.gameObject.transform.position;
        shieldPos.x = shieldCenter.x + Mathf.Sin(shieldAngle) * shieldRadius * direction;
        shieldPos.y = shieldCenter.y + Mathf.Cos(shieldAngle) * shieldRadius * direction;
        gameObject.transform.position = shieldPos;
        shieldAngle += 0.06f;
        
        //Check used to increase the opacity of the shields. Needed to make the summoning of the shields look smoother
        if (summoned)
        {
            shieldSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            summoned = false;
        }
    }

    private void OnDestroy()
    {
        shieldCount--;
    }
}
