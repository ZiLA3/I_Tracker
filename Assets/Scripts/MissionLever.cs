using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionLever : MissionObject
{
    [SerializeField] GameObject leverHandle; // ���� �ڵ� ������Ʈ

    [SerializeField] Vector3 leverLastEulerAngle;
    [SerializeField] float rotateSpeed = 5f; // ���� ȸ�� �ӵ�

    bool isLeverPulled = false; // ������ ��������� ����

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
                    Invoke("SetLeverPulledTrue", 0.2f); // ���� ��� ���¸� true�� ����
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

        Player.Instance.Hand.SetHandTrackingActive(true); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.PullDown); // RSP �̼� Ÿ�� ����
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        CameraManager.Instance.ToggleCamera(CameraType.leverCamera);
    }

    public override void ResetToMainView()
    {
        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetMainHandActive(true); // �� �� ��Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.Catch); // �� �̼� Ÿ�� �ʱ�ȭ
        Player.Instance.Hand.SetAnimator(null);

        inMissionUI?.SetActive(false);
        SetHandActive(false);

        base.ResetToMainView();
    }

    public override void SucceedMission()
    {
        base.SucceedMission();
        isLeverPulled = false; // ���� ��� ���� �ʱ�ȭ
    }
}
