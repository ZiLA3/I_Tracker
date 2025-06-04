using UnityEngine;

public class EscapeDoor : MissionObject
{
    [Header("Door Setting")]
    [SerializeField] GameObject door;
    [Space]
    [SerializeField] Vector3 doorLastEulerAngle;
    [SerializeField] float rotateSpeed = 5f;
    [SerializeField] float delayTimeToRotate = .3f;

    bool isDoorOpen = false;
    bool succeeded = false;

    Animator anim;

    private void Start()
    {
        anim = hand.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Player.Instance.Mission.currentInteractable != this || !Player.Instance.Mission.IsInMission)
            return;

        if (isDoorOpen)
            door.transform.localRotation = Quaternion.Slerp(door.transform.localRotation, Quaternion.Euler(doorLastEulerAngle), Time.deltaTime * rotateSpeed);

        if (succeeded)
            return; // 이미 문 열기 동작이 트리거되었으면 업데이트 중지

        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (FaceLandmark.HandFolds == 15 && Player.Instance.Mission.PlayerKeyCount == 3)
        {
            Invoke(nameof(SetDoorOpen), delayTimeToRotate); // 레버 당김 상태를 true로 설정

            anim.SetTrigger("Catch");

            succeeded = true;
            inMissionUI.SetActive(false); // 미션 UI 비활성화

            Player.Instance.SetGameWinTrue();
        }
    }

    private void SetDoorOpen() => isDoorOpen = true;

    public override void Interact()
    {
        base.Interact();

        Player.Instance.Mission.SetMissionActive(true);

        SetHandActive(true);

        CameraManager.Instance.ToggleCamera(CameraType.doorCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        Player.Instance.Mission.SetMissionActive(false);

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);
    }
}
