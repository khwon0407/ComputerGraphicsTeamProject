using UnityEngine;
using UnityEngine.SceneManagement; // 씬 로드를 위해 필요
using UnityEngine.UI; // UI를 다루기 위해 필요

public class StageSelectManager : MonoBehaviour
{
    // 버튼 배열로 스테이지 버튼을 저장
    public Button[] stageButtons;

    void Start()
    {
        // 각 버튼에 클릭 이벤트를 연결
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageIndex = i + 1; // 스테이지 번호 (1부터 시작)
            stageButtons[i].onClick.AddListener(() => LoadStage(stageIndex));
        }
    }

    // 특정 스테이지를 로드하는 함수
    public void LoadStage(int stageIndex)
    {
        string sceneName = "Stage" + stageIndex; // 스테이지 씬 이름 예: "Stage1"
        Debug.Log(sceneName + "로 이동합니다.");
        SceneManager.LoadScene(sceneName); // 해당 스테이지 씬 로드
    }
}
