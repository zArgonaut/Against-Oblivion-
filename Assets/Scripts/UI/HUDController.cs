using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;

    PlayerEnergy energy;
    PlayerHealth health;
    PlayerStamina stamina;

    void Awake()
    {
        energy = FindFirstObjectByType<PlayerEnergy>();
        health = FindFirstObjectByType<PlayerHealth>();
        stamina = FindFirstObjectByType<PlayerStamina>();

        if (energy != null) energy.OnEnergyChanged += UpdateEnergy;
        if (health != null) health.OnHealthChanged += UpdateHealth;
        if (stamina != null) stamina.OnStaminaChanged += UpdateStamina;

        if (energy != null) UpdateEnergy(energy.currentEnergy);
        if (health != null) UpdateHealth(health.currentHealth);
        if (stamina != null) UpdateStamina(stamina.currentStamina);
    }

    void OnDestroy()
    {
        if (energy != null) energy.OnEnergyChanged -= UpdateEnergy;
        if (health != null) health.OnHealthChanged -= UpdateHealth;
        if (stamina != null) stamina.OnStaminaChanged -= UpdateStamina;
    }

    void UpdateEnergy(int value)
    {
        if (energyText != null)
            energyText.text = "Energy: " + value;
    }

    void UpdateHealth(int value)
    {
        if (healthText != null)
            healthText.text = "HP: " + value;
    }

    void UpdateStamina(int value)
    {
        if (staminaText != null)
            staminaText.text = "Stamina: " + value;
    }
}
