using UnityEngine;

public class MissionObject : MonoBehaviour, IInteractable
{
    [Header("Mission Objects")]
    [SerializeField] protected GameObject interactUI;
    [SerializeField] protected GameObject hand;
    [SerializeField] protected GameObject inMissionUI;
    [SerializeField] protected GameObject clearKey;

    public virtual void Interact()
    {
        if (interactUI.activeSelf)
        {
            if (interactUI != null)
                interactUI.SetActive(false);

            if (inMissionUI != null)
                inMissionUI.SetActive(true);
        }

        Player.Instance.Mission.SetMissionActive(true);
    }

    public void SetInteractUIActive(bool active)
    {
        interactUI?.SetActive(active);
    }

    public void ShowClearKey()
    {
        if (clearKey != null)
            clearKey.SetActive(true);
    }

    public virtual void SetHandActive(bool active)
    {
        if (hand != null)
            hand.SetActive(active);
    }

    public virtual void ResetToMainView()
    {
        CameraManager.Instance.ToggleCamera(CameraType.mainCamera);
    }

    public virtual void SucceedMission()
    {
        ShowClearKey();
        ResetToMainView();

        Player.Instance.Mission.AddInteractedObject(this);
    }
}
