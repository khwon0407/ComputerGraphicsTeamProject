using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI를 다루기 위해 필요

public class ButtonHandler : MonoBehaviour
{
    public Button myButton; // 버튼을 참조할 변수

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        myButton.onClick.AddListener(goBack);
    }

    // 버튼 클릭 시 실행될 함수
    void goBack()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("메뉴로 돌아갑니다.");
    }
}
