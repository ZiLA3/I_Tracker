using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RSPType
{
    Rock = 0,
    Sissor = 1,
    Paper = 2,
}

public class HandCheck : MonoBehaviour
{
    Animator anim;

    public RSPType currentRSPType;// gel; private set
    public RSPType inputRSPType;

    public bool HandTrackingOn;
    public bool rspCapture = false; // RSP ĸó ����

    private void Start()
    {
        anim = GetComponent<Animator>();

        currentRSPType = RSPType.Paper; // �ʱⰪ ����
        inputRSPType = RSPType.Paper; // �ʱⰪ ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            MissionManager missionManager = MissionManager.Instance;

            missionManager.PopMissionKey();
        }

        GenerateRSP();
    }

    private void GenerateRSP() 
    {
        if (!HandTrackingOn)
            return;

        if (currentRSPType == inputRSPType)
            return;

        if(rspCapture) // RSP ĸó�� Ȱ��ȭ�� ��� ȥ�⵵�� ���̱� ���ؼ�
            return;

        currentRSPType = inputRSPType; // �Էµ� RSP Ÿ������ ������Ʈ

        anim.SetBool("Sissor", false);
        anim.SetBool("Paper", false);
        anim.SetBool("Rock", false);

        switch (currentRSPType) 
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

    public void SetRSPCaptureActive(bool active)
    {
        rspCapture = active;
    }
}
