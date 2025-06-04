using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // interactable 오브젝트를 찾기 위한 변수
    public MissionObject currentInteractable { get; private set; }
    public HashSet<MissionObject> interactedObjects { get; private set; }


    public bool IsInMission { get; private set; } = false; // 현재 미션 중인지 여부
    public bool LightTwinkling { get; set; } = false; // 현재 불빛 깜빡임 여부
    public int PlayerKeyCount { get; private set; } = 0;

    void Start()
    {
        interactedObjects = new HashSet<MissionObject>();

        currentInteractable = null;
    }

    void Update()
    {

    }

    public void SetCurrentInteractable(MissionObject interactable)
    {
        if (currentInteractable != null)
            currentInteractable.SetInteractUIActive(false);

        currentInteractable = interactable;
    }

    public void AddInteractedObject(MissionObject interactable)
    {
        if (interactable != null)
        {
            interactedObjects.Add(interactable);
        }
    }
    public bool IsInteracted(MissionObject interactable)
    {
        return interactedObjects.Contains(interactable);
    }

    public void SetMissionActive(bool active)
    {
        IsInMission = active;
    }

    public void IncreaseKeyCount() => PlayerKeyCount++; // 플레이어 키 개수 증가
}
