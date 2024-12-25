using UnityEngine;

public class HammerSwing : MonoBehaviour
{
    // ���� ����
    public float swingAngle = 45f; // �¿�� 45��
    public float swingSpeed = 2f; // ���� �ӵ�

    private float initialRotationZ; // �ʱ� Z�� ȸ�� ��

    void Start()
    {
        // �ʱ� Z�� ȸ���� ����
        initialRotationZ = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        // ���� ��� ��� (sin �Լ��� ������ �ݺ�)
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;

        // �ʱ� ȸ���� ��������� ������ ����
        transform.rotation = Quaternion.Euler(0, 0, initialRotationZ + angle);
    }
}
