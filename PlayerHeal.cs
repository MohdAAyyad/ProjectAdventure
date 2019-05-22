using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    public Player playerRef;

    private void healPlayer()
    {
        playerRef.playerHealNow = true;
        Destroy(gameObject);
    }
}
