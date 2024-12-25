using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerUpManager = GetComponent<PowerUpManager>();
        ballRenderer = GetComponent<Renderer>();

        originalColor = ballRenderer.material.color;

        gameOverText?.SetActive(false);
        winText?.SetActive(false);
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
    Debug.Log($"Triggered by: {other.gameObject.name}"); // 디버그 추가

    if (other.CompareTag("PowerUpBlock"))
    {
        Debug.Log("PowerUpBlock triggered!");
        powerUpManager.AddPowerUp(new PowerUp
        {
            Name = "SpeedBoost",
            Duration = 5f,
            Color = powerUpColor,
            ActivateEffect = () => speed *= 2,
            DeactivateEffect = () => speed = originalSpeed
        });

        Destroy(other.gameObject); // 블록 제거
    }

    if (other.CompareTag("JumpBoostBlock"))
    {
        Debug.Log("JumpBoostBlock triggered!");
        powerUpManager.AddPowerUp(new PowerUp
        {
            Name = "JumpBoost",
            Duration = 5f,
            Color = jumpBoostColor,
            ActivateEffect = () => jumpForce *= 1.5f,
            DeactivateEffect = () => jumpForce = originalJumpForce
        });

        Destroy(other.gameObject); // 블록 제거
    }
}


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision Detected: {collision.collider.name}");

        if (collision.collider.CompareTag("SpringBoard"))
        {
            Debug.Log("SpringBoard detected. Activating...");
            ActivateSpringBoard(collision.collider.transform.parent != null ? collision.collider.transform.parent.gameObject : collision.gameObject);
            return;
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
                    HandleJump();
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

        if (isJumpBoostActive && remainingJumpBoosts > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float boostedJumpForce = jumpForce * 1.5f;
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
            rb.AddForce(Vector3.up * Mathf.Min(jumpForce, maxJumpForce), ForceMode.Impulse);
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

        float springForce = 7f;
        float finalSpringForce = Mathf.Min(jumpForce + springForce, maxJumpForce);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * finalSpringForce, ForceMode.Impulse);

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

        isGrounded = false;
        hasJumped = true;
    }

    private void GameOver()
    {
        isGameActive = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        gameOverText.SetActive(true);
        StartCoroutine(RestartGame());
    }

    private void GameClear()
    {
        isGameActive = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        winText.SetActive(true);
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
}
