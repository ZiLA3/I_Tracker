using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Interact()
    {
        // Interact with the object
        Debug.Log("Interacted with " + gameObject.name);
    }
}
