using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionLever : MissionObject
{
    [Header("Lever Settings")]
    [SerializeField] GameObject leverHandle; // ���� �ڵ� ������Ʈ
    [Space]
    [SerializeField] Vector3 leverLastEulerAngle;
    [SerializeField] float rotateSpeed = 5f; // ���� ȸ�� �ӵ�
    [SerializeField] float delayTimeToRotate = .5f; // ���� ��� ������ �ð�

    bool isLeverPulled = false; // ������ ��������� ����
    bool succeeded = false; // ���� ���� -> �������� �� ���۵��� ���� ���� ����
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
            Invoke(nameof(SetLeverPulledTrue), delayTimeToRotate); // ���� ��� ���¸� true�� ����

            succeeded = true;
            Invoke("SucceedMission", 1.2f);
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
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetHandTrackingActive(false); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // �� �̼� Ÿ�� �ʱ�ȭ
        Player.Instance.Hand.SetAnimator(null); // �ִϸ����� �ʱ�ȭ

        if(inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);
    }

    public override void SucceedMission()
    {
        base.SucceedMission();

        isLeverPulled = false; // ���� ��� ���� �ʱ�ȭ
    }
}
