using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportTarget; // �ڷ���Ʈ�� ��ġ
    public AudioClip teleportSound; // �ڷ���Ʈ ȿ����
    private AudioSource audioSource; // ����� �ҽ�

    void Start()
    {
        // AudioSource�� ��ũ��Ʈ�� ����Ǿ� ���� �ʴٸ�, ������Ʈ �߰�
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Player �±װ� �ִ� ������Ʈ�� ����
        {
            // �ڷ���Ʈ �Ҹ� ���
            if (teleportSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }

            // ��ġ �̵�
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
