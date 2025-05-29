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
        CurrentRSPType = RSPType.Paper; // �ʱⰪ ����
        InputRSPType = RSPType.Paper; // �ʱⰪ ����
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
            // GenerateRSP(); // OpenClose �̼ǿ����� RSP ���� ������ ����ϵ��� ����
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

    public void SetHandMissionType(HandMissionType type) => HandMissionType = type;

    public void SetRSPCaptureActive(bool active) => rspCapture = active;

    public void SetInputRSPType(RSPType type) => InputRSPType = type;

    public void SetHandTrackingActive(bool active) => handTrackingOn = active;

    public void SetAnimator(Animator handAnim) => anim = handAnim;
}
