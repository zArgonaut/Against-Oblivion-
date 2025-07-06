using UnityEngine;

public enum WeaponType
{
    Porrete,
    Rifle,
    Shotgun,
    Granada,
    PaoBaguete
}

[System.Serializable]
public class WeaponSlot
{
    public WeaponType tipo;
    public int municao;
    public int capacidade;
}

public class InventoryManager : MonoBehaviour
{
    public WeaponSlot[] armas = new WeaponSlot[4];
    public int bandagens = 0;
    public int powerUps = 0;

    public event System.Action OnInventoryChanged;

    int slotEquipped = 0;
    public WeaponSlot ArmaEquipada => armas.Length > slotEquipped ? armas[slotEquipped] : null;

    public int MunicaoAtual => ArmaEquipada != null ? ArmaEquipada.municao : 0;
    public int CapacidadeAtual => ArmaEquipada != null ? ArmaEquipada.capacidade : 0;
    public WeaponType TipoAtual => ArmaEquipada != null ? ArmaEquipada.tipo : WeaponType.Porrete;

    void Awake()
    {
        // Preenche os slots com armas padrao e quantidades iniciais de municao
        if (armas.Length < 4)
            armas = new WeaponSlot[4];

        armas[0] = new WeaponSlot { tipo = WeaponType.Porrete, municao = 0, capacidade = 0 };
        armas[1] = new WeaponSlot { tipo = WeaponType.Rifle, municao = 50, capacidade = 100 };
        armas[2] = new WeaponSlot { tipo = WeaponType.Shotgun, municao = 20, capacidade = 40 };
        armas[3] = new WeaponSlot { tipo = WeaponType.Granada, municao = 5, capacidade = 10 };
        OnInventoryChanged?.Invoke();
    }

    public void AdicionarMunicao(WeaponType tipo, int quantidade)
    {
        foreach (var slot in armas)
        {
            if (slot.tipo == tipo)
            {
                slot.municao = Mathf.Min(slot.municao + quantidade, slot.capacidade);
                break;
            }
        }
        OnInventoryChanged?.Invoke();
    }

    public void AumentarCapacidade(WeaponType tipo, int quantidade)
    {
        foreach (var slot in armas)
        {
            if (slot.tipo == tipo)
            {
                slot.capacidade += quantidade;
                break;
            }
        }
        OnInventoryChanged?.Invoke();
    }

    public void UsarBandagem(PlayerHealth vida, int quantidade)
    {
        if (bandagens <= 0) return;
        vida.Curar(quantidade);
        bandagens--;
        OnInventoryChanged?.Invoke();
    }

    public void MudarSlotEquipada(int novoSlot, WeaponController controller)
    {
        if (novoSlot < 0 || novoSlot >= armas.Length)
            return;

        slotEquipped = novoSlot;
        AtualizarController(controller);
        OnInventoryChanged?.Invoke();
    }

    public void EquiparArma(int slot, WeaponController controller)
    {
        MudarSlotEquipada(slot, controller);
        OnInventoryChanged?.Invoke();
    }

    public bool ConsumirMunicao(int quantidade = 1)
    {
        var arma = ArmaEquipada;
        if (arma == null)
            return false;

        if (arma.capacidade == 0)
            return true; // melee weapons don't use ammo

        if (arma.municao < quantidade)
            return false;

        arma.municao -= quantidade;
        OnInventoryChanged?.Invoke();
        return true;
    }

    void AtualizarController(WeaponController controller)
    {
        if (controller == null) return;

        switch (ArmaEquipada.tipo)
        {
            case WeaponType.Porrete:
                controller.damage = 15;
                controller.fireRate = 0.8f;
                controller.energyCost = 0;
                break;
            case WeaponType.Rifle:
                controller.damage = 20;
                controller.fireRate = 0.3f;
                controller.energyCost = 5;
                break;
            case WeaponType.Shotgun:
                controller.damage = 35;
                controller.fireRate = 0.9f;
                controller.energyCost = 10;
                break;
            case WeaponType.Granada:
                controller.damage = 50;
                controller.fireRate = 1.2f;
                controller.energyCost = 15;
                break;
            case WeaponType.PaoBaguete:
                controller.damage = 5;
                controller.fireRate = 0.2f;
                controller.energyCost = 1;
                break;
        }
    }
}
