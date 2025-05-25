using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeCheck : MonoBehaviour
{
    [Header("Eye Point")]
    [SerializeField] Vector3 eyePointOffset = Vector3.zero; // 눈 위치 오프셋
    [SerializeField] Transform eye;

    [Header("Eye Casting")]
    [SerializeField] float sphereCastRadius = 0.2f;
    [SerializeField] float maxDistance = 20f;
    Vector3 eyePoint;
    Ray ray;

    [Header("Cmaera Zoom in")]
    [SerializeField] float zoomInFOV = 30f;
    float defaultFOV;
    bool isZoomedIn = false;

    // interactable 오브젝트를 찾기 위한 변수
    IInteractable currentInteractable;
    HashSet<IInteractable> interactedObjects;

    // LayerMask
    LayerMask interactableLayer;
    LayerMask monsterLayer;

    private void Start()
    {
        interactedObjects = new HashSet<IInteractable>();

        currentInteractable = null;

        defaultFOV = Camera.main.fieldOfView; // 기본 FOV 저장

        interactableLayer = LayerMask.GetMask("Interactable"); // "Interactable" 레이어를 가진 오브젝트만 상호작용 가능
        monsterLayer = LayerMask.GetMask("Monster"); // "Monster" 레이어를 가진 오브젝트만 감지
    }

    private void Update()
    {
        GenerateEyeRay(); // 선호출
        EyeVisualize(); // 눈 위치 시각화
        EnemyOnSight(); // 적이 시야에 들어왔는지 확인
        InteractCheck();
        InputControl();
    }
    private void GenerateEyeRay()
    {
        eyePoint = FaceLandmark.EyePoint;
        ray = Camera.main.ScreenPointToRay(eyePoint);
    }
    
    private void EyeVisualize() 
    {
        Vector3 screenPos = eyePoint;
        screenPos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        eye.position = worldPos + eyePointOffset;
    }

    private void InteractCheck() 
    {
        // 2) RaycastHit 변수 선언
        if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, maxDistance, interactableLayer))
        {
            IInteractable interactalbe = hit.transform.root.GetComponent<IInteractable>();

            if (interactalbe != null) 
            {
                if (!interactedObjects.Contains(interactalbe)) 
                {
                    currentInteractable = interactalbe;
                    return;
                }
            }
        }
        currentInteractable = null;
    }

    private void EnemyOnSight() 
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, monsterLayer)) 
        {
            Monster ememy = Monster.instance;

            ememy.isInSight = true; // 적이 시야에 들어옴
        }
        else
        {
            Monster ememy = Monster.instance;

            ememy.isInSight = false; // 적이 시야에서 벗어남
        }
    }

    private void InputControl()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
                interactedObjects.Add(currentInteractable);
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
}
