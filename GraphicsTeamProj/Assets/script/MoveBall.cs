using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MoveBall : MonoBehaviour
{
    [SerializeField] private int jumpBoostCount = 3;
    private int remainingJumpBoosts = 0;
    private bool isJumpBoostActive = false;
    private PowerUpManager powerUpManager;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float maxJumpForce = 25f;
    [SerializeField] private float fallThreshold = -10f;

    private readonly float originalSpeed = 5f;
    private readonly float originalJumpForce = 7f;

    private Rigidbody rb;

    private bool isGrounded = true;
    private bool isGameActive = true;

    private HashSet<Collider> groundColliders = new HashSet<Collider>();

    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject winText;

    [SerializeField] private Color powerUpColor = Color.red;
    [SerializeField] private Color jumpBoostColor = Color.blue;
    private Color originalColor;
    private Renderer ballRenderer;
    private bool hasJumped = false;

    // AudioClips for various events
    [SerializeField] private AudioClip gameStartSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip springBoardSound;

    [SerializeField] private AudioSource bgmAudioSource; // 배경음악 AudioSource
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerUpManager = GetComponent<PowerUpManager>();
        ballRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        originalColor = ballRenderer.material.color;

        gameOverText?.SetActive(false);
        winText?.SetActive(false);

        PlaySound(gameStartSound, 0.5f); // 게임 시작 사운드 재생

        // 배경음악 재생
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }
    }

    void Update()
    {
        if (!isGameActive) return;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) { move += Vector3.forward; }
        if (Input.GetKey(KeyCode.S)) { move += Vector3.back; }
        if (Input.GetKey(KeyCode.A)) { move += Vector3.left; }
        if (Input.GetKey(KeyCode.D)) { move += Vector3.right; }

        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);

        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Ball fell below the threshold!");
            GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUpBlock"))
        {
            PlaySound(powerUpSound); // 파워업 사운드
            powerUpManager.AddPowerUp(new PowerUp
            {
                Name = "SpeedBoost",
                Duration = 5f,
                Color = powerUpColor,
                ActivateEffect = () => speed *= 2,
                DeactivateEffect = () => speed = originalSpeed
            });

            Destroy(other.gameObject);
        }

        if (other.CompareTag("JumpBoostBlock"))
        {
            PlaySound(powerUpSound); // 파워업 사운드
            powerUpManager.AddPowerUp(new PowerUp
            {
                Name = "JumpBoost",
                Duration = 5f,
                Color = jumpBoostColor,
                ActivateEffect = () => jumpForce *= 2f,
                DeactivateEffect = () => jumpForce = originalJumpForce
            });

            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
{
    Debug.Log($"Collision Detected: {collision.collider.name}");

    if (collision.collider.CompareTag("SpringBoard"))
    {
        Debug.Log("SpringBoard detected. Activating...");
        PlaySound(springBoardSound,2f); // 점프대 사운드
        ActivateSpringBoard(collision.collider.transform.parent != null ? collision.collider.transform.parent.gameObject : collision.gameObject);
        return; // 점프대에서는 HandleJump를 실행하지 않음
    }

    foreach (ContactPoint contact in collision.contacts)
    {
        if (Vector3.Angle(contact.normal, Vector3.up) < 45)
        {
            if (!groundColliders.Contains(collision.collider))
            {
                groundColliders.Add(collision.collider);
            }

            if (!isGrounded)
            {
                isGrounded = true;
                Debug.Log("Grounded.");
            }

            if (!hasJumped)
            {
                hasJumped = true;
                HandleJump(); // 일반 점프 실행
            }
        }
    }

    if (collision.collider.CompareTag("DangerBlock"))
    {
        GameOver();
    }

    if (collision.collider.CompareTag("GoalBlock"))
    {
        GameClear();
    }
}


    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"Collision Exit Detected: {collision.collider.name}");
        groundColliders.Remove(collision.collider);

        if (groundColliders.Count == 0)
        {
            isGrounded = false;
            hasJumped = false;
            Debug.Log("No longer grounded.");
        }
    }

    private void HandleJump()
{
    Debug.Log("Normal jump performed.");
    PlaySound(jumpSound,3f); // 점프 사운드

    if (isJumpBoostActive && remainingJumpBoosts > 0)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float boostedJumpForce = jumpForce * 1.5f; // 점프 부스트 계산
        float finalJumpForce = Mathf.Min(boostedJumpForce, maxJumpForce);
        rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);
        remainingJumpBoosts--;

        if (remainingJumpBoosts <= 0)
        {
            DeactivateJumpBoost();
        }
    }
    else
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 일반 점프
    }
}


    private void PlaySound(AudioClip clip, float volumeScale = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            // volumeScale을 사용해 볼륨을 조정 (0.0 ~ 1.0)
            audioSource.PlayOneShot(clip, volumeScale);
        }
    }


    private void DeactivateJumpBoost()
    {
        Debug.Log("Jump Boost Deactivated!");
        isJumpBoostActive = false;
        remainingJumpBoosts = 0;

        if (ballRenderer != null)
        {
            ballRenderer.material.color = originalColor;
        }
    }

    private void ActivateSpringBoard(GameObject springBoard)
{
    Debug.Log("Activating SpringBoard...");

    float springForce = 15f; // 점프대에서의 고정된 힘
    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 Y축 속도를 초기화
    rb.AddForce(Vector3.up * springForce, ForceMode.Impulse); // 고정된 힘으로 점프

    Animator animator = springBoard.GetComponent<Animator>();
    if (animator != null)
    {
        Debug.Log("Triggering Animator.");
        animator.SetTrigger("Activate");
    }
    else
    {
        Debug.LogError($"Animator is missing on {springBoard.name}");
    }

    isGrounded = false; // 점프대에서는 항상 공중 상태로 전환
    hasJumped = true; // 점프 상태 기록
}

    private void GameOver()
{
    if (!isGameActive) return;

    isGameActive = false;
    rb.velocity = Vector3.zero;
    rb.isKinematic = true;
    gameOverText.SetActive(true);

    // 배경음악 정지
    if (bgmAudioSource != null)
    {
        StopBGM(); // 배경음악 정지
    }

    PlaySound(gameOverSound, 0.5f); // 게임 오버 사운드
    StartCoroutine(RestartGame());
}

private void GameClear()
{
    if (!isGameActive) return;

    isGameActive = false;
    rb.velocity = Vector3.zero;
    rb.isKinematic = true;
    winText.SetActive(true);

    // 배경음악 정지
    if (bgmAudioSource != null)
    {
        StopBGM(); // 배경음악 정지
    }

    PlaySound(winSound, 0.5f); // 승리 사운드
    StartCoroutine(LoadNextStage());
}

private void StopBGM()
{
    if (bgmAudioSource.isPlaying)
    {
        bgmAudioSource.Stop(); // 배경음악 완전히 정지
    }
}


    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = false;

        // 배경음악 다시 재생
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadNextStage()
    {
        yield return new WaitForSeconds(2f);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 배경음악 다시 재생
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
