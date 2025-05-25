using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // Default interaction logic for mission objects
        Debug.Log("Interacting with mission object: " + gameObject.name);
    }
}
