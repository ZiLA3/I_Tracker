using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public enum RSPType
{
    Rock,
    Sissor,
    Paper
}

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
    [SerializeField] float timeToGenerateRSP;
    float rspTimer = 0f;

    Animator anim;
    bool triggered; // 멈춤 여부 -> 메인 뷰로 돌아가거나 성공했을 떄 오작동을 막기 위한 변수

    private void Start()
    {
        anim = hand.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Player.Instance.Mission.currentInteractable != this || !Player.Instance.Mission.IsInMission)
            return;

        if(triggered)
            return;

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
                default:
                    handPictureRenderer.sharedMaterial = defaultMat; // 기본 재질로 설정
                    break;
            }

            CheckRSP();
        }
    }

    private void CheckRSP()
    {
        RSPType playerRSPType = RSPType.Paper;

        switch (FaceLandmark.HandFolds) 
        {
            case 15: // 손가락이 모두 펴져 있을 때
                playerRSPType = RSPType.Rock;
                anim.SetBool("Rock", true);
                break;
            case 3: // 엄지와 검지가 접혀 있을 때
                playerRSPType = RSPType.Sissor;
                anim.SetBool("Sissor", true);
                break;
            case 0: // 엄지와 검지를 제외한 모든 손가락이 접혀 있을 때
                playerRSPType = RSPType.Paper;
                anim.SetBool("Paper", true);
                break;
        }

        if (playerRSPType == RSPType.Rock && missionRSPType == RSPType.Sissor)
            SucceedSetting();
        else if (playerRSPType == RSPType.Sissor && missionRSPType == RSPType.Paper)
            SucceedSetting();
        else if (playerRSPType == RSPType.Paper && missionRSPType == RSPType.Rock)
            SucceedSetting();
        else
        {
            FailSetting();
        }
    }

    private void SucceedSetting()
    {
        triggered = true; // RSP 성공 시 미션 중지
        timeText.GetComponent<TextMeshProUGUI>().text = "Succeded!";
        Invoke("SucceedMission", 1f);
    }
    private void FailSetting()
    {
        triggered = true; // 마우스 오른쪽 버튼 클릭 시 미션 중지
        Invoke(nameof(ResetToMainView), 1f); // 실패 시 메인 뷰로 돌아가기
    }

    public override void Interact()
    {
        base.Interact();

        triggered = false; // 미션 중지 상태 초기화

        SetHandActive(true); // 손 활성화

        timeText.SetActive(true);
        rspTimer = timeToGenerateRSP; // RSP 타이머 초기화

        CameraManager.Instance.ToggleCamera(CameraType.rspCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        inMissionUI.SetActive(false);
        timeText.SetActive(false);
        SetHandActive(false);

        handPictureRenderer.sharedMaterial = defaultMat; // 손 재질 초기화
    }

    public override void SucceedMission()
    {
        base.SucceedMission();
    }
}
