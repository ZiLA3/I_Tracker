using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Transform hand;


    void Start()
    {
        
    }

    void Update()
    {
        transform.position = hand.position + offset;
    }
}
