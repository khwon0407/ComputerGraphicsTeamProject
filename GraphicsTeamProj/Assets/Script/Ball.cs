using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PowerUpManager))]
public class BallGameController : MonoBehaviour
{
    private Rigidbody rb;
    private PowerUpManager powerUpManager;

    #region Movement_Parameters
    [Header("Movement Settings (from ball.cs)")]
    [Tooltip("충돌 시 위로 튀어오르는 속도")]
    public float bounceSpeed = 14.9f;
    
    [Tooltip("WASD 입력에 따른 이동 속도")]
    public float moveSpeed   = 11.41f;
    
    [Tooltip("XZ 평면에서의 최대 이동 속도")]
    public float maxSpeed    = 4.32f;
    
    [Tooltip("WASD 입력이 없을 때 감속되는 속도")]
    public float deceleration = 4.46f;
    
    [Tooltip("충돌 후 반사되는 탄성 계수 (x/z 축 방향)")]
    public float bounciness  = 0.8f;
    
    [Tooltip("중력 가속도 배율 (1이면 Unity 기본 중력)")]
    public float gravityScale = 2.78f;
    #endregion

    #region Camera_Reference
    [Header("Camera Reference")]
    [Tooltip("카메라 Transform (없으면 Camera.main 자동 할당)")]
    public Transform cameraTransform;
    #endregion

    #region Jump_Parameters
    [Header("Jump / Ground Settings (from MoveBall.cs)")]
    [Tooltip("점프력 (Space, 지면 접지 상태일 때)")]
    public float jumpForce = 7f;
    [Tooltip("최대 점프력 상한")]
    public float maxJumpForce = 25f;

    [Tooltip("접지 판정에 사용할 법선 각도 임계값 (예: 45도 이하면 '지면' 취급)")]
    public float groundAngleThreshold = 45f;
    
    private bool isGrounded = false;
    private HashSet<Collider> groundColliders = new HashSet<Collider>();
    #endregion

    #region PowerUp_Parameters
    #endregion

    #region Game_Flow
    [Header("Game Flow UI")]
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject winText;

    [Tooltip("낙사 처리 기준 높이")]
    public float fallThreshold = -10f;
    
    private bool isGameActive = true;
    #endregion

    #region Debug_Force
    [Header("Debug Settings")]
    [Tooltip("공에 가해진 이동 힘 시각화 여부")]
    public bool drawDebugForces = true;
    [Tooltip("디버그 선 길이 보정")]
    public float debugForceScale = 0.2f;
    [Tooltip("디버그 선 색상")]
    public Color debugForceColor = Color.red;
    
    private Vector3 debugLastMoveForce;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        powerUpManager = GetComponent<PowerUpManager>();
        if (powerUpManager == null)
        {
            Debug.LogWarning("PowerUpManager가 없습니다. 파워업 기능이 동작하지 않을 수 있습니다.");
        }

        if (gameOverText) gameOverText.SetActive(false);
        if (winText) winText.SetActive(false);

        isGameActive = true;
    }

    void FixedUpdate()
    {
        if (!isGameActive) return;

        rb.AddForce(Physics.gravity * (gravityScale - 1f), ForceMode.Acceleration);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput   = Input.GetAxis("Vertical");

        Vector3 movementForce = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 camRight   = Vector3.ProjectOnPlane(cameraTransform.right,   Vector3.up).normalized;

            Vector3 movement = (camForward * verticalInput) + (camRight * horizontalInput);
            movement = movement.normalized;
            movementForce = movement * moveSpeed;
        }
        else
        {
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            movementForce = movement * moveSpeed;
        }

        rb.AddForce(movementForce, ForceMode.Acceleration);
        debugLastMoveForce = movementForce;

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }

        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            float newX = Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime);
            float newZ = Mathf.MoveTowards(rb.velocity.z, 0, deceleration * Time.deltaTime);
            rb.velocity = new Vector3(newX, rb.velocity.y, newZ);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); 
            float finalJumpForce = Mathf.Min(jumpForce, maxJumpForce);
            rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Ball fell below the threshold => GameOver");
            GameOver();
        }
    }

    #region Collision_Triggers

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("SpringBoard"))
        {
            ActivateSpringBoard(collision.collider.gameObject);
            return;
        }

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

        bool isGroundLike = false; 
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < groundAngleThreshold)
            {
                groundColliders.Add(collision.collider);
                isGroundLike = true;
            }
        }

        if (isGroundLike)
        {
            isGrounded = true;
            return;
        }

        if (collision.contacts.Length > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 currentVelocity = rb.velocity;

            Vector3 reflectedVelocity = Vector3.Reflect(currentVelocity, normal);

            Vector3 newVelocity = new Vector3(
                reflectedVelocity.x * bounciness,
                bounceSpeed,
                reflectedVelocity.z * bounciness
            );

            rb.velocity = newVelocity;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (groundColliders.Contains(collision.collider))
        {
            groundColliders.Remove(collision.collider);
        }

        if (groundColliders.Count == 0)
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isGameActive) return;
        if (powerUpManager == null) return;

        if (other.CompareTag("PowerUpBlock"))
        {
            powerUpManager.AddPowerUp(new PowerUp
            {
                Name = "SpeedBoost",
                Duration = 5f,
                Color = Color.red,
                ActivateEffect   = () => moveSpeed *= 2,
                DeactivateEffect = () => moveSpeed = 11.41f
            });
            Destroy(other.gameObject);
        }

        if (other.CompareTag("JumpBoostBlock"))
        {
            powerUpManager.AddPowerUp(new PowerUp
            {
                Name = "JumpBoost",
                Duration = 5f,
                Color = Color.blue,
                ActivateEffect   = () => jumpForce *= 1.5f,
                DeactivateEffect = () => jumpForce = 7f
            });
            Destroy(other.gameObject);
        }
    }
    #endregion

    #region SpringBoard
    private void ActivateSpringBoard(GameObject springBoardObj)
    {
        float springForce = 7f;
        float finalSpringForce = Mathf.Min(jumpForce + springForce, maxJumpForce);
        
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * finalSpringForce, ForceMode.Impulse);

        Animator animator = springBoardObj.GetComponentInParent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Activate");
        }
        else
        {
            Debug.LogWarning($"Animator not found in {springBoardObj.name}'s parent.");
        }

        isGrounded = false;
    }
    #endregion

    #region GameFlow_Logic
    private void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        if (gameOverText) gameOverText.SetActive(true);
        StartCoroutine(RestartGame());
    }

    private void GameClear()
    {
        if (!isGameActive) return;
        isGameActive = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        if (winText) winText.SetActive(true);
        StartCoroutine(LoadNextStage());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f);
        rb.isKinematic = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadNextStage()
    {
        yield return new WaitForSeconds(2f);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    #endregion

    #region Debug_Visual
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
