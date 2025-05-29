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

    // LayerMask
    LayerMask interactableLayer;
    LayerMask monsterLayer;

    public bool EnemyOnSight { get; private set; } // 적이 시야에 들어왔는지 여부

    private void Start()
    {
        defaultFOV = Camera.main.fieldOfView; // 기본 FOV 저장

        interactableLayer = LayerMask.GetMask("Interactable"); // "Interactable" 레이어를 가진 오브젝트만 상호작용 가능
        monsterLayer = LayerMask.GetMask("Monster"); // "Monster" 레이어를 가진 오브젝트만 감지
    }

    private void Update()
    {
        GenerateEyeRay(); // 선호출
        EyeVisualize(); // 눈 위치 시각화
        CheckEnemyOnSight(); // 적이 시야에 들어왔는지 확인
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
        MissionManager missionManager = Player.Instance.Mission;

        if (missionManager.IsInMission)
            return;

        // RaycastHit 변수 선언
        if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, maxDistance, interactableLayer))
        {
            MissionObject interactalbe = hit.transform.GetComponentInParent<MissionObject>();

            if (interactalbe != null) 
            {
                if (!missionManager.IsInteracted(interactalbe)) 
                {
                    missionManager.SetCurrentInteractable(interactalbe); // 현재 상호작용 가능한 오브젝트 설정
                    missionManager.currentInteractable.SetInteractUIActive(true); // 상호작용 UI 활성화
                    return;
                }
            }
        }

        if(missionManager.currentInteractable != null) // 현재 상호작용 가능했던 오브젝트가 있다면
            missionManager.currentInteractable.SetInteractUIActive(false); // 상호작용 UI 비활성화
        
        missionManager.SetCurrentInteractable(null); // 현재 상호작용 가능한 오브젝트 초기화
    }

    private void CheckEnemyOnSight() 
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, monsterLayer)) 
        {
            Monster ememy = Monster.Instance;

            EnemyOnSight = true; // 적이 시야에 들어옴
        }
        else
        {
            Monster ememy = Monster.Instance;

            EnemyOnSight = false; // 적이 시야에서 벗어남
        }
    }

    private void InputControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MissionManager missionManager = Player.Instance.Mission;

            if (missionManager.currentInteractable != null)
            {
                missionManager.currentInteractable.Interact();
            }
        }

        // zoom in 효과 -> 마우스 휠 스크롤로 줌인/줌아웃
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (CameraManager.Instance.CurrentCameraType == CameraType.mainCamera) 
        {
            if (scroll > 0 && !isZoomedIn)
            {
                isZoomedIn = true;

                Camera.main.fieldOfView = zoomInFOV; // 줌인
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 targetPoint = hit.point;
                    Camera.main.transform.LookAt(targetPoint);
                }
            }
            else if (scroll < 0 && isZoomedIn)
            {
                isZoomedIn = false;

                Camera.main.fieldOfView = defaultFOV; // 줌아웃
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0); // 카메라 회전 초기화
            }
        }
    }
}
