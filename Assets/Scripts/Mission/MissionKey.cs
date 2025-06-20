using UnityEngine;

public enum KeyType
{
    Black,
    Red,
    Blue,
}

public class MissionKey : MissionObject
{
    [SerializeField] KeyType keyType; // 키 타입 (Black, Red, Blue)
    [SerializeField] float delayTime = .3f; // 손 동작을 보기 위한 딜레이

    bool succeeded = false; // 성공 여부 -> 성공했을 때 오작동을 막기 위한 변수

    Animator anim;

    private void Start()
    {
        anim = hand.GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.Mission.currentInteractable != this || !GameManager.Instance.Mission.IsInMission)
            return;

        if (succeeded)
            return; // 이미 키 잡기 동작이 트리거되었으면 업데이트 중지

        if (Input.GetMouseButtonDown(1))
            ResetToMainView();

        if (FaceLandmark.HandFolds == 15)
        {
            succeeded = true; // 키 잡기 동작이 트리거됨

            anim.SetTrigger("Catch");
            Invoke("SucceedMission", delayTime); // 키를 잡았을 때 미션 성공
        }
    }

    public override void Interact()
    {
        base.Interact();

        SetHandActive(true);

        // 키 타입에 따라 카메라 전환
        switch (keyType)
        {
            case KeyType.Black:
                CameraManager.Instance.ToggleCamera(CameraType.blackKeyCamera);
                break;
            case KeyType.Red:
                CameraManager.Instance.ToggleCamera(CameraType.redKeyCamera);
                break;
            case KeyType.Blue:
                CameraManager.Instance.ToggleCamera(CameraType.blueKeyCamera);
                break;
        }
    }

    public override void ResetToMainView()
    {
        GameManager.Instance.Mission.SetMissionActive(false);

        if (inMissionUI != null)
            inMissionUI.SetActive(false);

        SetHandActive(false);

        base.ResetToMainView();
    }

    public override void SucceedMission()
    {
        ResetToMainView();

        GameManager.Instance.UpdateKey();

        transform.gameObject.SetActive(false); // 키 오브젝트 비활성화
    }
}
