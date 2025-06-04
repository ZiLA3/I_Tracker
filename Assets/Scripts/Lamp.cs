using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] Light pointLight0; // 조명 컴포넌트
    [SerializeField] Light pointLight1; // 조명 컴포넌트

    float defaultIntensity0; // 기본 조명 강도
    float defaultIntensity1; // 기본 조명 강도

    bool triggered = false; // 트리거 상태

    private void Start()
    {
        defaultIntensity0 = pointLight0.intensity; // 초기 조명 강도 저장
        defaultIntensity1 = pointLight1.intensity; // 초기 조명 강도 저장
    }

    void Update()
    {
        if (GameManager.Instance.Mission.LightTwinkling)
        {
            pointLight0.intensity = Mathf.PingPong(defaultIntensity0, 1f); // 조명 깜빡임 효과
            pointLight1.intensity = Mathf.PingPong(defaultIntensity1, 1f); // 조명 깜빡임 효과
        }
        else if (!triggered)
        {
            triggered = true; // 트리거 상태 변경

            pointLight0.intensity = defaultIntensity0; // 조명 강도 초기화
            pointLight1.intensity = defaultIntensity1; // 조명 강도 초기화
        }
    }
}
