using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEmitter : MonoBehaviour
{
    private GameObject findPlayer;
    // Start is called before the first frame update
    void Start()
    {
        findPlayer = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.position = findPlayer.transform.position;   
    }
}
