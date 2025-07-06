using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    public event System.Action<int> OnHealthChanged;

    [Tooltip("Event invoked when the player dies")]
    public UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void LevarDano(int quantidade)
    {
        if (quantidade <= 0) return;

        currentHealth -= quantidade;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Curar(int quantidade)
    {
        if (quantidade <= 0) return;

        currentHealth += quantidade;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
    }

    void Die()
    {
        onDeath?.Invoke();
        if (GameManager.Instance != null)
            GameManager.Instance.EnterGameOver();
        gameObject.SetActive(false);
    }
}
