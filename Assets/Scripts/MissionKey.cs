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
    [SerializeField] KeyType keyType; // Ű Ÿ�� (Black, Red, Blue)
    [SerializeField] float time = .3f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (Player.Instance.Mission.currentInteractable == this && Player.Instance.Mission.IsInMission)
        {
            if (Player.Instance.Hand.catchAction)
            {
                Invoke("SucceedMission", time); // Ű�� ����� �� �̼� ����
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        SetHandActive(true);

        Player.Instance.Hand.SetHandTrackingActive(true); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.Catch); // RSP �̼� Ÿ�� ����
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        // Ű Ÿ�Կ� ���� ī�޶� ��ȯ
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

        Player.Instance.Hand.SetHandTrackingActive(false); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // �� �̼� Ÿ�� �ʱ�ȭ
        Player.Instance.Hand.SetAnimator(null); // �ִϸ����� �ʱ�ȭ

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);

        base.ResetToMainView();
    }

    public override void SucceedMission()
    {
        ResetToMainView();
        transform.gameObject.SetActive(false); // Ű ������Ʈ ��Ȱ��ȭ
    }
}
