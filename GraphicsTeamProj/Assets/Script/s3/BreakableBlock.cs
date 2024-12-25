using UnityEngine;

public class BreakableBlockTest : MonoBehaviour
{
    public AudioClip breakSound; // 부서지는 효과음
    public GameObject destructionEffect; // 파괴 효과 (선택 사항)

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player와 충돌 감지됨");

            // 소리 재생 전용 오브젝트 생성
            if (breakSound != null)
            {
                GameObject audioObject = new GameObject("BlockBreakSound");
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 0f; // 2D 사운드
                audioSource.volume = 1.0f;
                audioSource.PlayOneShot(breakSound);
                Destroy(audioObject, breakSound.length); // 소리 끝난 후 제거
            }

            // 파괴 효과 (선택 사항)
            if (destructionEffect != null)
            {
                Instantiate(destructionEffect, transform.position, Quaternion.identity);
            }

            // 블록 파괴
            Destroy(gameObject);
        }
    }
}
