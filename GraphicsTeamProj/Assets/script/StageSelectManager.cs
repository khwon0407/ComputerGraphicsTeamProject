using UnityEngine;
using UnityEngine.SceneManagement; // �� �ε带 ���� �ʿ�
using UnityEngine.UI; // UI�� �ٷ�� ���� �ʿ�

public class StageSelectManager : MonoBehaviour
{
    // ��ư �迭�� �������� ��ư�� ����
    public Button[] stageButtons;

    void Start()
    {
        // �� ��ư�� Ŭ�� �̺�Ʈ�� ����
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageIndex = i + 1; // �������� ��ȣ (1���� ����)
            stageButtons[i].onClick.AddListener(() => LoadStage(stageIndex));
        }
    }

    // Ư�� ���������� �ε��ϴ� �Լ�
    public void LoadStage(int stageIndex)
    {
        string sceneName = "Stage" + stageIndex; // �������� �� �̸� ��: "Stage1"
        Debug.Log(sceneName + "�� �̵��մϴ�.");
        SceneManager.LoadScene(sceneName); // �ش� �������� �� �ε�
    }
}
