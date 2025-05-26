using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MissionKey : MonoBehaviour
{
    public bool keyGetSuccessed = false;

    private void Update()
    {
        if (keyGetSuccessed) 
        {
            transform.gameObject.SetActive(false);
        }
    }
}
