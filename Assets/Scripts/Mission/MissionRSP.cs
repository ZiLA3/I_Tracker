using TMPro;
using UnityEngine;

public enum RSPType
{
    Paper = 0,
    Rock = 1,
    Sissor = 2
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
    private RSPType currentPlayerRspType;
    private RSPType beforePlayerRSPType;
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
        if (GameManager.Instance.Mission.currentInteractable != this || !GameManager.Instance.Mission.IsInMission)
            return;

        if (triggered)
            return;

        if (Input.GetMouseButtonDown(1))
            FailSetting();

        rspTimer -= Time.deltaTime;

        if (timeText.activeSelf)
            timeText.GetComponent<TextMeshProUGUI>().text = rspTimer.ToString("F1");

        GeneratePlayerRSP();
        GenerateMissionRSP();
    }

    private void GenerateMissionRSP()
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
                    handPictureRenderer.sharedMaterial = paperMat; // 기본 재질로 설정
                    break;
            }

            CheckRSP();
        }
    }

    private void CheckRSP()
    {
        if (currentPlayerRspType == RSPType.Rock && missionRSPType == RSPType.Sissor)
            SucceedSetting();
        else if (currentPlayerRspType == RSPType.Sissor && missionRSPType == RSPType.Paper)
            SucceedSetting();
        else if (currentPlayerRspType == RSPType.Paper && missionRSPType == RSPType.Rock)
            SucceedSetting();
        else
        {
            timeText.GetComponent<TextMeshProUGUI>().text = "Failed!!";
            FailSetting();
        }
    }

    private void GeneratePlayerRSP()
    {
        switch (FaceLandmark.HandFolds)
        {
            case 15:
                currentPlayerRspType = RSPType.Rock;
                break;
            case 12: 
                currentPlayerRspType = RSPType.Sissor;
                break;
            case 0: 
                currentPlayerRspType = RSPType.Paper;
                break;
        }

        ChangeAnimation(currentPlayerRspType);
    }

    private void SucceedSetting()
    {
        triggered = true; // RSP 성공 시 미션 중지
        timeText.GetComponent<TextMeshProUGUI>().text = "Succeeded!!";
        Invoke("SucceedMission", 1f);
    }
    private void FailSetting()
    {
        triggered = true; // 마우스 오른쪽 버튼 클릭 시 미션 중지

        ChangeAnimation(RSPType.Paper);
        currentPlayerRspType = RSPType.Paper; // 초기값 설정

        Invoke(nameof(ResetToMainView), 1f); // 실패 시 메인 뷰로 돌아가기
    }

    private void ChangeAnimation(RSPType currentType)
    {
        if (currentType != beforePlayerRSPType)
            beforePlayerRSPType = currentType; // 이전 RSP 타입 업데이트

        anim.SetBool("Paper", false);
        anim.SetBool("Rock", false);
        anim.SetBool("Sissor", false);

        switch (currentType)
        {
            case RSPType.Rock:
                anim.SetBool("Rock", true);
                break;
            case RSPType.Paper:
                anim.SetBool("Paper", true);
                break;
            case RSPType.Sissor:
                anim.SetBool("Sissor", true);
                break;
        }
    }

    public override void Interact()
    {
        base.Interact();

        triggered = false; // 미션 중지 상태 초기화

        SetHandActive(true); // 손 활성화

        timeText.SetActive(true);
        rspTimer = timeToGenerateRSP; // RSP 타이머 초기화

        currentPlayerRspType = RSPType.Paper; // 초기값 설정
        beforePlayerRSPType = RSPType.Paper; // 초기값 설정

        CameraManager.Instance.ToggleCamera(CameraType.rspCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        GameManager.Instance.Mission.SetMissionActive(false);

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
