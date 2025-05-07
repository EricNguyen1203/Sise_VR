using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    void DestroyScriptInstance()
    {
        // Removes this script instance from the game object
        Destroy(this);
    }

    void DestroyComponent()
    {
        // Removes the rigidbody from the game object
        Destroy(GetComponent<Rigidbody>());
    }

    void DestroyObjectDelayed()
    {
        // Kills the game object in 5 seconds after loading the object
        Destroy(gameObject, 5);
    }

    // When the user presses Ctrl, it will remove the
    // BoxCollider component from the game object
    void Update()
    {
        if (Input.GetButton("Fire1") && GetComponent<BoxCollider>())
        {
            Destroy(GetComponent<BoxCollider>());
        }
    }
}
