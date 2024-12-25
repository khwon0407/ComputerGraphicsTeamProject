using UnityEngine;

public class BreakableBlockTest : MonoBehaviour
{
    public AudioClip breakSound; // �μ����� ȿ����
    public GameObject destructionEffect; // �ı� ȿ�� (���� ����)

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player�� �浹 ������");

            // �Ҹ� ��� ���� ������Ʈ ����
            if (breakSound != null)
            {
                GameObject audioObject = new GameObject("BlockBreakSound");
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 0f; // 2D ����
                audioSource.volume = 1.0f;
                audioSource.PlayOneShot(breakSound);
                Destroy(audioObject, breakSound.length); // �Ҹ� ���� �� ����
            }

            // �ı� ȿ�� (���� ����)
            if (destructionEffect != null)
            {
                Instantiate(destructionEffect, transform.position, Quaternion.identity);
            }

            // ��� �ı�
            Destroy(gameObject);
        }
    }
}
