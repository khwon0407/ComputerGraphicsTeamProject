using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리
using TMPro; // TextMeshPro 네임스페이스 추가

public class MoveBall : MonoBehaviour
{
    [SerializeField]
    float speed = 5f; // 기본 이동 속도

    [SerializeField]
    float jumpForce = 10f; // 점프 힘
    Rigidbody rb; // Rigidbody 컴포넌트

    bool isGrounded = true; // 공이 바닥에 닿아있는지 여부
    bool isPoweredUp = false; // 특수 능력 활성화 여부
    bool isGameActive = true; // 게임 진행 상태 플래그

    float powerUpDuration = 3f; // 특수 능력 지속 시간
    float jumpBoostDuration = 3f; // 점프 강화 지속 시간

    [SerializeField] GameObject gameOverText; // "Game Over" 메시지
    [SerializeField] GameObject winText;     // "You Win!" 메시지

    [SerializeField] Color powerUpColor = Color.red; // PowerUp 시 변경할 색상
    [SerializeField] Color jumpBoostColor = Color.blue; // Jump Boost 시 변경할 색상
    private Color originalColor; // 원래 색상
    private Renderer ballRenderer; // 공의 Renderer 컴포넌트

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        rb.isKinematic = false; // 중력과 물리 엔진 영향 받도록 설정

        // Renderer 및 원래 색상 초기화
        ballRenderer = GetComponent<Renderer>();
        originalColor = ballRenderer.material.color;

        // UI 초기화
        if (gameOverText != null) gameOverText.SetActive(false);
        if (winText != null) winText.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return; // 게임 오버/승리 상태에서는 입력 무시

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) { move += Vector3.forward; }
        if (Input.GetKey(KeyCode.S)) { move += Vector3.back; }
        if (Input.GetKey(KeyCode.A)) { move += Vector3.left; }
        if (Input.GetKey(KeyCode.D)) { move += Vector3.right; }

        if (move != Vector3.zero)
        {
            rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.collider.CompareTag("Ground"))
    {
        isGrounded = true;
    }

    if (isGrounded)
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // DangerBlock: 충돌 시 게임 종료
    if (collision.collider.CompareTag("DangerBlock"))
    {
        GameOver();
    }

    // PowerUpBlock: 충돌 시 특수 능력 부여
    if (collision.collider.CompareTag("PowerUpBlock"))
    {
        StartCoroutine(PowerUp());
        Destroy(collision.gameObject); // 충돌한 PowerUp 블록 제거
    }

    // JumpBoostBlock: 충돌 시 점프 강화
    if (collision.collider.CompareTag("JumpBoostBlock"))
    {
        StartCoroutine(JumpBoost());
        Destroy(collision.gameObject); // 충돌한 JumpBoost 블록 제거
    }

    // SpringBoard: 밟으면 즉시 높은 점프
    if (collision.collider.CompareTag("SpringBoard"))
    {
        ActivateSpringBoard(collision.gameObject);
    }

    // GoalBlock: 충돌 시 게임 성공
    if (collision.collider.CompareTag("GoalBlock"))
    {
        GameClear();
    }
}

private void ActivateSpringBoard(GameObject springBoard)
{
    Debug.Log("SpringBoard Activated!");

    // 공에 즉시 상향 힘 추가
    float springForce = 15f; // 점프대에서 추가되는 힘
    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 Y축 속도를 초기화
    rb.AddForce(Vector3.up * springForce, ForceMode.Impulse);

    // 시각적 효과 (예: 스프링이 눌리는 애니메이션)
    Animator animator = springBoard.GetComponent<Animator>();
    if (animator != null)
    {
        animator.SetTrigger("Activate");
    }
}

    private void GameOver()
    {
        Debug.Log("Game Over!");
        isGameActive = false; // 입력 차단
        rb.velocity = Vector3.zero; // 물리 속도 초기화
        rb.isKinematic = true; // Rigidbody 비활성화
        gameOverText.SetActive(true); // "Game Over" 메시지 표시
        StartCoroutine(RestartGame()); // 게임 재시작
    }

    private void GameClear()
    {
        Debug.Log("You Win!");
        isGameActive = false; // 입력 차단
        rb.velocity = Vector3.zero; // 물리 속도 초기화
        rb.isKinematic = true; // Rigidbody 비활성화
        winText.SetActive(true); // "You Win!" 메시지 표시
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        rb.isKinematic = false; // Rigidbody 다시 활성화
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 리로드
    }

    private IEnumerator PowerUp()
    {
        Debug.Log("Power Up Activated!");
        isPoweredUp = true;
        float originalSpeed = speed;

        // 속도 증가
        speed *= 3;

        // 색상 변경
        if (ballRenderer != null)
        {
            ballRenderer.material.color = powerUpColor;
        }

        yield return new WaitForSeconds(powerUpDuration); // 지속 시간 동안 대기

        // 속도와 색상 복구
        speed = originalSpeed;

        if (ballRenderer != null)
        {
            ballRenderer.material.color = originalColor;
        }

        isPoweredUp = false;
        Debug.Log("Power Up Deactivated!");
    }

    private IEnumerator JumpBoost()
    {
        Debug.Log("Jump Boost Activated!");
        float originalJumpForce = jumpForce;

        // 점프 힘 증가
        jumpForce *= 1.5f;

        // 색상 변경
        if (ballRenderer != null)
        {
            ballRenderer.material.color = jumpBoostColor;
        }

        yield return new WaitForSeconds(jumpBoostDuration); // 지속 시간 동안 대기

        // 점프 힘과 색상 복구
        jumpForce = originalJumpForce;

        if (ballRenderer != null)
        {
            ballRenderer.material.color = originalColor;
        }

        Debug.Log("Jump Boost Deactivated!");
    }
}