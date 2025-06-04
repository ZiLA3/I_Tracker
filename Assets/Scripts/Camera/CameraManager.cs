using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType
{
    mainCamera,
    doorCamera,
    rspCamera,
    leverCamera,
    keypadCamera,
    blackKeyCamera,
    redKeyCamera,
    blueKeyCamera
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
    [SerializeField] Camera rspCamera;
    [SerializeField] Camera leverCamera;
    [SerializeField] Camera keypadCamera;
    [SerializeField] Camera blackKeyCamera;
    [SerializeField] Camera redKeyCamera;
    [SerializeField] Camera blueKeyCamera;

    public CameraType CurrentCameraType { get; private set; } = CameraType.mainCamera;

    public void ToggleCamera(CameraType cameraType)
    {
        mainCamera.gameObject.SetActive(false);
        doorCamera.gameObject.SetActive(false);
        rspCamera.gameObject.SetActive(false);
        keypadCamera.gameObject.SetActive(false);
        leverCamera.gameObject.SetActive(false);
        blackKeyCamera.gameObject.SetActive(false);
        redKeyCamera.gameObject.SetActive(false);
        blueKeyCamera.gameObject.SetActive(false);

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
            case CameraType.rspCamera:
                CurrentCameraType = CameraType.rspCamera;
                rspCamera.gameObject.SetActive(true);
                break;
            case CameraType.keypadCamera:
                CurrentCameraType = CameraType.keypadCamera;
                keypadCamera.gameObject.SetActive(true);
                break;
            case CameraType.leverCamera:
                CurrentCameraType = CameraType.leverCamera;
                leverCamera.gameObject.SetActive(true);
                break;
            case CameraType.blackKeyCamera:
                CurrentCameraType = CameraType.blackKeyCamera;
                blackKeyCamera.gameObject.SetActive(true);
                break;
            case CameraType.redKeyCamera:
                CurrentCameraType = CameraType.redKeyCamera;
                redKeyCamera.gameObject.SetActive(true);
                break;
            case CameraType.blueKeyCamera:
                CurrentCameraType = CameraType.blueKeyCamera;
                blueKeyCamera.gameObject.SetActive(true);
                break;
        }
    }
}
