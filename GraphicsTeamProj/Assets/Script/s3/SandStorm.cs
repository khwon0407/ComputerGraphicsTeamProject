//using UnityEngine;

//public class SandStorm : MonoBehaviour
//{
//    public float pushForce = 2f; // ���� ���������� �о�� ��
//    public ParticleSystem sandParticles; // �� �ٶ� ��ƼŬ

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Player")) // Player �±װ� �ִ� ������Ʈ�� ����
//        {
//            Rigidbody rb = other.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                // ���������� �� ���ϱ�
//                rb.AddForce(Vector3.right * pushForce, ForceMode.Force);
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player") && sandParticles != null)
//        {
//            sandParticles.Play(); // �÷��̾ ������ ��ƼŬ ���
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player") && sandParticles != null)
//        {
//            sandParticles.Stop(); // �÷��̾ ������ ��ƼŬ ����
//        }
//    }
//}

using UnityEngine;

public class SandStorm : MonoBehaviour
{
    public float pushForce = 2f; // ���� ���������� �о�� ��
    public ParticleSystem sandParticles; // �� �ٶ� ��ƼŬ
    public AudioClip sandStormSound; // �𷡹ٶ� �Ҹ�
    private AudioSource audioSource; // ����� �ҽ�

    void Start()
    {
        // AudioSource ������Ʈ�� �������� �߰�
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sandStormSound;
        audioSource.loop = true; // ���� ���� (��� ���)
        audioSource.playOnAwake = false; // ó���� ������� ����
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Player �±װ� ���� ������Ʈ�� ����
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ���������� �� ���ϱ�
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
                sandParticles.Play(); // �÷��̾ ������ ��ƼŬ ���
            }
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play(); // �𷡹ٶ� �Ҹ� ���
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sandParticles != null)
            {
                sandParticles.Stop(); // �÷��̾ ������ ��ƼŬ ����
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop(); // �𷡹ٶ� �Ҹ� ����
            }
        }
    }
}
