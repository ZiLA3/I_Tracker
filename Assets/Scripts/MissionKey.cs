using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyType
{
    Black,
    Red,
    Blue,
}

public class MissionKey : MissionObject
{
    [SerializeField] KeyType keyType; // 키 타입 (Black, Red, Blue)
    [SerializeField] float time = .3f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (Player.Instance.Mission.currentInteractable == this && Player.Instance.Mission.IsInMission)
        {
            if (Player.Instance.Hand.catchAction)
            {
                Invoke("SucceedMission", time); // 키를 잡았을 때 미션 성공
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        SetHandActive(true);

        Player.Instance.Hand.SetHandTrackingActive(true); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.Catch); // RSP 미션 타입 설정
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        // 키 타입에 따라 카메라 전환
        switch(keyType)
        {
            case KeyType.Black:
                CameraManager.Instance.ToggleCamera(CameraType.blackKeyCamera);
                break;
            case KeyType.Red:
                CameraManager.Instance.ToggleCamera(CameraType.redKeyCamera);
                break;
            case KeyType.Blue:
                CameraManager.Instance.ToggleCamera(CameraType.blueKeyCamera);
                break;
            }
    }

    public override void ResetToMainView()
    {
        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetHandTrackingActive(false); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // 손 미션 타입 초기화
        Player.Instance.Hand.SetAnimator(null); // 애니메이터 초기화

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);

        base.ResetToMainView();
    }

    public override void SucceedMission()
    {
        ResetToMainView();
        transform.gameObject.SetActive(false); // 키 오브젝트 비활성화
    }
}
