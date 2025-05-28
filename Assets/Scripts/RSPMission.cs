using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;



public class RSPMission : MissionObject
{
    [Header("RSP Mission Objects")]
    [SerializeField] GameObject timeText;

    [Header("Hand Materials")]
    private Material defaultMat;
    [SerializeField] private Material rockMat;
    [SerializeField] private Material sissorMat;
    [SerializeField] private Material paperMat;

    [Header("RSP Mission Settings")]
    [SerializeField] float timeToGenerateRSP = 2f;
    public float rspTimer = 0f;
    
    [Space]
    [SerializeField] MeshRenderer handRenderer;
    [SerializeField] HandCheck handCheck;
    public RSPType missionRSPType;

    private void Start()
    {
        defaultMat = handRenderer.sharedMaterial;
        rspTimer = timeToGenerateRSP;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ResetToMainView();
            ActiveMissionObjectActive(false);
            MissionManager.Instance.SetMissionActive(false);
        }

        if(MissionManager.Instance.currentInteractable == this && MissionManager.Instance.IsInMission)
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
                    handRenderer.sharedMaterial = rockMat;
                    break;
                case RSPType.Sissor:
                    handRenderer.sharedMaterial = sissorMat;
                    break;
                case RSPType.Paper:
                    handRenderer.sharedMaterial = paperMat;
                    break;
            }

            CheckRSP();
        }
    }

    private void CheckRSP()
    {
        handCheck.rspCapture = true; // RSP 캡처 시작

        RSPType type = handCheck.currentRSPType;

        if (type == RSPType.Rock && missionRSPType == RSPType.Sissor)
            Invoke("SucceedMission", 1f);
        else if (type == RSPType.Sissor && missionRSPType == RSPType.Paper)
            Invoke("SucceedMission", 1f);
        else if (type == RSPType.Paper && missionRSPType == RSPType.Rock)
            Invoke("SucceedMission", 1f);
        else
            Invoke("ResetToMainView", 1f); // 실패 시 메인 뷰로 돌아가기

        handCheck.rspCapture = false; // RSP 캡처 종료
        timeText?.SetActive(false);
    }

    public override void Interact()
    {
        base.Interact();

        timeText?.SetActive(true);

        rspTimer = timeToGenerateRSP; // RSP 타이머 초기화

        CameraManager.Instance.ToggleCamera(CameraType.rspCamera);
    }

    public override void ResetToMainView()
    {
        MissionManager.Instance.SetMissionActive(false);

        inMissionUI?.SetActive(false);
        ActiveMissionObjectActive(false);
        timeText?.SetActive(false);

        handRenderer.sharedMaterial = defaultMat; // 손 재질 초기화

        base.ResetToMainView();
    }

    public override void SucceedMission()
    {
        base.SucceedMission();
    }
}
