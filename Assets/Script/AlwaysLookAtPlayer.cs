using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookAtPlayer : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player);
            transform.Rotate(0, 180, 0); // Adjust for correct facing
        }
    }
}
