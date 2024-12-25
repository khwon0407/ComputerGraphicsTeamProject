using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip testSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D 사운드로 설정
        audioSource.volume = 1.0f;

        if (testSound != null)
        {
            audioSource.PlayOneShot(testSound);
            Debug.Log("테스트 사운드 재생 완료");
        }
        else
        {
            Debug.LogWarning("testSound가 연결되지 않았습니다!");
        }
    }
}
