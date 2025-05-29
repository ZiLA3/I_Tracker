using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class MissionRSP : MissionObject
{
    [Header("RSP Mission Objects")]
    [SerializeField] GameObject timeText;
 
    [Header("Hand Materials")]
    [SerializeField] MeshRenderer handPictureRenderer;
    [Space]
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material rockMat;
    [SerializeField] private Material sissorMat;
    [SerializeField] private Material paperMat;

    [Header("RSP Mission Settings")]
    private RSPType missionRSPType;
    [SerializeField] float timeToGenerateRSP = 2f;
    [SerializeField] float rspTimer = 0f;

    bool stopped;

    private void Start()
    {
        rspTimer = timeToGenerateRSP;
    }

    private void Update()
    {
        if (Player.Instance.Mission.currentInteractable != this || !Player.Instance.Mission.IsInMission)
            return;

        if(stopped)
            return; // �̼��� ������ ��� ������Ʈ ����

        if (Input.GetMouseButtonDown(1))
            FailSetting();

        rspTimer -= Time.deltaTime;

        if (timeText.activeSelf) 
            timeText.GetComponent<TextMeshProUGUI>().text = rspTimer.ToString("F1");

        GenerateRSP();
    }

    private void GenerateRSP()
    {
        if (rspTimer <= 0)
        {
            rspTimer = timeToGenerateRSP;

            int randomValue = UnityEngine.Random.Range(0, 3);

            missionRSPType = (RSPType)randomValue;

            switch (missionRSPType)
            {
                case RSPType.Rock:
                    handPictureRenderer.sharedMaterial = rockMat;
                    break;
                case RSPType.Sissor:
                    handPictureRenderer.sharedMaterial = sissorMat;
                    break;
                case RSPType.Paper:
                    handPictureRenderer.sharedMaterial = paperMat;
                    break;
            }

            CheckRSP();
        }
    }

    private void CheckRSP()
    {
        Player.Instance.Hand.SetRSPCaptureActive(true); // RSP ĸó ����

        RSPType type = Player.Instance.Hand.CurrentRSPType;

        if (type == RSPType.Rock && missionRSPType == RSPType.Sissor)
            SucceedSetting();
        else if (type == RSPType.Sissor && missionRSPType == RSPType.Paper)
            SucceedSetting();
        else if (type == RSPType.Paper && missionRSPType == RSPType.Rock)
            SucceedSetting();
        else
        {
            FailSetting();
            timeText.GetComponent<TextMeshProUGUI>().text = "Failed!";
        }

        Player.Instance.Hand.SetRSPCaptureActive(false); // RSP ĸó ����
    }

    private void SucceedSetting()
    {
        stopped = true; // RSP ���� �� �̼� ����
        timeText.GetComponent<TextMeshProUGUI>().text = "Succeded!";
        Invoke("SucceedMission", 1f);
    }
    private void FailSetting()
    {
        stopped = true; // ���콺 ������ ��ư Ŭ�� �� �̼� ����
        Player.Instance.Hand.SetDefaultRSPState(); // RSP ���� �ʱ�ȭ
        Invoke(nameof(ResetToMainView), 1f); // ���� �� ���� ��� ���ư���
    }

    public override void Interact()
    {
        base.Interact();

        stopped = false; // �̼� ���� ���� �ʱ�ȭ

        SetHandActive(true); // �� Ȱ��ȭ

        Player.Instance.Hand.SetHandTrackingActive(true); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.RSP); // RSP �̼� Ÿ�� ����
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        timeText?.SetActive(true);
        rspTimer = timeToGenerateRSP; // RSP Ÿ�̸� �ʱ�ȭ

        CameraManager.Instance.ToggleCamera(CameraType.rspCamera);
    }

    public override void ResetToMainView()
    {
        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetAnimator(null); // �ִϸ����� �ʱ�ȭ
        Player.Instance.Hand.SetHandTrackingActive(false); // �� ���� Ȱ��ȭ
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // �� �̼� Ÿ�� �ʱ�ȭ

        inMissionUI.SetActive(false);
        timeText.SetActive(false);
        SetHandActive(false);

        handPictureRenderer.sharedMaterial = defaultMat; // �� ���� �ʱ�ȭ

        CameraManager.Instance.ToggleCamera(CameraType.mainCamera); // ���� ī�޶�� ��ȯ
    }

    public override void SucceedMission()
    {
        base.SucceedMission();
    }
}
