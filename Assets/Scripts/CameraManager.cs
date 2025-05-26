using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType
{
    mainCamera,
    backCamera,
    enemyCamera,
    keypadCamera
}

public class CameraManager : MonoBehaviour
{ 
    public static CameraManager Instance { get; private set; }

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
    [SerializeField] Camera backCamera;
    [SerializeField] Camera enemyCamera;
    [SerializeField] Camera keypadCamera;

    public void ToggleCamera(CameraType cameraType)
    {
        mainCamera.gameObject.SetActive(false);
        backCamera.gameObject.SetActive(false);
        enemyCamera.gameObject.SetActive(false);
        keypadCamera.gameObject.SetActive(false);

        switch (cameraType)
        {
            case CameraType.mainCamera:
                mainCamera.gameObject.SetActive(true);
                break;
            case CameraType.backCamera:
                backCamera.gameObject.SetActive(true);
                break;
            case CameraType.enemyCamera:
                enemyCamera.gameObject.SetActive(true);
                break;
            case CameraType.keypadCamera:
                keypadCamera.gameObject.SetActive(true);
                break;
        }
    }
}
