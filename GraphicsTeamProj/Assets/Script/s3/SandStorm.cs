//using UnityEngine;

//public class SandStorm : MonoBehaviour
//{
//    public float pushForce = 2f; // 공을 오른쪽으로 밀어내는 힘
//    public ParticleSystem sandParticles; // 모래 바람 파티클

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Player")) // Player 태그가 있는 오브젝트만 반응
//        {
//            Rigidbody rb = other.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                // 오른쪽으로 힘 가하기
//                rb.AddForce(Vector3.right * pushForce, ForceMode.Force);
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player") && sandParticles != null)
//        {
//            sandParticles.Play(); // 플레이어가 들어오면 파티클 재생
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player") && sandParticles != null)
//        {
//            sandParticles.Stop(); // 플레이어가 나가면 파티클 정지
//        }
//    }
//}

using UnityEngine;

public class SandStorm : MonoBehaviour
{
    public float pushForce = 2f; // 공을 오른쪽으로 밀어내는 힘
    public ParticleSystem sandParticles; // 모래 바람 파티클
    public AudioClip sandStormSound; // 모래바람 소리
    private AudioSource audioSource; // 오디오 소스

    void Start()
    {
        // AudioSource 컴포넌트를 동적으로 추가
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sandStormSound;
        audioSource.loop = true; // 루프 설정 (계속 재생)
        audioSource.playOnAwake = false; // 처음에 재생하지 않음
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Player 태그가 붙은 오브젝트만 반응
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 오른쪽으로 힘 가하기
                rb.AddForce(Vector3.right * pushForce, ForceMode.Force);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sandParticles != null)
            {
                sandParticles.Play(); // 플레이어가 들어오면 파티클 재생
            }
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play(); // 모래바람 소리 재생
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sandParticles != null)
            {
                sandParticles.Stop(); // 플레이어가 나가면 파티클 정지
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop(); // 모래바람 소리 멈춤
            }
        }
    }
}
