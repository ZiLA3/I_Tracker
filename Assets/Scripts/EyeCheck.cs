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

    [Header("Cmaera Zoom in")]
    [SerializeField] float zoomInFOV = 30f;
    float defaultFOV;
    bool isZoomedIn = false;

    LayerMask interactableLayer;
    Interactable currentInteractable;
    HashSet<Transform> interactedObjects;

    Vector3 eyePoint;
    Ray ray;

    private void Start()
    {
        interactedObjects = new HashSet<Transform>();

        currentInteractable = null;

        interactableLayer = LayerMask.GetMask("Interactable"); // "Interactable" 레이어를 가진 오브젝트만 상호작용 가능
        defaultFOV = Camera.main.fieldOfView; // 기본 FOV 저장
    }

    private void Update()
    {
        GenerateEyeRay(); // 선호출
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

        // zoom in 효과
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            isZoomedIn = !isZoomedIn; // 줌인 상태 토글

            if (isZoomedIn)
            {
                Camera.main.fieldOfView = zoomInFOV; // 줌인
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 targetPoint = hit.point;
                    Camera.main.transform.LookAt(targetPoint);
                }
            }
            else 
            {
                Camera.main.fieldOfView = defaultFOV; // 기본 FOV로 복원
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0); // 카메라 각도 초기화
            }
        }
    }

    private void EyeVisualize() 
    {
        Vector3 screenPos = eyePoint;
        screenPos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        transform.position = worldPos + transformOffset;
    }

    private void InteractCheck() 
    {
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

    private void GenerateEyeRay()
    {
        eyePoint = FaceLandmark.EyePoint;
        ray = Camera.main.ScreenPointToRay(eyePoint);
    }
}
