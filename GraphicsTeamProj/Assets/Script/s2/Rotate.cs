using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f; // 초당 회전 속도 (각도)

    void Update()
    {
        // Y 축을 기준으로 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
