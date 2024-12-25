using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class QuitButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private TextMeshProUGUI buttonText;  // 버튼의 TextMeshPro 텍스트 참조
    private Image buttonImage;           // 버튼의 배경 이미지 참조

    private Color originalTextColor;     // 원래 텍스트 색상
    private Color originalBackgroundColor;  // 원래 배경 색상

    private void Awake()
    {
        // 버튼의 자식에서 TextMeshProUGUI와 Image 컴포넌트를 자동으로 찾습니다.
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponentInChildren<Image>();

        if (buttonText == null)
        {
            Debug.LogError("QuitButtonHoverHandler: TextMeshProUGUI를 찾을 수 없습니다. 텍스트 구성 요소가 버튼의 자식에 있는지 확인하세요.");
        }

        if (buttonImage == null)
        {
            Debug.LogError("QuitButtonHoverHandler: Image를 찾을 수 없습니다. 버튼의 자식에 배경 이미지가 있는지 확인하세요.");
        }

        // 원래 색상 저장
        if (buttonText != null)
        {
            originalTextColor = buttonText.color;
        }
        if (buttonImage != null)
        {
            originalBackgroundColor = buttonImage.color;
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

    // 버튼 클릭을 시작할 때
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonText != null && buttonImage != null)
        {
            buttonText.color = Color.black;  // 글자 색상 검정으로 변경
            buttonImage.color = Color.white;  // 배경 색상 흰색으로 변경
            Debug.Log("버튼 클릭됨: 배경을 흰색으로, 텍스트를 검정색으로 변경.");
        }
    }

    // 버튼 클릭을 끝낼 때
    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonText != null && buttonImage != null)
        {
            buttonText.color = originalTextColor;  // 원래 텍스트 색상으로 복원
            buttonImage.color = originalBackgroundColor;  // 원래 배경 색상으로 복원
            Debug.Log("버튼 클릭 종료: 원래 색상으로 복원.");
        }
    }
}
