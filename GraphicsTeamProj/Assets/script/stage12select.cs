using UnityEngine;
using UnityEngine.SceneManagement;

public class stage12select : MonoBehaviour
{
    // 스테이지 1로 이동
    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    // 스테이지 2로 이동
    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage2");
    }

    // 게임 종료 함수
    public void QuitGame()
    {
        // 에디터에서 실행 중일 경우 로그 표시
#if UNITY_EDITOR
        Debug.Log("게임 종료 버튼이 눌렸습니다. (에디터에서는 종료되지 않음)");
#else
        // 빌드된 애플리케이션에서 게임 종료
        Application.Quit();
#endif
    }

    // 메인 메뉴로 이동하는 함수
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("stageselect");
    }
}
