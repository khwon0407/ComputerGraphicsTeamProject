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
}
