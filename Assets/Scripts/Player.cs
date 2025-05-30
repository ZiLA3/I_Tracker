using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] GameObject winUI; // �¸� UI
    [SerializeField] GameObject loseUI; // �й� UI
    [SerializeField] TextMeshProUGUI keyCountText; // Ű ���� UI (�߰��� �κ�)
    [SerializeField] float timeToShowWinUI = 2f; // �¸� UI ǥ�� �ð�
    [SerializeField] float timeToShowLoseUI = 2f; // �й� UI ǥ�� �ð�s
    float timer = 0f;

    public event Action KeyCountChanged; // Ű ���� ���� �̺�Ʈ

    public static Player Instance { get; private set; }
    public HandCheck Hand { get; private set; }
    public EyeCheck Eye { get; private set; }
    public MissionManager Mission { get; private set; }

    public bool IsGameOver { get; private set; } = false; // ���� ���� ����
    public bool IsGameWin { get; private set; } = false; // ���� �¸� ����

    private bool triggeredOfGameWin = false; // Ʈ���� ����
    private bool triggeredOfGameOver = false; // Ʈ���� ����


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
        KeyCountChanged?.Invoke(); // Ű ���� ���� �̺�Ʈ ȣ��
    }

    private void UpdateKeyCountText()
    {
        keyCountText.text = $"Key Counts : {Mission.PlayerKeyCount}"; // Ű ���� UI ������Ʈ
    }

    private void Update()
    {
        // ���� �¸� ���°� true�̰�, Ʈ���Ű� ���� �߻����� �ʾҴٸ�
        if (IsGameWin && !triggeredOfGameWin)
        {
            triggeredOfGameWin = true; // Ʈ���� ���¸� true�� ����
            timer = timeToShowWinUI; // �¸� UI ǥ�� �ð� �ʱ�ȭ
        }

        // ���� ���� ���°� true�̰�, Ʈ���Ű� ���� �߻����� �ʾҴٸ�
        if (IsGameOver && !triggeredOfGameOver)
        {
            triggeredOfGameOver = true; // Ʈ���� ���¸� true�� ����
            timer = timeToShowLoseUI; // �й� UI ǥ�� �ð� �ʱ�ȭ
        }

        // ���� �й� UI ǥ�� �ð� ����
        if (IsGameOver)
        {
            timer -= Time.deltaTime; // Ÿ�̸� ����

            if (timer <= 0f)
            {
                loseUI.SetActive(true); // �й� UI Ȱ��ȭ
                IsGameOver = false; // ���� ���� ���� �ʱ�ȭ
                triggeredOfGameOver = false; // Ʈ���� ���� �ʱ�ȭ
            }
        }

        // ���� �¸� UI ǥ�� �ð� ����
        if (IsGameWin)
        {
            timer -= Time.deltaTime; // Ÿ�̸� ����

            if (timer <= 0f)
            {
                winUI.SetActive(true); // �¸� UI Ȱ��ȭ
                IsGameWin = false; // ���� �¸� ���� �ʱ�ȭ
                triggeredOfGameWin = false; // Ʈ���� ���� �ʱ�ȭ
            }
        }
    }

    public void SetGameWinTrue() => IsGameWin = true; // ���� �¸� ���� ����
    public void SetGameOverTrue() => IsGameOver = true; // ���� ���� ���� ����

}
