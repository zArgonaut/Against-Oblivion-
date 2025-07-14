using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
// using Core;  // seu namespace onde está WeaponType

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
    public int grenades    = 2;
    public int bandages   = 0;
    public int ammoCount  = 30;

    private Image[] icons;
    private TMP_Text[] counts;
    private Image[] highlights;

    private int selectedSlot = 0;

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
        RefreshUI();
        UpdateHighlight();
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

            switch (i)
            {
                case 0: icon = rifleSprite;   break;
                case 1: icon = shotgunSprite; break;
                case 2: icon = porreteSprite; break;
                case 3: icon = grenadeSprite; c = grenades.ToString(); break;
                case 4: icon = bandageSprite; c = bandages.ToString(); break;
                case 5: icon = ammoSprite;    c = ammoCount.ToString();  break;
            }

            icons[i].sprite  = icon;
            icons[i].enabled = icon != null;
            counts[i].text   = c;
        }
    }

    public void AddBandage(int amt = 1)
    {
        bandages += amt;
        RefreshUI();
    }
    public bool ConsumeAmmo(int amt = 1)
    {
        if (ammoCount >= amt)
        {
            ammoCount -= amt;
            RefreshUI();
            return true;
        }
        return false;
    }
}
