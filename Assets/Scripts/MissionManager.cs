using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // Singleton instance for easy access
    public static MissionManager Instance;


    // interactable 오브젝트를 찾기 위한 변수
    public MissionObject currentInteractable { get; private set; }
    public HashSet<MissionObject> interactedObjects { get; private set; }

    // Mission Key 저장 Queue
    public Queue<GameObject> GameObjects { get; private set; }

    public bool IsInMission { get; private set; } = false; // 현재 미션 중인지 여부
    public int playeKeyCount { get; private set; } = 0;


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

    void Start()
    {
        interactedObjects = new HashSet<MissionObject>();
        GameObjects = new Queue<GameObject>();

        currentInteractable = null;
    }

    void Update()
    {
        
    }

    public void SetCurrentInteractable(MissionObject interactable)
    {
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

    public void PushMissionKey(GameObject GameObject) 
    {
        GameObject.SetActive(true);
        GameObjects.Enqueue(GameObject);
    }

    public void PopMissionKey()
    {
        if (GameObjects.Count > 0)
        {
            playeKeyCount++;
            GameObject key = GameObjects.Dequeue();
            key.SetActive(false);
        }
    }

    public void SetMissionActive(bool active)
    {
        IsInMission = active;
    }
}
