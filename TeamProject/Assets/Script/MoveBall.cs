using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    [SerializeField]
    float speed = 5f; // 이동 속도

    [SerializeField]
    float jumpForce = 10f; // 점프 힘
    Rigidbody rb; // Rigidbody 컴포넌트

    bool isGrounded = true; // 공이 바닥에 닿아있는지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        rb.isKinematic = false; // 중력과 물리 엔진 영향 받도록 설정
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        // 키 입력을 받아 이동 방향 계산
        if (Input.GetKey(KeyCode.W)) { move += Vector3.forward; }
        if (Input.GetKey(KeyCode.S)) { move += Vector3.back; }
        if (Input.GetKey(KeyCode.A)) { move += Vector3.left; }
        if (Input.GetKey(KeyCode.D)) { move += Vector3.right; }

        // 물리적 이동 (속도는 그대로, 힘을 0으로 설정)
        if (move != Vector3.zero)
        {
            // X, Z 방향으로만 이동하고 Y 방향 속도는 유지
            rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
        }
        else
        {
            // 키 입력이 없으면 속도 0으로 설정 (X, Z 방향)
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    // 충돌 시 바닥에 닿으면 isGrounded를 true로 설정
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true; // 바닥에 닿았으면 점프 가능
        }

        // 충돌 시 튕겨나가도록 힘을 추가
        // 충돌 후 위로 튕겨 나가도록 설정
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위로 힘을 추가하여 튕겨 오르게 함
        }

        // 충돌 후 힘을 0으로 설정하여 튕겨나가지 않게 함
        rb.velocity = new Vector3(0, rb.velocity.y, 0); // X, Z 방향의 속도를 0으로 설정
    }
}
