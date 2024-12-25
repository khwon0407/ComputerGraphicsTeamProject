using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // ī�޶� ���� ��� (��)
    public float distance = 5.0f; // ī�޶�� �� ���� �Ÿ�
    public float height = 2.0f; // ī�޶� ����
    public float rotationSpeed = 5.0f; // ���콺�� ȸ���ϴ� �ӵ�

    private float currentRotationAngle;
    private float desiredRotationAngle;
    private Quaternion currentRotation;

    void LateUpdate()
    {
        // ���콺 �Է����� ī�޶� ȸ��
        float horizontalInput = Input.GetAxis("Mouse X");
        desiredRotationAngle += horizontalInput * rotationSpeed;

        // ���� ȸ������ ��ǥ ȸ���� ������ ����
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, desiredRotationAngle, Time.deltaTime * rotationSpeed);

        // ī�޶� ��ġ ���
        currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        Vector3 position = target.position - (currentRotation * Vector3.forward * distance);
        position.y = target.position.y + height;

        // ī�޶� ��ġ �� ��� �ٶ󺸱�
        transform.position = position;
        transform.LookAt(target);
    }
}
