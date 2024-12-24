using UnityEngine;

public class DynamicCameraControl : MonoBehaviour
{
    public Transform ball; // 따라갈 공의 Transform
    public float stepHeight = 8f; // 카메라가 이동할 기준 높이
    public float smoothSpeed = 2f; // 카메라 이동 속도
    public float targetOffset = 5f; // 카메라가 목표 높이에 추가할 오프셋
    public float defaultHeight = 0f; // 카메라의 기본 높이

    private bool isCameraUp = false; // 카메라가 올라간 상태인지 확인

    void Update()
    {
        if (ball == null) return;

        // 공이 기준 높이 이상으로 올라가고, 카메라가 아직 올라가지 않은 경우
        if (ball.position.y > stepHeight && !isCameraUp)
        {
            isCameraUp = true; // 상태 전환
            float targetHeight = stepHeight + targetOffset; // 목표 높이 계산
            StartCoroutine(MoveCameraToHeight(targetHeight));
        }

        // 공이 기준 높이 아래로 내려가고, 카메라가 올라간 상태인 경우
        if (ball.position.y < stepHeight && isCameraUp)
        {
            isCameraUp = false; // 상태 전환
            StartCoroutine(MoveCameraToHeight(defaultHeight));
        }
    }

    private System.Collections.IEnumerator MoveCameraToHeight(float targetHeight)
    {
        Vector3 startPosition = transform.position; // 현재 카메라 위치
        Vector3 targetPosition = new Vector3(startPosition.x, targetHeight, startPosition.z); // 목표 위치

        float elapsedTime = 0f;
        float duration = 1f / smoothSpeed; // 카메라 이동에 걸리는 시간

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치를 정확히 설정
        transform.position = targetPosition;
    }
}
