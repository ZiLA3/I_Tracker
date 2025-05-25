using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera backCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCamera();
        }
    }

    private void ToggleCamera()
    {
        if (mainCamera.enabled)
        {
            mainCamera.enabled = false;
            backCamera.enabled = true;
        }
        else
        {
            mainCamera.enabled = true;
            backCamera.enabled = false;
        }
    }
}
