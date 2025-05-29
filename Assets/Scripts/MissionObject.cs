using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject : MonoBehaviour, IInteractable
{
    [Header("Mission Objects")]
    [SerializeField] protected GameObject interactUI;
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

        Player.Instance.Mission.SetMissionActive(true);
    }

    public void SetInteractUIActive(bool active)
    {
        interactUI?.SetActive(active);
    }

    public void ShowClearKey()
    {
        Player.Instance.Mission.PushMissionKey(clearKey);
    }

    public virtual void ActiveMissionObjectActive(bool active)
    {
        
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
        Player.Instance.Mission.AddInteractedObject(this);
    }
}
