using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 대상 (공)
    public float distance = 5.0f; // 카메라와 공 사이 거리
    public float height = 2.0f; // 카메라 높이
    public float rotationSpeed = 5.0f; // 마우스로 회전하는 속도

    private float currentRotationAngle;
    private float desiredRotationAngle;
    private Quaternion currentRotation;

    void LateUpdate()
    {
        // 마우스 입력으로 카메라 회전
        float horizontalInput = Input.GetAxis("Mouse X");
        desiredRotationAngle += horizontalInput * rotationSpeed;

        // 현재 회전값과 목표 회전값 사이의 보간
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, desiredRotationAngle, Time.deltaTime * rotationSpeed);

        // 카메라 위치 계산
        currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        Vector3 position = target.position - (currentRotation * Vector3.forward * distance);
        position.y = target.position.y + height;

        // 카메라 위치 및 대상 바라보기
        transform.position = position;
        transform.LookAt(target);
    }
}
