using UnityEngine;

public class HowToPlayHover : MonoBehaviour
{
    public GameObject howToPlayPanel; // 게임 방법을 표시할 패널

    private void Start()
    {
        // 게임 시작 시 Panel을 숨깁니다.
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // 마우스가 버튼 위로 올려졌을 때
    public void OnPointerEnter()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true); // 패널 활성화 (게임 방법 텍스트 보이게)
        }
    }

    // 마우스가 버튼에서 벗어났을 때
    public void OnPointerExit()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false); // 패널 비활성화 (게임 방법 텍스트 숨김)
        }
    }
}

