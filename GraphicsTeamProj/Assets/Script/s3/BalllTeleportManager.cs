using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportTarget; // 텔레포트할 위치
    public AudioClip teleportSound; // 텔레포트 효과음
    private AudioSource audioSource; // 오디오 소스

    void Start()
    {
        // AudioSource가 스크립트에 연결되어 있지 않다면, 컴포넌트 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Player 태그가 있는 오브젝트만 반응
        {
            // 텔레포트 소리 재생
            if (teleportSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }

            // 위치 이동
            if (teleportTarget != null)
            {
                other.transform.position = teleportTarget.position;
            }
            else
            {
                Debug.LogWarning("Teleport target is not set!");
            }
        }
    }
}
