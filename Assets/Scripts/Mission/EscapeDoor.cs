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
        if (GameManager.Instance.Mission.currentInteractable != this || !GameManager.Instance.Mission.IsInMission)
            return;

        if (isDoorOpen)
            door.transform.localRotation = Quaternion.Slerp(door.transform.localRotation, Quaternion.Euler(doorLastEulerAngle), Time.deltaTime * rotateSpeed);

        if (succeeded)
            return; // �̹� �� ���� ������ Ʈ���ŵǾ����� ������Ʈ ����

        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (FaceLandmark.HandFolds == 15 && GameManager.Instance.Mission.PlayerKeyCount == 3)
        {
            Invoke(nameof(SetDoorOpen), delayTimeToRotate); // ���� ��� ���¸� true�� ����

            anim.SetTrigger("Catch");

            succeeded = true;
            inMissionUI.SetActive(false); // �̼� UI ��Ȱ��ȭ

            GameManager.Instance.SetGameWinTrue();
        }
    }

    private void SetDoorOpen() => isDoorOpen = true;

    public override void Interact()
    {
        base.Interact();

        GameManager.Instance.Mission.SetMissionActive(true);

        SetHandActive(true);

        CameraManager.Instance.ToggleCamera(CameraType.doorCamera);
    }

    public override void ResetToMainView()
    {
        base.ResetToMainView();

        GameManager.Instance.Mission.SetMissionActive(false);

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);
    }
}
