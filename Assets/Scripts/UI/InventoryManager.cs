using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class InventoryManager : MonoBehaviour
{
    [Header("Background (não mexer)")]
    [SerializeField] private Image backgroundImage;

    [Header("Slots (assign 6)")]
    [Tooltip("Arraste em ordem: Slot1/Icon → Slot6/Icon")]
    [SerializeField] private Image[] slotIcons = new Image[6];
    [Tooltip("Arraste em ordem: Slot1/Highlight → Slot6/Highlight")]
    [SerializeField] private Image[] slotHighlights = new Image[6];
    [Tooltip("Arraste em ordem: Slot1/CountText → Slot6/CountText")]
    [SerializeField] private TMP_Text[] slotCounts = new TMP_Text[6];

    [Header("Sprites dos Itens")]
    public Sprite rifleSprite;
    public Sprite shotgunSprite;
    public Sprite porreteSprite;
    public Sprite grenadeSprite;
    public Sprite bandageSprite;
    public Sprite ammoSprite;

    // Coleções acessadas pelo GameManager
    public Weapon[] armas = new Weapon[0];
    public int bandagens = 0;
    public int powerUps = 0;

    private int[] counts = new int[6];
    private int selectedIndex = 0;
    private int lastSelectedIndex = -1;

    void Start()
    {
        counts = new int[] { 1, 1, 1, 2, 0, 30 };
        foreach (var h in slotHighlights)
            if (h) h.gameObject.SetActive(false);
        RefreshUI();
        SelectSlot(0);
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SelectSlot((selectedIndex + 1) % counts.Length);
        else if (scroll < 0f) SelectSlot((selectedIndex + counts.Length - 1) % counts.Length);

        for (int i = 0; i < counts.Length; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
    }

    private void SelectSlot(int idx)
    {
        if (idx == selectedIndex || idx < 0 || idx >= slotHighlights.Length)
            return;

        if (slotHighlights[selectedIndex])
            slotHighlights[selectedIndex].gameObject.SetActive(false);

        selectedIndex = idx;

        if (slotHighlights[selectedIndex])
            slotHighlights[selectedIndex].gameObject.SetActive(true);

        RefreshUI();
    }

    private void UpdateHighlight()
    {
        if (slotHighlights == null || slotHighlights.Length == 0)
            return;

        for (int i = 0; i < slotHighlights.Length; i++)
            if (slotHighlights[i])
                slotHighlights[i].gameObject.SetActive(i == selectedIndex);
    }

    private void RefreshUI()
    {
        for (int i = 0; i < counts.Length; i++)
        {
            Sprite icon = null;
            string txt = "";

            switch (i)
            {
                case 0: icon = rifleSprite;   break;
                case 1: icon = shotgunSprite; break;
                case 2: icon = porreteSprite; break;
                case 3: icon = grenadeSprite; txt = counts[i].ToString(); break;
                case 4: icon = bandageSprite; txt = counts[i].ToString(); break;
                case 5: icon = ammoSprite;    txt = counts[i].ToString(); break;
            }

            if (slotIcons[i])
            {
                slotIcons[i].sprite  = icon;
                slotIcons[i].enabled = icon != null;
            }
            if (slotCounts[i])
                slotCounts[i].text = txt;
        }
    }

    public void AddBandage(int v = 1)  { counts[4] += v; RefreshUI(); }

    /// <summary>
    /// Wrapper de compatibilidade em português.
    /// </summary>
    public bool ConsumirMunicao(int v = 1)
    {
        return ConsumeAmmo(v);
    }

    public bool ConsumeAmmo(int v = 1)
    {
        if (counts[5] >= v)
        {
            counts[5] -= v;
            RefreshUI();
            return true;
        }
        return false;
    }
}
