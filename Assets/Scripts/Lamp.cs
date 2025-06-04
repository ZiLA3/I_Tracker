using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] Light pointLight0; // ���� ������Ʈ
    [SerializeField] Light pointLight1; // ���� ������Ʈ

    float defaultIntensity0; // �⺻ ���� ����
    float defaultIntensity1; // �⺻ ���� ����

    bool triggered = false; // Ʈ���� ����

    private void Start()
    {
        defaultIntensity0 = pointLight0.intensity; // �ʱ� ���� ���� ����
        defaultIntensity1 = pointLight1.intensity; // �ʱ� ���� ���� ����
    }

    void Update()
    {
        if (GameManager.Instance.Mission.LightTwinkling)
        {
            pointLight0.intensity = Mathf.PingPong(defaultIntensity0, 1f); // ���� ������ ȿ��
            pointLight1.intensity = Mathf.PingPong(defaultIntensity1, 1f); // ���� ������ ȿ��
        }
        else if (!triggered)
        {
            triggered = true; // Ʈ���� ���� ����

            pointLight0.intensity = defaultIntensity0; // ���� ���� �ʱ�ȭ
            pointLight1.intensity = defaultIntensity1; // ���� ���� �ʱ�ȭ
        }
    }
}
