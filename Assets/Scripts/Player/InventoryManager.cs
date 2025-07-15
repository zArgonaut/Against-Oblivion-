using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class Arma
{
    public WeaponType tipo;
    public int municao;
    public int capacidade;
}

public class InventoryManager : MonoBehaviour
{
    [Header("Slot Containers (drag & drop aqui)")]
    [Tooltip("Arraste os 6 GameObjects dos slots, na ordem: Rifle, Shotgun, Porrete, Granada, Bandagem, Munição")]
    public RectTransform[] slotContainers = new RectTransform[6];

    [Header("Sprites dos Itens")]
    public Sprite rifleSprite;
    public Sprite shotgunSprite;
    public Sprite porreteSprite;
    public Sprite grenadeSprite;
    public Sprite bandageSprite;
    public Sprite ammoSprite;

    [Header("Dados de Inventário")]
    public Arma[] armas = new Arma[3]; // Rifle, Shotgun, Porrete
    public int grenades   = 2;
    public int bandagens  = 0;
    public int powerUps   = 0;

    private Image[] icons;
    private TMP_Text[] counts;
    private Image[] highlights;

    private int selectedSlot = 0;

    public Arma ArmaEquipada => selectedSlot < armas.Length ? armas[selectedSlot] : null;

    public event Action OnInventoryChanged;

    void Awake()
    {
        int n = slotContainers.Length;
        icons       = new Image[n];
        counts      = new TMP_Text[n];
        highlights  = new Image[n];

        for (int i = 0; i < n; i++)
        {
            var root = slotContainers[i];
            icons[i]      = root.Find("Icon").GetComponent<Image>();
            counts[i]     = root.Find("CountText").GetComponent<TMP_Text>();
            highlights[i] = root.Find("Highlight").GetComponent<Image>();
            highlights[i].gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (armas == null || armas.Length == 0)
        {
            armas = new Arma[3];
            armas[0] = new Arma { tipo = WeaponType.Rifle, municao = 30, capacidade = 30 };
            armas[1] = new Arma { tipo = WeaponType.Shotgun, municao = 8, capacidade = 8 };
            armas[2] = new Arma { tipo = WeaponType.Porrete, municao = 0, capacidade = 0 };
        }

        RefreshUI();
        UpdateHighlight();
        OnInventoryChanged?.Invoke();
    }

    void Update()
    {
        HandleScroll();
        HandleNumberKeys();
    }

    private void HandleScroll()
    {
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f) SelectSlot((selectedSlot + 1) % slotContainers.Length);
        else if (d < 0f) SelectSlot((selectedSlot + slotContainers.Length - 1) % slotContainers.Length);
    }

    private void HandleNumberKeys()
    {
        for (int i = 0; i < slotContainers.Length; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
    }

    private void SelectSlot(int idx)
    {
        selectedSlot = idx;
        RefreshUI();
        UpdateHighlight();
        OnInventoryChanged?.Invoke();
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < highlights.Length; i++)
            highlights[i].gameObject.SetActive(i == selectedSlot);
    }

    private void RefreshUI()
    {
        for (int i = 0; i < slotContainers.Length; i++)
        {
            Sprite icon = null;
            string c    = "";

            if (i < armas.Length)
            {
                var arma = armas[i];
                switch (arma.tipo)
                {
                    case WeaponType.Rifle:   icon = rifleSprite;   break;
                    case WeaponType.Shotgun: icon = shotgunSprite; break;
                    case WeaponType.Porrete: icon = porreteSprite; break;
                }
                c = arma.municao.ToString();
            }
            else if (i == armas.Length)
            {
                icon = grenadeSprite;
                c = grenades.ToString();
            }
            else if (i == armas.Length + 1)
            {
                icon = bandageSprite;
                c = bandagens.ToString();
            }

            icons[i].sprite  = icon;
            icons[i].enabled = icon != null;
            counts[i].text   = c;
        }
    }

    public void AddBandage(int amt = 1)
    {
        bandagens += amt;
        RefreshUI();
        OnInventoryChanged?.Invoke();
    }

    public bool ConsumirMunicao(int amt = 1)
    {
        var arma = ArmaEquipada;
        if (arma != null && arma.municao >= amt)
        {
            arma.municao -= amt;
            RefreshUI();
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void AdicionarMunicao(WeaponType tipo, int amt)
    {
        foreach (var a in armas)
        {
            if (a.tipo == tipo)
            {
                a.municao += amt;
                if (a.municao > a.capacidade)
                    a.municao = a.capacidade;
                break;
            }
        }
        RefreshUI();
        OnInventoryChanged?.Invoke();
    }

    public void AumentarCapacidade(WeaponType tipo, int amt)
    {
        foreach (var a in armas)
        {
            if (a.tipo == tipo)
            {
                a.capacidade += amt;
                break;
            }
        }
        RefreshUI();
        OnInventoryChanged?.Invoke();
    }
}
