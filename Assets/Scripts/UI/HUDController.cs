using System;
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

    void Start()
    {
        inventory = Object.FindFirstObjectByType<InventoryManager>();
        health = Object.FindFirstObjectByType<PlayerHealth>();
        stamina = Object.FindFirstObjectByType<PlayerStamina>();
        energy = Object.FindFirstObjectByType<PlayerEnergy>();

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

    void OnDestroy()
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
        if (inventory == null)
            return;

        var arma = inventory.ArmaEquipada;
        if (weaponText != null)
            weaponText.text = arma.ToString();
        if (ammoText != null)
        {
            int ammo = inventory.GetItemCount(5);
            ammoText.text = ammo.ToString();
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
