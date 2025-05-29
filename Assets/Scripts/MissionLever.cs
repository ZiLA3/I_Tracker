using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionLever : MissionObject
{
    [SerializeField] GameObject leverHandle; // 레버 핸들 오브젝트

    [SerializeField] Vector3 leverLastEulerAngle;
    [SerializeField] float rotateSpeed = 5f; // 레버 회전 속도

    bool isLeverPulled = false; // 레버가 당겨졌는지 여부

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (Player.Instance.Mission.currentInteractable == this && Player.Instance.Mission.IsInMission) 
        {
            if (Player.Instance.Hand.leverPullDown)
            {
                if (!isLeverPulled)
                {
                    Invoke("SetLeverPulledTrue", 0.2f); // 레버 당김 상태를 true로 설정
                }

                Invoke("SucceedMission", 1.2f);
            }

            if (isLeverPulled)
            {
                leverHandle.transform.localRotation = Quaternion.Slerp(leverHandle.transform.localRotation, Quaternion.Euler(leverLastEulerAngle), Time.deltaTime * rotateSpeed);
            }
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
        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetMainHandActive(true); // 손 모델 비활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.Catch); // 손 미션 타입 초기화
        Player.Instance.Hand.SetAnimator(null);

        inMissionUI?.SetActive(false);
        SetHandActive(false);

        base.ResetToMainView();
    }

    public override void SucceedMission()
    {
        base.SucceedMission();
        isLeverPulled = false; // 레버 당김 상태 초기화
    }
}
