using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCamera : MonoBehaviour
{
    private GameObject cameraFindPlayer;
    private Camera cameraComponent;
    private float cameraStartingSize;
    private Vector2 cameraPlayerStartingPosition;
    private bool cameraStartMoving;
    private Vector3 cameraOffset = new Vector3(0.5f,0.0f,0.0f);
    private Vector3 cameraSmoothSpeed = new Vector3(0.125f,0.0f,0.0f);
    public bool cameraShake;
    private float cameraShakeTimer;
    public static bool cameraFollowingPlayer;

    // Start is called before the first frame update
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
        cameraShakeTimer = 0.01f;
        cameraShake = false;
    }

    private void Update()
    {
        if(cameraShake)
        {
            if(cameraShakeTimer>0.0f)
            {
                cameraComponent.orthographicSize = cameraStartingSize - 0.02f ;
                cameraShakeTimer -= Time.deltaTime;
            }
            else
            {
                cameraShake = false;
                cameraComponent.orthographicSize = cameraStartingSize;
                cameraShakeTimer = 0.05f;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraFollowingPlayer)
        {
            gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, new Vector3(cameraFindPlayer.gameObject.transform.position.x + cameraOffset.x,gameObject.transform.position.y,gameObject.transform.position.
                z),ref cameraSmoothSpeed, 0.5f);

        }

    }
}