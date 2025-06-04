using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] GameObject winUI; // 승리 UI
    [SerializeField] GameObject loseUI; // 패배 UI
    [SerializeField] GameObject escUI;
    [SerializeField] TextMeshProUGUI keyCountText; // 키 개수 UI (추가된 부분)
    [SerializeField] float timeToShowUI = 2f; // 승리 UI 표시 시간

    [Header("Monster")]
    [SerializeField] Monster monster;

    float timer = 0f;

    public event Action KeyCountChanged; // 키 개수 변경 이벤트

    public static GameManager Instance { get; private set; }
    public EyeCheck Eye { get; private set; }
    public MissionManager Mission { get; private set; }

    public bool IsGameOver { get; private set; } = false; // 게임 오버 상태
    public bool IsGameWin { get; private set; } = false; // 게임 승리 상태
    public bool GameStopped { get; private set; } = false; // 게임 일시 정지 상태

    private bool triggered0 = false; // 트리거 상태
    private bool triggered1 = false; // 트리거 상태

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
        KeyCountChanged?.Invoke(); // 키 개수 변경 이벤트 호출
    }

    private void UpdateKeyCountText()
    {
        keyCountText.text = $"Key Counts : {Mission.PlayerKeyCount}"; // 키 개수 UI 업데이트
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

        // 게임 승리 상태가 true이고, 트리거가 아직 발생하지 않았다면
        if ((IsGameWin || IsGameOver) && !triggered0)
        {
            triggered0 = true;
            timer = timeToShowUI; // 승리 UI 표시 시간 초기화
        }

        if (triggered0 && !triggered1)
        {
            timer -= Time.deltaTime; // 타이머 감소

            if (timer <= 0f)
            {
                triggered1 = true; // 트리거 상태 초기화

                if (IsGameWin)
                    winUI.SetActive(true); // 승리 UI 활성화
                else
                    loseUI.SetActive(true); // 패배 UI 활성화

            }
        }
    }

    public void Resume()
    {
        escUI.SetActive(false);

        monster.ActiveAnim(true);

        Time.timeScale = 1f; // 게임 재개
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit(); // 게임 종료
    }

    private void Stop()
    {
        GameStopped = true; // 게임 일시 정지 상태 설정

        monster.ActiveAnim(false);

        escUI.SetActive(true);

        Time.timeScale = 0f; // 게임 일시 정지
    }

    public bool StopScipt() => IsGameWin || IsGameOver || GameStopped;

    public void SetGameWinTrue() 
    {
        IsGameWin = true; // 게임 승리 상태 설정
        monster.ActiveAnim(false); // 몬스터 애니메이션 비활성화
    }

    public void SetGameOverTrue() => IsGameOver = true; // 게임 오버 상태 설정

}
