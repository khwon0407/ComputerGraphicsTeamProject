using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI�� �ٷ�� ���� �ʿ�

public class ButtonHandler : MonoBehaviour
{
    public Button myButton; // ��ư�� ������ ����

    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ ����
        myButton.onClick.AddListener(goBack);
    }

    // ��ư Ŭ�� �� ����� �Լ�
    void goBack()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("�޴��� ���ư��ϴ�.");
    }
}
