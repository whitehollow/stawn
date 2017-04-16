using UnityEngine;

class HealthBar : MonoBehaviour
{
    public Player Player;
    public Transform ForegroundSprite;
    public SpriteRenderer ForegroundRenderer;

    public Color MaxHealthColor = Color.green;
    public Color MinHealthColor = Color.yellow;

    public void Update()
    {
        var healthPercent = Player.Health /(float) Player.MaxHealth;

        ForegroundSprite.localScale = new Vector3(healthPercent, 1, 0);

        ForegroundRenderer.color = Color.Lerp(MinHealthColor, MaxHealthColor, healthPercent);
    }

}

