using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Transform eye;
    [SerializeField] Vector3 cameraOffset;

    private void LateUpdate()
    {
        transform.position = eye.position + cameraOffset;
    }
}
