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

    bool triggered; // 멈춤 여부 -> 메인 뷰로 돌아가거나 성공했을 떄 오작동을 막기 위한 변수

    private void Start()
    {
        rspTimer = timeToGenerateRSP;
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
                case RSPType.Paper:
                    handPictureRenderer.sharedMaterial = paperMat;
                    break;
            }

            CheckRSP();
        }
    }

    private void CheckRSP()
    {
        Player.Instance.Hand.SetRSPCaptureActive(true); // RSP 캡처 시작

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

        Player.Instance.Hand.SetRSPCaptureActive(false); // RSP 캡처 종료
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
        Player.Instance.Hand.SetDefaultRSPState(); // RSP 상태 초기화
        Invoke(nameof(ResetToMainView), 1f); // 실패 시 메인 뷰로 돌아가기
    }

    public override void Interact()
    {
        base.Interact();

        triggered = false; // 미션 중지 상태 초기화

        SetHandActive(true); // 손 활성화

        Player.Instance.Hand.SetHandTrackingActive(true); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.RSP); // RSP 미션 타입 설정
        Player.Instance.Hand.SetAnimator(hand.GetComponent<Animator>());

        timeText?.SetActive(true);
        rspTimer = timeToGenerateRSP; // RSP 타이머 초기화

        CameraManager.Instance.ToggleCamera(CameraType.rspCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        Player.Instance.Hand.SetAnimator(null); // 애니메이터 초기화
        Player.Instance.Hand.SetHandTrackingActive(false); // 손 추적 활성화
        Player.Instance.Hand.SetHandMissionType(HandActionType.None); // 손 미션 타입 초기화

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
