using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{

    public GameObject flames;
    public int fireRateStart;
    public float timeInBetweenStart;
    private int fireRate;
    private float timeInBetween;
    private float timeBetweenFireRate;
    private bool flameThrowing;
    private Vector2 flamePos;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        flamePos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f);
        timeBetweenFireRate = 0.5f;
        flameThrowing = true;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            //Check if we're still firing
                if(fireRate>0)
                {
                //Give a mini breather between each flame
                    if(timeBetweenFireRate>0.0f)
                    {
                        timeBetweenFireRate -= Time.deltaTime;
                    }
                    else
                    {
                    //Create the flames and decrease the fireRate
                       Instantiate(flames, flamePos, gameObject.transform.rotation);
                       timeBetweenFireRate = 0.5f;
                       fireRate--;
                    }
                }
                else
                {
                //If we've fired all we have, go into reloading
                    if(timeInBetween>0.0f)
                    {
                        timeInBetween -= Time.deltaTime;
                    }
                    else
                    {
                        fireRate = fireRateStart;
                        timeInBetween = timeInBetweenStart;
                        timeBetweenFireRate = 0.0f;
                    }
                }
        }
    }

    private void OnBecameVisible()
    {
        active = true;
    }

    private void OnBecameInvisible()
    {
        active = false;
    }
}
