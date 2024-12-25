using UnityEngine;

public class HammerSwing : MonoBehaviour
{
    // 각도 범위
    public float swingAngle = 45f; // 좌우로 45도
    public float swingSpeed = 2f; // 진동 속도

    private float initialRotationZ; // 초기 Z축 회전 값

    void Start()
    {
        // 초기 Z축 회전을 저장
        initialRotationZ = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        // 진자 운동을 계산 (sin 함수로 움직임 반복)
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;

        // 초기 회전에 상대적으로 각도를 적용
        transform.rotation = Quaternion.Euler(0, 0, initialRotationZ + angle);
    }
}
