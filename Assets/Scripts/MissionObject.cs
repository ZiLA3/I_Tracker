using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject : MonoBehaviour, IInteractable
{
    [Header("Mission Objects")]
    [SerializeField] protected GameObject interactUI;
    [SerializeField] protected GameObject[] mission;
    [SerializeField] protected GameObject inMissionUI;
    [SerializeField] protected GameObject clearKey;

    public virtual void Interact()
    {
        if (interactUI.activeSelf) 
        {
            interactUI?.SetActive(false);
            inMissionUI?.SetActive(true);
            ActiveMissionObjectActive(true);
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

    public void ActiveMissionObjectActive(bool active)
    {
        foreach (GameObject obj in mission)
        {
            obj?.SetActive(active);
        }
    }

    public virtual void ResetToMainView() 
    {
        CameraManager.Instance.ToggleCamera(CameraType.mainCamera);
    }

    public virtual void SucceedMission() 
    {
        ShowClearKey();
        ResetToMainView();
        ActiveMissionObjectActive(false);
        MissionManager.Instance.AddInteractedObject(this);
    }
}
