using UnityEngine;
using UnityEngine.UI; // RawImage를 사용하려면 이 네임스페이스를 추가해야 합니다.

public class RawImageToggleOnClick : MonoBehaviour
{
    public RawImage image1;  // 첫 번째 RawImage
    public RawImage image2;  // 두 번째 RawImage

    private void Awake()
    {
        // 초기 상태 설정: 첫 번째 이미지만 보이도록
        if (image1 != null && image2 != null)
        {
            image1.gameObject.SetActive(true);  // 첫 번째 이미지는 보이도록
            image2.gameObject.SetActive(false); // 두 번째 이미지는 숨기도록
        }
    }

    // RawImage 클릭 시 호출되는 함수
    public void OnClickToggleImages()
    {
        if (image1 != null && image2 != null)
        {
            // 두 이미지 상태를 번갈아가며 전환
            bool isImage1Active = image1.gameObject.activeSelf;

            image1.gameObject.SetActive(!isImage1Active); // 첫 번째 이미지 상태 반전
            image2.gameObject.SetActive(isImage1Active);  // 두 번째 이미지 상태 반전
        }
    }
}
