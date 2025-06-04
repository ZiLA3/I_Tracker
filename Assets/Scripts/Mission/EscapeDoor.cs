using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeDoor : MissionObject
{
    [Header("Door Setting")]
    [SerializeField] GameObject door; 
    [Space]
    [SerializeField] Vector3 doorLastEulerAngle;
    [SerializeField] float rotateSpeed = 5f;
    [SerializeField] float delayTimeToRotate = .3f;

    bool isDoorOpen = false; 
    bool succeeded = false; 

    private void Update()
    {
        if (Player.Instance.Mission.currentInteractable != this || !Player.Instance.Mission.IsInMission)
            return;

        if (isDoorOpen)
            door.transform.localRotation = Quaternion.Slerp(door.transform.localRotation, Quaternion.Euler(doorLastEulerAngle), Time.deltaTime * rotateSpeed);

        if (succeeded)
            return; // 이미 문 열기 동작이 트리거되었으면 업데이트 중지

        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (Player.Instance.Hand.catchAction && Player.Instance.Mission.PlayerKeyCount == 3)
        {

            Invoke(nameof(SetDoorOpen), delayTimeToRotate); // 레버 당김 상태를 true로 설정

            succeeded = true;
            inMissionUI.SetActive(false); // 미션 UI 비활성화

            Player.Instance.SetGameWinTrue();
        }
    }

    private void SetDoorOpen() => isDoorOpen = true;

    public override void Interact()
    {
        base.Interact();

        SetHandActive(true);

        Player.Instance.Hand.SetHandTrackingActive(true); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.Catch); // RSP 미션 타입 설정
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        CameraManager.Instance.ToggleCamera(CameraType.doorCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetHandTrackingActive(false); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // 손 미션 타입 초기화
        Player.Instance.Hand.SetAnimator(null); // 애니메이터 초기화

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);
    }
}
