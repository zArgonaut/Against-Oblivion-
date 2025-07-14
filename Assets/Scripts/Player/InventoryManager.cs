using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Core;

[System.Serializable]
public class WeaponSlot
{
    public WeaponType tipo;
    public int municao;
    public int capacidade;
}

[System.Serializable]
public class SlotUI
{
    public Image backgroundImage;
    public Image iconImage;
    public TextMeshProUGUI countText;
    public Image highlightImage;
}

public class InventoryManager : MonoBehaviour
{
    [Header("Slot UI References")] 
    public SlotUI[] slots = new SlotUI[6];

    [Header("Item Sprites")] 
    public Sprite rifleSprite;
    public Sprite shotgunSprite;
    public Sprite porreteSprite;
    public Sprite grenadeSprite;
    public Sprite bandageSprite;
    public Sprite ammoSprite;

    [Header("Inventory Data")] 
    public WeaponSlot[] armas = new WeaponSlot[3];
    public int grenades = 2;
    public int bandagens;
    public int powerUps;
    public int indiceEquipada;

    int slotSelecionado;

    public event Action OnInventoryChanged;

    public WeaponSlot ArmaEquipada
    {
        get
        {
            if (armas == null || armas.Length == 0) return null;
            indiceEquipada = Mathf.Clamp(indiceEquipada, 0, armas.Length - 1);
            return armas[indiceEquipada];
        }
    }

    void Start()
    {
        foreach (var s in slots)
            if (s != null && s.highlightImage != null)
                s.highlightImage.gameObject.SetActive(false);

        slotSelecionado = 0;
        indiceEquipada = 0;
        RefreshUI();
        UpdateHighlight();
    }

    void Update()
    {
        HandleScroll();
        HandleNumberKeys();
    }

    void HandleScroll()
    {
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f) SelectSlot((slotSelecionado + 1) % slots.Length);
        else if (d < 0f) SelectSlot((slotSelecionado + slots.Length - 1) % slots.Length);
    }

    void HandleNumberKeys()
    {
        for (int i = 0; i < slots.Length; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
    }

    public void SelectSlot(int idx)
    {
        slotSelecionado = Mathf.Clamp(idx, 0, slots.Length - 1);
        if (slotSelecionado < armas.Length)
            indiceEquipada = slotSelecionado;
        UpdateHighlight();
        RefreshUI();
        OnInventoryChanged?.Invoke();
        ApplyCurrentItem();
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] != null && slots[i].highlightImage != null)
                slots[i].highlightImage.gameObject.SetActive(i == slotSelecionado);
    }

    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var ui = slots[i];
            if (ui == null) continue;
            switch (i)
            {
                case 0:
                    SetSlotUI(ui, rifleSprite, "");
                    break;
                case 1:
                    SetSlotUI(ui, shotgunSprite, "");
                    break;
                case 2:
                    SetSlotUI(ui, porreteSprite, "");
                    break;
                case 3:
                    SetSlotUI(ui, grenadeSprite, grenades.ToString());
                    break;
                case 4:
                    SetSlotUI(ui, bandageSprite, bandagens.ToString());
                    break;
                case 5:
                    int ammo = ArmaEquipada != null ? ArmaEquipada.municao : 0;
                    SetSlotUI(ui, ammoSprite, ammo.ToString());
                    break;
            }
        }
    }

    void SetSlotUI(SlotUI ui, Sprite icon, string count)
    {
        if (ui.iconImage != null)
        {
            ui.iconImage.sprite = icon;
            ui.iconImage.enabled = icon != null;
        }
        if (ui.countText != null)
            ui.countText.text = count;
    }

    public bool ConsumirMunicao()
    {
        var arma = ArmaEquipada;
        if (arma == null) return false;
        if (arma.capacidade == 0) return true;
        if (arma.municao <= 0) return false;
        arma.municao--;
        RefreshUI();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void AumentarCapacidade(WeaponType tipo, int valor)
    {
        foreach (var slot in armas)
        {
            if (slot.tipo == tipo)
            {
                slot.capacidade += valor;
                slot.municao = Mathf.Min(slot.municao, slot.capacidade);
                RefreshUI();
                OnInventoryChanged?.Invoke();
                break;
            }
        }
    }

    public void AdicionarMunicao(WeaponType tipo, int valor)
    {
        foreach (var slot in armas)
        {
            if (slot.tipo == tipo)
            {
                if (slot.capacidade > 0)
                    slot.municao = Mathf.Min(slot.municao + valor, slot.capacidade);
                RefreshUI();
                OnInventoryChanged?.Invoke();
                break;
            }
        }
    }

    public void AddBandage(int amount = 1)
    {
        bandagens += amount;
        RefreshUI();
        OnInventoryChanged?.Invoke();
    }

    void ApplyCurrentItem()
    {
        // Future implementation: equip weapon or use item prefabs here.
    }
}
