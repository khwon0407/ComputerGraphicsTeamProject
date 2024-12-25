using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI buttonText; // 버튼의 TextMeshPro 텍스트 참조

    private void Awake()
    {
        // 버튼의 자식에서 TextMeshProUGUI 컴포넌트를 자동으로 찾습니다.
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogError("ButtonHoverHandler: TextMeshProUGUI를 찾을 수 없습니다. 텍스트 구성 요소가 버튼의 자식에 있는지 확인하세요.");
        }
    }

    // 마우스가 버튼 위로 올려졌을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            Debug.Log("버튼에 마우스가 올려졌습니다!");
            buttonText.text = $"<u>{buttonText.text}</u>"; // 밑줄 추가
        }
    }

    // 마우스가 버튼에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            Debug.Log("버튼에서 마우스가 벗어났습니다!");
            buttonText.text = buttonText.text.Replace("<u>", "").Replace("</u>", ""); // 밑줄 제거
        }
    }
}
