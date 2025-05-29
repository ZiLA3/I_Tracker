using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RSPType
{
    Rock = 0,
    Sissor = 1,
    Paper = 2,
}

public enum HandActionType 
{
    None,
    RSP = 0,
    PullDown = 1,
    Catch = 2,
}

public class HandCheck : MonoBehaviour
{
    [SerializeField] GameObject mainHand;
    [SerializeField] Animator mainHandAnim;

    Animator anim;

    public HandActionType HandType { get; private set; }

    public RSPType CurrentRSPType; // { get; private set; }
    public RSPType InputRSPType; // { get; private set; }
    
    public bool leverPullDown; // { get; private set; }
    public bool keyCatch; // {get; private set;} 키 잡기 여부

    public bool handTrackingOn; // private
    private bool rspCapture;
    bool triggered = false; // 손 잡기 동작이 트리거되었는지 여부

    float timer;
    float handCatchTime = 0.3f; // 미션 통과를 위한 시간

    private void Start()
    {
        handTrackingOn = true;

        HandType = HandActionType.Catch;
        CurrentRSPType = RSPType.Paper; // 초기값 설정
        InputRSPType = RSPType.Paper; // 초기값 설정

        SetMainHandActive(true); // mainHand 활성화 및 Animator 설정
    }

    void Update()
    {
        if (!handTrackingOn)
            return;

        if (HandType == HandActionType.RSP)
            GenerateRSP();
        else if (HandType == HandActionType.PullDown)
            HandPullDown();
        else if (HandType == HandActionType.Catch)
            HandCatch(); // 손 잡기 동작 처리

        // 손 잡기 동작을 위한 타이머
        if (leverPullDown)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                leverPullDown = false; // 손 잡기 동작 완료 후 false로 설정
                triggered = false; // 손 잡기 동작이 완료되었으므로 트리거 상태 초기화
            }
        }
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

    public void HandPullDown()
    {
        if (leverPullDown && !triggered) 
        {
            anim.SetTrigger("Pull");
            triggered = true; // 손 잡기 동작이 트리거되었음을 표시
        }
    }

    public void HandCatch() 
    {
        if (leverPullDown && !triggered)
        { 
            anim.SetTrigger("Catch");
            triggered = true; // 손 잡기 동작이 트리거되었음을 표시
        }
    }

    [ContextMenu("LeverPullDownActive")]
    public void SetLeverPulldown()
    {
        leverPullDown = true; // 손 잡기 동작 활성화

        // 손 잡기 동작을 위한 타이머 초기화
        timer = handCatchTime;
    }

    public void SetHandMissionType(HandActionType type) => HandType = type;

    public void SetRSPCaptureActive(bool active) => rspCapture = active;

    public void SetInputRSPType(RSPType type) => InputRSPType = type;

    public void SetHandTrackingActive(bool active) => handTrackingOn = active;

    public void SetAnimator(Animator handAnim) => anim = handAnim;

    public void SetMainHandActive(bool active)
    {
        mainHand.SetActive(active);

        if(active)
            anim = mainHandAnim; // Animator를 mainHandAnim으로 설정
    }
}
