using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected GameObject interactUI;
    [SerializeField] protected GameObject mission;
    [SerializeField] protected GameObject inMissionUI;
    [SerializeField] protected GameObject clearKey;

    public virtual void Interact()
    {
        if (interactUI.activeSelf) 
        {
            interactUI?.SetActive(false);
            inMissionUI?.SetActive(true);
        }

        MissionManager.Instance.SetMissionActive(true);
    }

    public void SetInteractUIActive(bool active)
    {
        interactUI?.SetActive(active);
    }

    public void ShowClearKey()
    {
        MissionManager.Instance.PushMissionKey(clearKey);
    }

    public void ActiveMissionUIActive(bool active)
    {
        mission?.SetActive(active);
    }

    public virtual void ResetToMainView() 
    {
        CameraManager.Instance.ToggleCamera(CameraType.mainCamera);
    }
}
