using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType
{
    mainCamera,
    doorCamera,
    enemyCamera,
    keypadCamera
}

public class CameraManager : MonoBehaviour
{ 
    public static CameraManager Instance { get; private set; }

    [SerializeField] GameObject mainCamUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera doorCamera;
    [SerializeField] Camera enemyCamera;
    [SerializeField] Camera keypadCamera;

    public CameraType CurrentCameraType { get; private set; } = CameraType.mainCamera;

    public void ToggleCamera(CameraType cameraType)
    {
        mainCamera.gameObject.SetActive(false);
        doorCamera.gameObject.SetActive(false);
        enemyCamera.gameObject.SetActive(false);
        keypadCamera.gameObject.SetActive(false);

        mainCamUI.SetActive(false);

        switch (cameraType)
        {
            case CameraType.mainCamera:
                CurrentCameraType = CameraType.mainCamera;
                mainCamera.gameObject.SetActive(true);
                mainCamUI.SetActive(true);
                break;
            case CameraType.doorCamera:
                CurrentCameraType = CameraType.doorCamera;
                doorCamera.gameObject.SetActive(true);
                break;
            case CameraType.enemyCamera:
                CurrentCameraType = CameraType.enemyCamera;
                enemyCamera.gameObject.SetActive(true);
                break;
            case CameraType.keypadCamera:
                CurrentCameraType = CameraType.keypadCamera;
                keypadCamera.gameObject.SetActive(true);
                break;
        }
    }
}
