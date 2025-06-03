using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionLever : MissionObject
{
    [Header("Lever Settings")]
    [SerializeField] GameObject leverHandle; // 레버 핸들 오브젝트
    [Space]
    [SerializeField] Vector3 leverLastEulerAngle;
    [SerializeField] float rotateSpeed = 5f; // 레버 회전 속도
    [SerializeField] float delayTimeToRotate = .5f; // 레버 당김 딜레이 시간

    bool isLeverPulled = false; // 레버가 당겨졌는지 여부
    bool succeeded = false; // 성공 여부 -> 성공했을 때 오작동을 막기 위한 변수
    private void Update()
    {
        if (Player.Instance.Mission.currentInteractable != this || !Player.Instance.Mission.IsInMission)
            return;

        if (isLeverPulled)
        {
            leverHandle.transform.localRotation = Quaternion.Slerp(leverHandle.transform.localRotation, Quaternion.Euler(leverLastEulerAngle), Time.deltaTime * rotateSpeed);
        }

        if (succeeded)
            return;

        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (Player.Instance.Hand.leverPullDown)
        {
            Invoke(nameof(SetLeverPulledTrue), delayTimeToRotate); // 레버 당김 상태를 true로 설정

            succeeded = true;
            Invoke("SucceedMission", 1.2f);
        }       
    }

    private void SetLeverPulledTrue() => isLeverPulled = true;

    public override void Interact()
    {
        base.Interact();

        SetHandActive(true);

        Player.Instance.Hand.SetHandTrackingActive(true); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.PullDown); // RSP 미션 타입 설정
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        CameraManager.Instance.ToggleCamera(CameraType.leverCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetHandTrackingActive(false); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // 손 미션 타입 초기화
        Player.Instance.Hand.SetAnimator(null); // 애니메이터 초기화

        if(inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);
    }

    public override void SucceedMission()
    {
        base.SucceedMission();

        isLeverPulled = false; // 레버 당김 상태 초기화
    }
}
