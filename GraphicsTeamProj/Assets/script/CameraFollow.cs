using UnityEngine;

public class DynamicCameraControl : MonoBehaviour
{
    public Transform ball; // 따라갈 공의 Transform
    public float stepHeight = 8f; // 카메라가 이동할 기준 높이
    public float smoothSpeedY = 2f; // Y축 이동 속도
    public float smoothSpeedZ = 2f; // Z축 이동 속도
    public float yOffset = 5f; // Y축 목표 높이에 추가할 오프셋
    public float defaultHeight = 0f; // 카메라의 기본 높이
    public float zFollowDistance = 10f; // 공과 카메라의 Z축 거리

    private bool isCameraUp = false; // 카메라가 올라간 상태인지 확인

    void Update()
    {
        if (ball == null) return;

        // Y축 이동
        if (ball.position.y > stepHeight && !isCameraUp)
        {
            isCameraUp = true;
            StartCoroutine(MoveCameraY(defaultHeight + yOffset));
        }
        else if (ball.position.y <= stepHeight && isCameraUp)
        {
            isCameraUp = false;
            StartCoroutine(MoveCameraY(defaultHeight));
        }

        // Z축 따라가기
        MoveCameraZ(ball.position.z);
    }

    private System.Collections.IEnumerator MoveCameraY(float targetHeight)
    {
        float startY = transform.position.y;
        float elapsedTime = 0f;
        float duration = 1f / smoothSpeedY;

        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startY, targetHeight, elapsedTime / duration);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
    }

    private void MoveCameraZ(float targetZ)
    {
        float targetCameraZ = targetZ - zFollowDistance; // 공과 카메라의 Z축 간격 유지
        float currentZ = transform.position.z;
        float newZ = Mathf.Lerp(currentZ, targetCameraZ, Time.deltaTime * smoothSpeedZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }
}
