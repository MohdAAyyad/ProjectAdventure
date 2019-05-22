using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCamera : MonoBehaviour
{
    private GameObject cameraFindPlayer;
    private Camera cameraComponent;
    private float cameraSpeed;
    private float cameraInterpolation;
    private float cameraStartingSize;
    private Vector2 cameraPlayerStartingPosition;
    private bool cameraStartMoving;
    private Vector3 cameraOffset = new Vector3(1.0f,0.5f,0.0f);
    private Vector3 cameraSmoothSpeed = new Vector3(0.125f,0.0f,0.0f);
    public bool cameraShake;
    private float cameraShakeTimer;
    private Vector3 cameraOriginalPos;
    private float cameraShakeMaxRight;
    private float cameraShakeMaxLeft;
    private float cameraShakeMaxUp;
    private float cameraShakeMaxDown;
    private bool cameraChangingSize;
    private float cameraNewSize;
    public static bool cameraFollowingPlayer;


    void Start()
    {
        if (cameraFindPlayer = GameObject.Find("Player"))
        {
            
            //gameObject.transform.position = new Vector3(cameraFindPlayer.transform.position.x , cameraFindPlayer.transform.position.y +0.5f, gameObject.transform.position.z);
            cameraPlayerStartingPosition = gameObject.transform.position;
        }
        else
        {
            Debug.Log("Camera could not find player");
        }
        cameraComponent = gameObject.GetComponent<Camera>();
        cameraStartingSize = cameraComponent.orthographicSize;
        cameraFollowingPlayer = true;
        cameraStartMoving = false;
        cameraShakeTimer = 0.2f;
        cameraShake = false;
        cameraSpeed = 2.0f;
        cameraInterpolation = cameraSpeed * Time.deltaTime;
        cameraChangingSize = false;
        cameraNewSize = cameraComponent.orthographicSize;
    }

    private void Update()
    {
        if(cameraShake)
        {
            if(cameraShakeTimer>0.0f)
            {
                //cameraComponent.orthographicSize = cameraStartingSize - 0.02f ;
                cameraOriginalPos = gameObject.transform.position;
                cameraShakeMaxRight = gameObject.transform.position.x + 0.05f;
                cameraShakeMaxLeft = gameObject.transform.position.x - 0.05f;
                cameraShakeMaxUp = gameObject.transform.position.y + 0.04f;
                cameraShakeMaxDown = gameObject.transform.position.y - 0.04f;
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, Random.Range(cameraShakeMaxDown, cameraShakeMaxUp), gameObject.transform.position.z);
                cameraShakeTimer -= Time.deltaTime;
            }
            else
            {
                cameraShake = false;
                gameObject.transform.position = cameraOriginalPos;
                //cameraComponent.orthographicSize = cameraStartingSize;
                cameraShakeTimer = 0.2f;
            }
        }

        if(cameraChangingSize)
        {
            if(cameraComponent.orthographicSize<cameraNewSize)
            {
                cameraComponent.orthographicSize += 0.1f;

                if(cameraComponent.orthographicSize >= cameraNewSize)
                {
                    cameraChangingSize = false;
                }
            }
            else if(cameraComponent.orthographicSize > cameraNewSize)
            {
                cameraComponent.orthographicSize -= 0.1f;

                if (cameraComponent.orthographicSize <= cameraNewSize)
                {
                    cameraChangingSize = false;
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraFollowingPlayer)
        {
            gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, new Vector3(cameraFindPlayer.gameObject.transform.position.x + cameraOffset.x, cameraFindPlayer.gameObject.transform.position.y, gameObject.transform.position.
             z), ref cameraSmoothSpeed, 0.5f);          
                        
        }

    }

    public void cameraChangeSize(float newSize)
    {
        cameraNewSize = newSize;
        cameraChangingSize = true;
    }
}