using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RSPType
{
    Rock = 0,
    Sissor = 1,
    Paper = 2,
}

public enum HandMissionType 
{
    RSP = 0,
    OpenClose = 1,
}

public class HandCheck : MonoBehaviour
{
    Animator anim;

    public HandMissionType HandMissionType { get; private set; }

    public RSPType CurrentRSPType;// { get; private set; }
    public RSPType InputRSPType;// { get; private set; }

    private bool handTrackingOn;
    private bool rspCapture;

    private void Start()
    {
        CurrentRSPType = RSPType.Paper; // 초기값 설정
        InputRSPType = RSPType.Paper; // 초기값 설정
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            MissionManager missionManager = MissionManager.Instance;

            missionManager.PopMissionKey();
        }

        if (!handTrackingOn)
            return;

        if (HandMissionType == HandMissionType.RSP)
            GenerateRSP();
        else if (HandMissionType == HandMissionType.OpenClose) ;
            // GenerateRSP(); // OpenClose 미션에서도 RSP 생성 로직을 사용하도록 변경
    }

    private void GenerateRSP() 
    {
        if (CurrentRSPType == InputRSPType)
            return;

        if(rspCapture) // RSP 캡처가 활성화된 경우 혼잡도를 줄이기 위해서
            return;

        CurrentRSPType = InputRSPType; // 입력된 RSP 타입으로 업데이트

        anim.SetBool("Sissor", false);
        anim.SetBool("Paper", false);
        anim.SetBool("Rock", false);

        switch (CurrentRSPType) 
        {
            case RSPType.Rock:
                anim.SetBool("Rock", true);
                break;
            case RSPType.Sissor:
                anim.SetBool("Sissor", true);
                break;
            case RSPType.Paper:
                anim.SetBool("Paper", true);
                break;
        }              
    }

    public void SetHandMissionType(HandMissionType type) => HandMissionType = type;

    public void SetRSPCaptureActive(bool active) => rspCapture = active;

    public void SetInputRSPType(RSPType type) => InputRSPType = type;

    public void SetHandTrackingActive(bool active) => handTrackingOn = active;

    public void SetAnimator(Animator handAnim) => anim = handAnim;
}
