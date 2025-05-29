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
    public bool keyCatch; // {get; private set;} Ű ��� ����

    public bool handTrackingOn; // private
    private bool rspCapture;
    bool triggered = false; // �� ��� ������ Ʈ���ŵǾ����� ����

    float timer;
    float handCatchTime = 0.3f; // �̼� ����� ���� �ð�

    private void Start()
    {
        handTrackingOn = true;

        HandType = HandActionType.Catch;
        CurrentRSPType = RSPType.Paper; // �ʱⰪ ����
        InputRSPType = RSPType.Paper; // �ʱⰪ ����

        SetMainHandActive(true); // mainHand Ȱ��ȭ �� Animator ����
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
            HandCatch(); // �� ��� ���� ó��

        // �� ��� ������ ���� Ÿ�̸�
        if (leverPullDown)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                leverPullDown = false; // �� ��� ���� �Ϸ� �� false�� ����
                triggered = false; // �� ��� ������ �Ϸ�Ǿ����Ƿ� Ʈ���� ���� �ʱ�ȭ
            }
        }
    }

    private void GenerateRSP() 
    {
        if (CurrentRSPType == InputRSPType)
            return;

        if(rspCapture) // RSP ĸó�� Ȱ��ȭ�� ��� ȥ�⵵�� ���̱� ���ؼ�
            return;

        CurrentRSPType = InputRSPType; // �Էµ� RSP Ÿ������ ������Ʈ

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
            triggered = true; // �� ��� ������ Ʈ���ŵǾ����� ǥ��
        }
    }

    public void HandCatch() 
    {
        if (leverPullDown && !triggered)
        { 
            anim.SetTrigger("Catch");
            triggered = true; // �� ��� ������ Ʈ���ŵǾ����� ǥ��
        }
    }

    [ContextMenu("LeverPullDownActive")]
    public void SetLeverPulldown()
    {
        leverPullDown = true; // �� ��� ���� Ȱ��ȭ

        // �� ��� ������ ���� Ÿ�̸� �ʱ�ȭ
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
            anim = mainHandAnim; // Animator�� mainHandAnim���� ����
    }
}
