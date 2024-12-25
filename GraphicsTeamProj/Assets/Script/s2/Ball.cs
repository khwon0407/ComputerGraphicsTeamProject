using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    #region MovementSettings
    [Header("Movement Settings")]
    public float bounceSpeed = 15f;
    public float moveSpeed = 12f;
    public float maxSpeed = 5f;
    public float deceleration = 4f;
    public float bounciness = 0.8f;
    public float gravityScale = 2.8f;

    public float originalMoveSpeed { get; private set; }
    public float originalBounceSpeed { get; private set; }
    public float originalMaxSpeed { get; private set; }
    #endregion

    #region CameraReference
    [Header("Camera Reference")]
    public Transform cameraTransform;
    #endregion

    #region DebugSettings
    [Header("Debug Settings")]
    public bool drawDebugForces = true;
    public float debugForceScale = 0.2f;
    public Color debugForceColor = Color.red;

    private Vector3 debugLastMoveForce;
    #endregion

    #region GameGimmickSettings
    [Header("Game Gimmicks")]
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject winText;
    [SerializeField] private AudioSource bgmAudioSource;
    private AudioSource audioSource;

    [SerializeField] private AudioClip gameStartSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip springBoardSound;

    private bool isGameActive = true;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (gameOverText != null) gameOverText.SetActive(false);
        if (winText != null) winText.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && gameStartSound != null)
        {
            PlaySound(gameStartSound, 1f);
        }

        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        originalMaxSpeed = maxSpeed;
        originalMoveSpeed = moveSpeed;
        originalBounceSpeed = bounceSpeed;
    }

    void Update()
    {
        if (!isGameActive) return;

        if (transform.position.y < fallThreshold)
        {
            GameOver();
        }
    }

    void FixedUpdate()
    {
        if (!isGameActive) return;

        rb.AddForce(Physics.gravity * (gravityScale - 1f), ForceMode.Acceleration);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movementForce = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            Vector3 moveDir = (camForward * vertical) + (camRight * horizontal);

            movementForce = moveDir.normalized * moveSpeed;
        }
        else
        {
            Vector3 moveDir = new Vector3(horizontal, 0f, vertical).normalized;
            movementForce = moveDir * moveSpeed;
        }

        rb.AddForce(movementForce, ForceMode.Acceleration);
        debugLastMoveForce = movementForce;

        Vector3 hVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (hVelocity.magnitude > maxSpeed)
        {
            hVelocity = hVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(hVelocity.x, rb.velocity.y, hVelocity.z);
        }

        if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
        {
            float newX = Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime);
            float newZ = Mathf.MoveTowards(rb.velocity.z, 0, deceleration * Time.deltaTime);
            rb.velocity = new Vector3(newX, rb.velocity.y, newZ);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DangerBlock"))
        {
            GameOver();
            return;
        }

        if (collision.collider.CompareTag("GoalBlock"))
        {
            GameClear();
            return;
        }

        if (collision.collider.CompareTag("SpringBoard"))
        {
            GameObject springBoard = collision.collider.transform.parent != null 
                ? collision.collider.transform.parent.gameObject 
                : collision.gameObject;

            Animator animator = springBoard.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Activate");
            }

            PlaySound(springBoardSound, 2f);

            Debug.Log($"SpringBoard - Before: Velocity Y = {rb.velocity.y:F2}");

            rb.velocity = new Vector3(
                rb.velocity.x,
                bounceSpeed * 2f,
                rb.velocity.z
            );
            
            Debug.Log($"SpringBoard - After: Velocity Y = {rb.velocity.y:F2}");
            return;
        }

        Vector3 contactNormal = collision.contacts[0].normal;
        float angle = Vector3.Angle(contactNormal, Vector3.up);

        if (angle < 45f)
        {
            Debug.Log($"Normal Bounce - Before: Velocity Y = {rb.velocity.y:F2}");
            
            Vector3 currentVelocity = rb.velocity;
            rb.velocity = new Vector3(
                currentVelocity.x,
                bounceSpeed,
                currentVelocity.z
            );
            
            Debug.Log($"Normal Bounce - After: Velocity Y = {rb.velocity.y:F2}");
        }

        // 공이 EnemyTrigger표면 (NavMesh 타일)에 닿으면 적 Agent를 활성화
        if (collision.gameObject.CompareTag("EnemyTrigger"))
        {
            TriggerEnemyController[] enemies = FindObjectsOfType<TriggerEnemyController>();
            foreach (var enemy in enemies)
            {
                enemy.StartChasing();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        BallPowerUpManager powerUpManager = GetComponent<BallPowerUpManager>();
        
        if (other.CompareTag("PowerUpBlock"))
        {
            PlaySound(powerUpSound);
            powerUpManager.AddSpeedBoost(2f, 5f);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("JumpBoostBlock"))
        {
            PlaySound(powerUpSound);
            powerUpManager.AddBounceBoost(1.5f, 5f);
            Destroy(other.gameObject);
        }
    }

    #region GameOver / GameClear
    private void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        gameOverText?.SetActive(true);

        StopBGM();
        PlaySound(gameOverSound, 1f);

        StartCoroutine(RestartGame());
    }

    private void GameClear()
    {
        if (!isGameActive) return;
        isGameActive = false;

        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
        }
        rb.isKinematic = true;
        winText?.SetActive(true);

        StopBGM();
        PlaySound(winSound, 0.5f);

        StartCoroutine(LoadNextStage());
    }
    #endregion

    #region SceneControlCoroutines
    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = false;
        if (bgmAudioSource != null) bgmAudioSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadNextStage()
    {
        yield return new WaitForSeconds(2f);
        if (bgmAudioSource != null) bgmAudioSource.Play();
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(currentIndex + 1);
        else
            SceneManager.LoadScene(0);
    }
    #endregion

    #region AudioHelpers
    private void PlaySound(AudioClip clip, float volumeScale = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volumeScale);
        }
    }

    private void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }
    #endregion

    #region DebugGizmos
    void OnDrawGizmos()
    {
        if (!drawDebugForces) return;
        if (!Application.isPlaying) return;

        Vector3 ballPos = transform.position;
        Vector3 forceDir = debugLastMoveForce * debugForceScale;

        Gizmos.color = debugForceColor;
        Gizmos.DrawLine(ballPos, ballPos + forceDir);
        Gizmos.DrawSphere(ballPos + forceDir, 0.05f);
    }
    #endregion
}
