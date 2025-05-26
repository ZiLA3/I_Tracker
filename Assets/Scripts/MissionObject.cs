using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected GameObject interactUI;
    [SerializeField] protected GameObject mission;
    [SerializeField] protected GameObject clearKey;

    public virtual void Interact()
    {
        if (interactUI.activeSelf) 
        {
            interactUI.SetActive(false);
        }
    }

    public void SetInteractUIActive(bool active)
    {
        interactUI?.SetActive(active);
    }

    public void ShowClearKey()
    {
        clearKey?.SetActive(true);
    }

    public void ActiveMissionUIActive(bool active)
    {
        mission?.SetActive(active);
    }

    public void ResetCamera() 
    {
        CameraManager.Instance.ToggleCamera(CameraType.mainCamera);
    }
}
