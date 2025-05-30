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
            return; // �̹� �� ���� ������ Ʈ���ŵǾ����� ������Ʈ ����

        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (Player.Instance.Hand.catchAction && Player.Instance.Mission.PlayerKeyCount == 3)
        {

            Invoke(nameof(SetDoorOpen), delayTimeToRotate); // ���� ��� ���¸� true�� ����

            succeeded = true;
            inMissionUI.SetActive(false); // �̼� UI ��Ȱ��ȭ

            Player.Instance.SetGameWinTrue();
        }
    }

    private void SetDoorOpen() => isDoorOpen = true;

    public override void Interact()
    {
        base.Interact();

        SetHandActive(true);

        Player.Instance.Hand.SetHandTrackingActive(true); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.Catch); // RSP �̼� Ÿ�� ����
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        CameraManager.Instance.ToggleCamera(CameraType.doorCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetHandTrackingActive(false); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // �� �̼� Ÿ�� �ʱ�ȭ
        Player.Instance.Hand.SetAnimator(null); // �ִϸ����� �ʱ�ȭ

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);
    }
}
