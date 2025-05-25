using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType
{
    MainCamera,
    BackCamera,
    EnemyCamera
}

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera backCamera;
    [SerializeField] Camera enemyCamera;

    private void Update()
    {
        
    }

    public void ToggleCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.MainCamera:
                mainCamera.gameObject.SetActive(true);
                backCamera.gameObject.SetActive(false);
                enemyCamera.gameObject.SetActive(false);
                break;
            case CameraType.BackCamera:
                mainCamera.gameObject.SetActive(false);
                backCamera.gameObject.SetActive(true);
                enemyCamera.gameObject.SetActive(false);
                break;
            case CameraType.EnemyCamera:
                mainCamera.gameObject.SetActive(false);
                backCamera.gameObject.SetActive(false);
                enemyCamera.gameObject.SetActive(true);
                break;
        }
    }
}
