using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip testSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D ����� ����
        audioSource.volume = 1.0f;

        if (testSound != null)
        {
            audioSource.PlayOneShot(testSound);
            Debug.Log("�׽�Ʈ ���� ��� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("testSound�� ������� �ʾҽ��ϴ�!");
        }
    }
}
