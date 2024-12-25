using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp
{
    public string Name { get; set; } // 파워업 이름
    public float Duration { get; set; } // 지속 시간
    public Color Color { get; set; } // 파워업 활성화 시 적용 색상
    public System.Action ActivateEffect { get; set; } // 활성화 효과
    public System.Action DeactivateEffect { get; set; } // 비활성화 효과
}


public class PowerUpManager : MonoBehaviour
{
    private List<PowerUp> activePowerUps = new List<PowerUp>();
    private Renderer ballRenderer; // 공의 Renderer
    private Color originalColor;

    void Start()
    {
        ballRenderer = GetComponent<Renderer>();
        originalColor = ballRenderer.material.color;
    }

    public void AddPowerUp(PowerUp powerUp)
    {
        // 동일 이름의 파워업 제거 (중복 방지)
        RemovePowerUp(powerUp.Name);

        // 파워업 활성화
        activePowerUps.Add(powerUp);
        powerUp.ActivateEffect.Invoke();

        // 색상 업데이트
        UpdateColor();

        // 지속 시간 후 비활성화
        StartCoroutine(RemovePowerUpAfterDuration(powerUp));
    }

    public void RemovePowerUp(string powerUpName)
    {
        PowerUp existingPowerUp = activePowerUps.Find(p => p.Name == powerUpName);
        if (existingPowerUp != null)
        {
            existingPowerUp.DeactivateEffect.Invoke();
            activePowerUps.Remove(existingPowerUp);

            // 색상 업데이트
            UpdateColor();
        }
    }

    private IEnumerator RemovePowerUpAfterDuration(PowerUp powerUp)
    {
        yield return new WaitForSeconds(powerUp.Duration);
        RemovePowerUp(powerUp.Name);
    }

    private void UpdateColor()
    {
        if (activePowerUps.Count > 0)
        {
            // 마지막으로 활성화된 파워업의 색상으로 설정
            ballRenderer.material.color = activePowerUps[activePowerUps.Count - 1].Color;
        }
        else
        {
            // 파워업이 없으면 원래 색상으로 복구
            ballRenderer.material.color = originalColor;
        }
    }
}
