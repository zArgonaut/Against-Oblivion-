using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    public int maxEnergy = 100;
    public int currentEnergy;
    public float regenRate = 10f;

    public event System.Action<int> OnEnergyChanged;

    void Awake()
    {
        currentEnergy = maxEnergy;
        OnEnergyChanged?.Invoke(currentEnergy);
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += Mathf.CeilToInt(regenRate * Time.deltaTime);
            if (currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;
            OnEnergyChanged?.Invoke(currentEnergy);
        }
    }

    public bool Consume(int amount)
    {
        if (currentEnergy < amount)
            return false;

        currentEnergy -= amount;
        OnEnergyChanged?.Invoke(currentEnergy);
        return true;
    }

    public void Restore(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy);
    }
}
