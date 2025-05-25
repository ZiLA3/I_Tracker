using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public static Monster instance;

    [Header("Move Info")]
    [SerializeField] float moveSpeed;

    CharacterController controller;
    Animator anim;

    // State Boolean variables
    public bool isInSight = false;
    public bool mission = false;
    public bool lightOn = false;
    private bool inDeathZone = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        if (inDeathZone)
            return;

        if (mission || lightOn || !isInSight)
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
            inDeathZone = true;
            anim.SetBool("Idle", false);
            anim.SetBool("Move", false);
            anim.SetBool("Attack", true);
        }
    }
}
