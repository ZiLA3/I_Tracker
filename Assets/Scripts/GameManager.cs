using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] GameObject winUI; // �¸� UI
    [SerializeField] GameObject loseUI; // �й� UI
    [SerializeField] GameObject escUI;
    [SerializeField] TextMeshProUGUI keyCountText; // Ű ���� UI (�߰��� �κ�)
    [SerializeField] float timeToShowUI = 2f; // �¸� UI ǥ�� �ð�

    [Header("Monster")]
    [SerializeField] Monster monster;

    float timer = 0f;

    public event Action KeyCountChanged; // Ű ���� ���� �̺�Ʈ

    public static GameManager Instance { get; private set; }
    public EyeCheck Eye { get; private set; }
    public MissionManager Mission { get; private set; }

    public bool IsGameOver { get; private set; } = false; // ���� ���� ����
    public bool IsGameWin { get; private set; } = false; // ���� �¸� ����
    public bool GameStopped { get; private set; } = false; // ���� �Ͻ� ���� ����

    private bool triggered0 = false; // Ʈ���� ����
    private bool triggered1 = false; // Ʈ���� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

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
        if(Input.GetMouseButtonDown(1) && CameraManager.Instance.CurrentCameraType == CameraType.mainCamera)
        {
            if (escUI.activeSelf)
                Resume();
            else
                Stop();
        }

        // ���� �¸� ���°� true�̰�, Ʈ���Ű� ���� �߻����� �ʾҴٸ�
        if ((IsGameWin || IsGameOver) && !triggered0)
        {
            triggered0 = true;
            timer = timeToShowUI; // �¸� UI ǥ�� �ð� �ʱ�ȭ
        }

        if (triggered0 && !triggered1)
        {
            timer -= Time.deltaTime; // Ÿ�̸� ����

            if (timer <= 0f)
            {
                triggered1 = true; // Ʈ���� ���� �ʱ�ȭ

                if (IsGameWin)
                    winUI.SetActive(true); // �¸� UI Ȱ��ȭ
                else
                    loseUI.SetActive(true); // �й� UI Ȱ��ȭ

            }
        }
    }

    public void Resume()
    {
        escUI.SetActive(false);

        monster.ActiveAnim(true);

        Time.timeScale = 1f; // ���� �簳
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit(); // ���� ����
    }

    private void Stop()
    {
        GameStopped = true; // ���� �Ͻ� ���� ���� ����

        monster.ActiveAnim(false);

        escUI.SetActive(true);

        Time.timeScale = 0f; // ���� �Ͻ� ����
    }

    public bool StopScipt() => IsGameWin || IsGameOver || GameStopped;

    public void SetGameWinTrue() 
    {
        IsGameWin = true; // ���� �¸� ���� ����
        monster.ActiveAnim(false); // ���� �ִϸ��̼� ��Ȱ��ȭ
    }

    public void SetGameOverTrue() => IsGameOver = true; // ���� ���� ���� ����

}
