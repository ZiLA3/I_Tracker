using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] GameObject winUI; // 승리 UI
    [SerializeField] GameObject loseUI; // 패배 UI
    [SerializeField] TextMeshProUGUI keyCountText; // 키 개수 UI (추가된 부분)
    [SerializeField] float timeToShowWinUI = 2f; // 승리 UI 표시 시간
    [SerializeField] float timeToShowLoseUI = 2f; // 패배 UI 표시 시간s
    float timer = 0f;

    public event Action KeyCountChanged; // 키 개수 변경 이벤트

    public static Player Instance { get; private set; }
    public HandCheck Hand { get; private set; }
    public EyeCheck Eye { get; private set; }
    public MissionManager Mission { get; private set; }

    public bool IsGameOver { get; private set; } = false; // 게임 오버 상태
    public bool IsGameWin { get; private set; } = false; // 게임 승리 상태

    private bool triggeredOfGameWin = false; // 트리거 상태
    private bool triggeredOfGameOver = false; // 트리거 상태


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Hand = GetComponent<HandCheck>();
            Eye = GetComponent<EyeCheck>();
            Mission = GetComponent<MissionManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        KeyCountChanged += Mission.IncreaseKeyCount;
        KeyCountChanged += UpdateKeyCountText;
    }

    public void UpdateKey() 
    {
        KeyCountChanged?.Invoke(); // 키 개수 변경 이벤트 호출
    }

    private void UpdateKeyCountText()
    {
        keyCountText.text = $"Key Counts : {Mission.PlayerKeyCount}"; // 키 개수 UI 업데이트
    }

    private void Update()
    {
        // 게임 승리 상태가 true이고, 트리거가 아직 발생하지 않았다면
        if (IsGameWin && !triggeredOfGameWin)
        {
            triggeredOfGameWin = true; // 트리거 상태를 true로 설정
            timer = timeToShowWinUI; // 승리 UI 표시 시간 초기화
        }

        // 게임 오버 상태가 true이고, 트리거가 아직 발생하지 않았다면
        if (IsGameOver && !triggeredOfGameOver)
        {
            triggeredOfGameOver = true; // 트리거 상태를 true로 설정
            timer = timeToShowLoseUI; // 패배 UI 표시 시간 초기화
        }

        // 게임 패배 UI 표시 시간 설정
        if (IsGameOver)
        {
            timer -= Time.deltaTime; // 타이머 감소

            if (timer <= 0f)
            {
                loseUI.SetActive(true); // 패배 UI 활성화
                IsGameOver = false; // 게임 오버 상태 초기화
                triggeredOfGameOver = false; // 트리거 상태 초기화
            }
        }

        // 게임 승리 UI 표시 시간 설정
        if (IsGameWin)
        {
            timer -= Time.deltaTime; // 타이머 감소

            if (timer <= 0f)
            {
                winUI.SetActive(true); // 승리 UI 활성화
                IsGameWin = false; // 게임 승리 상태 초기화
                triggeredOfGameWin = false; // 트리거 상태 초기화
            }
        }
    }

    public void SetGameWinTrue() => IsGameWin = true; // 게임 승리 상태 설정
    public void SetGameOverTrue() => IsGameOver = true; // 게임 오버 상태 설정

}
