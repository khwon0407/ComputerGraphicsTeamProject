using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPowerUp
{
    public string Name { get; set; }
    public float Duration { get; set; }
    public Color Color { get; set; }
    public System.Action ActivateEffect { get; set; }
    public System.Action DeactivateEffect { get; set; }
}

public class BallPowerUpManager : MonoBehaviour
{
    private List<BallPowerUp> activePowerUps = new List<BallPowerUp>();
    private Renderer ballRenderer;
    private Color originalColor;

    [Header("Power Up Colors")]
    [SerializeField] private Color speedBoostColor = Color.red;
    [SerializeField] private Color bounceBoostColor = Color.blue;

    private BallController ballController;

    void Start()
    {
        ballRenderer = GetComponent<Renderer>();
        originalColor = ballRenderer.material.color;
        ballController = GetComponent<BallController>();
    }

    public void AddPowerUp(BallPowerUp powerUp)
    {
        RemovePowerUp(powerUp.Name);
        activePowerUps.Add(powerUp);
        powerUp.ActivateEffect.Invoke();
        UpdateColor();
        StartCoroutine(RemovePowerUpAfterDuration(powerUp));
    }

    public void RemovePowerUp(string powerUpName)
    {
        BallPowerUp existingPowerUp = activePowerUps.Find(p => p.Name == powerUpName);
        if (existingPowerUp != null)
        {
            existingPowerUp.DeactivateEffect.Invoke();
            activePowerUps.Remove(existingPowerUp);
            UpdateColor();
        }
    }

    private IEnumerator RemovePowerUpAfterDuration(BallPowerUp powerUp)
    {
        yield return new WaitForSeconds(powerUp.Duration);
        RemovePowerUp(powerUp.Name);
    }

    private void UpdateColor()
    {
        if (activePowerUps.Count > 0)
        {
            ballRenderer.material.color = activePowerUps[activePowerUps.Count - 1].Color;
        }
        else
        {
            ballRenderer.material.color = originalColor;
        }
    }

    public void AddSpeedBoost(float multiplier, float duration)
    {
        AddPowerUp(new BallPowerUp
        {
            Name = "SpeedBoost",
            Duration = duration,
            Color = speedBoostColor,
            ActivateEffect = () => 
            {
                ballController.moveSpeed *= multiplier;
                ballController.maxSpeed *= multiplier;
            },
            DeactivateEffect = () => 
            {
                ballController.moveSpeed = ballController.originalMoveSpeed;
                ballController.maxSpeed = ballController.originalMaxSpeed;
            }
        });
    }

    public void AddBounceBoost(float multiplier, float duration)
    {
        AddPowerUp(new BallPowerUp
        {
            Name = "BounceBoost",
            Duration = duration,
            Color = bounceBoostColor,
            ActivateEffect = () => 
            {
                ballController.bounceSpeed *= multiplier;
            },
            DeactivateEffect = () => 
            {
                ballController.bounceSpeed = ballController.originalBounceSpeed;
            }
        });
    }
} 