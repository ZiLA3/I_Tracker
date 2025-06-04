using UnityEngine;

public class Monster : MonoBehaviour
{
    public static Monster Instance;

    [Header("Move Info")]
    [SerializeField] float moveSpeed;

    CharacterController controller;
    Animator anim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        // 초기 애니메이션 상태 설정
        anim.SetBool("Idle", true);
        anim.SetBool("Move", false);
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (GameManager.Instance.IsGameWin) 
        {
            anim.speed = 0;
            return;
        }

        if (GameManager.Instance.IsGameOver)
            return;

        if (GameManager.Instance.Mission.IsInMission || GameManager.Instance.Mission.LightTwinkling || !GameManager.Instance.Eye.EnemyOnSight)
        {
            controller.Move(transform.forward * moveSpeed * Time.deltaTime);

            anim.SetBool("Idle", false);
            anim.SetBool("Move", true);
        }
        else
        {
            controller.Move(Vector3.zero);
            anim.SetBool("Idle", true);
            anim.SetBool("Move", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameManager.Instance.Mission.currentInteractable?.ResetToMainView();

        GameManager.Instance.SetGameOverTrue();

        anim.SetBool("Idle", false);
        anim.SetBool("Move", false);
        anim.SetBool("Attack", true);
    }
}
