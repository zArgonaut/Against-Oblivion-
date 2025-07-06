using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI energyText;

    InventoryManager inventory;
    PlayerHealth health;
    PlayerStamina stamina;
    PlayerEnergy energy;

    void OnEnable()
    {
        if (inventory == null)
            inventory = FindObjectOfType<InventoryManager>();
        if (health == null)
            health = FindObjectOfType<PlayerHealth>();
        if (stamina == null)
            stamina = FindObjectOfType<PlayerStamina>();
        if (energy == null)
            energy = FindObjectOfType<PlayerEnergy>();

        if (inventory != null)
            inventory.OnInventoryChanged += UpdateWeaponUI;
        if (health != null)
            health.OnHealthChanged += UpdateHealthUI;
        if (stamina != null)
            stamina.OnStaminaChanged += UpdateStaminaUI;
        if (energy != null)
            energy.OnEnergyChanged += UpdateEnergyUI;

        UpdateWeaponUI();
        if (health != null) UpdateHealthUI(health.currentHealth);
        if (stamina != null) UpdateStaminaUI(stamina.currentStamina);
        if (energy != null) UpdateEnergyUI(energy.currentEnergy);
    }

    void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateWeaponUI;
        if (health != null)
            health.OnHealthChanged -= UpdateHealthUI;
        if (stamina != null)
            stamina.OnStaminaChanged -= UpdateStaminaUI;
        if (energy != null)
            energy.OnEnergyChanged -= UpdateEnergyUI;
    }

    void UpdateWeaponUI()
    {
        if (inventory == null || inventory.ArmaEquipada == null)
            return;

        var arma = inventory.ArmaEquipada;
        if (weaponText != null)
            weaponText.text = arma.tipo.ToString();
        if (ammoText != null)
        {
            string ammo = arma.capacidade == 0 ? "\u221E" : arma.municao + "/" + arma.capacidade;
            ammoText.text = ammo;
        }
    }

    void UpdateHealthUI(int value)
    {
        if (healthText != null && health != null)
            healthText.text = "HP: " + value + "/" + health.maxHealth;
    }

    void UpdateStaminaUI(int value)
    {
        if (staminaText != null && stamina != null)
            staminaText.text = "ST: " + value + "/" + stamina.maxStamina;
    }

    void UpdateEnergyUI(int value)
    {
        if (energyText != null && energy != null)
            energyText.text = "EN: " + value + "/" + energy.maxEnergy;
    }
}
