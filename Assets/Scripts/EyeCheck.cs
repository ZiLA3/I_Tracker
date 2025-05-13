using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeCheck : MonoBehaviour
{
    [Header("Eye Point Offset")]
    [SerializeField] Vector3 transformOffset;

    [Header("Eye Casting")]
    [SerializeField] float sphereCastRadius = 0.2f;
    [SerializeField] float maxDistance = 20f;

    LayerMask interactableLayer;
    Interactable currentInteractable;
    HashSet<Transform> interactedObjects;

    private void Start()
    {
        interactedObjects = new HashSet<Transform>();

        currentInteractable = null;
        interactableLayer = LayerMask.GetMask("Interactable"); // "Interactable" 레이어를 가진 오브젝트만 상호작용 가능
    }

    private void Update()
    {
        EyeVisualize(); // 눈 위치 시각화
        InteractCheck();

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if(currentInteractable != null)
            {
                currentInteractable.Interact(); 
                interactedObjects.Add(currentInteractable.transform.root);
            }
        }
    }

    private void EyeVisualize() 
    {
        Vector3 screenPos = FaceLandmark.EyePoint;
        screenPos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        transform.position = worldPos + transformOffset;
    }

    private void InteractCheck() 
    {
        Vector3 eyePoint = FaceLandmark.EyePoint;
        // 1) 화면상의 마우스 위치로부터 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(eyePoint);

        // 2) RaycastHit 변수 선언
        if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, maxDistance, interactableLayer))
        {
            if (hit.transform.root.GetComponent<Interactable>()) 
            {
                if (!interactedObjects.Contains(hit.transform.root)) 
                {
                    currentInteractable = hit.transform.root.GetComponent<Interactable>();
                    return;
                }
            }
        }
        currentInteractable = null;
    }
}
