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
    None = 0,
    RSP = 1,
    PullDown = 2,
    Catch = 3,
}

public class HandCheck : MonoBehaviour
{
    Animator anim;

    public HandActionType HandType { get; private set; } // 손 동작이 레버/catch/가위바위보/None 으로 나누기 위한 변수 -> 오동작을 막기 위함.

    public RSPType CurrentRSPType; // { get; private set; }
    public RSPType InputRSPType; // { get; private set; } => 데이터 분석 후 RSP 구분이 되면 그 때 이 변수를 사용하여 RSP 타입을 정해야 함.

    public bool leverPullDown; // { get; private set; }
    public bool catchAction; // {get; private set;} 키 잡기 여부

    public bool handTrackingOn; // private variable
    private bool rspCapture; // RSP 캡처 활성화 여부 -> 캡처할 때의 동작이 들어가서 혼잡도를 줄이기 위해 사용
    bool triggered = false; // 손 잡기 동작이 트리거되었는지 여부 -> 애니메이션이 한번만 실행되도록 하기 위함

    float timer;
    float delayTime = 0.3f; // 미션 통과를 위한 시간 -> 손 잡기 동작을 다른 스크립트가 인식하기 위한 시간

    private void Start()
    {
        HandType = HandActionType.Catch;
        CurrentRSPType = RSPType.Paper; // 초기값 설정
        InputRSPType = RSPType.Paper; // 초기값 설정
    }

    void Update()
    {
        if (!handTrackingOn)
            return;

        // TEST 용 Input 시그널

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetLeverPulldown();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetCatch();
        }
        //

        if (HandType == HandActionType.RSP)
            GenerateRSP();
        else if (HandType == HandActionType.PullDown)
            HandPullDown();
        else if (HandType == HandActionType.Catch)
            HandCatch(); // 손 잡기 동작 처리

        if (leverPullDown)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                leverPullDown = false; // 손 잡기 동작 완료 후 false로 설정
                triggered = false; // 손 잡기 동작이 완료되었으므로 트리거 상태 초기화
            }
        }

        if (catchAction)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                catchAction = false; // 손 잡기 동작 완료 후 false로 설정
                triggered = false; // 손 잡기 동작이 완료되었으므로 트리거 상태 초기화
            }
        }
    }

    private void GenerateRSP()
    {
        if (CurrentRSPType == InputRSPType)
            return;

        if (rspCapture) // RSP 캡처가 활성화된 경우 혼잡도를 줄이기 위해서
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

    // 기본 RSP 상태 설정 메서드 -> 동작이 paper가 되도록 설정
    public void SetDefaultRSPState()
    {
        CurrentRSPType = RSPType.Paper; // 기본 RSP 타입을 Paper로 설정
        InputRSPType = RSPType.Paper; // 입력된 RSP 타입도 Paper로 설정

        anim.SetBool("Sissor", false);
        anim.SetBool("Rock", false);
        anim.SetBool("Paper", true);
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
        if (catchAction && !triggered)
        {
            anim.SetTrigger("Catch");
            triggered = true; // 손 잡기 동작이 트리거되었음을 표시
        }
    }

    private void SetLeverPulldown()
    {
        leverPullDown = true;

        timer = delayTime;
    }

    private void SetCatch()
    {
        catchAction = true;

        timer = delayTime;
    }

    public void SetHandMissionType(HandActionType type) => HandType = type;

    public void SetRSPCaptureActive(bool active) => rspCapture = active;

    public void SetInputRSPType(RSPType type) => InputRSPType = type; // 해당 메서드는 외부에서 RSP 타입을 설정할 때 사용됩니다.
    public void SetInputRSPType(int type) => InputRSPType = (RSPType)type; // 해당 메서드는 외부에서 RSP 타입을 설정할 때 사용됩니다.

    public void SetHandTrackingActive(bool active) => handTrackingOn = active;

    public void SetAnimator(Animator handAnim)
    {
        anim = handAnim;
    }
}