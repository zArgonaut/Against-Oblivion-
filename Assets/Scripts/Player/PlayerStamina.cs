using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    public int maxStamina = 100;
    public int currentStamina;
    public float regenRate = 15f;

    public event System.Action<int> OnStaminaChanged;

    void Awake()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina);
    }

    void Update()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += Mathf.CeilToInt(regenRate * Time.deltaTime);
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
            OnStaminaChanged?.Invoke(currentStamina);
        }
    }

    public bool Use(int amount)
    {
        if (currentStamina < amount)
            return false;
        currentStamina -= amount;
        OnStaminaChanged?.Invoke(currentStamina);
        return true;
    }

    public void Restore(int amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina);
    }
}
