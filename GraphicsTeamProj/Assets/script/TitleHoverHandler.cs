using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TitleHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text1;  // 첫 번째 텍스트 (하얀색 또는 검정색 텍스트)
    public TextMeshProUGUI text2;  // 두 번째 텍스트 (하얀색 또는 검정색 텍스트)

    private Color originalText1Color; // 첫 번째 텍스트의 원래 색상
    private Color originalText2Color; // 두 번째 텍스트의 원래 색상

    private void Awake()
    {
        // 텍스트 구성 요소를 찾기
        if (text1 == null || text2 == null)
        {
            Debug.LogError("TitleHoverHandler: 텍스트를 참조할 수 없습니다. 텍스트 구성 요소를 연결하세요.");
        }

        // 원래 색상 저장
        if (text1 != null)
        {
            originalText1Color = text1.color;
        }

        if (text2 != null)
        {
            originalText2Color = text2.color;
        }
    }

    // 마우스가 텍스트 위로 올려졌을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text1 != null && text2 != null)
        {
            Debug.Log("마우스가 텍스트 위로 올려졌습니다!");

            // 첫 번째 텍스트 배경 색상 반전 (검정 → 하얀색, 하얀색 → 검정색)
            if (text1.color == Color.white)
            {
                text1.color = Color.black;
            }
            else if (text1.color == Color.black)
            {
                text1.color = Color.white;
            }

            // 두 번째 텍스트 배경 색상 반전 (검정 → 하얀색, 하얀색 → 검정색)
            if (text2.color == Color.white)
            {
                text2.color = Color.black;
            }
            else if (text2.color == Color.black)
            {
                text2.color = Color.white;
            }
        }
    }

    // 마우스가 텍스트에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (text1 != null && text2 != null)
        {
            Debug.Log("마우스가 텍스트에서 벗어났습니다!");

            // 원래 색상으로 복원
            text1.color = originalText1Color;
            text2.color = originalText2Color;
        }
    }
}
