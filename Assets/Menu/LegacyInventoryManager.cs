using UnityEngine;
using UnityEngine.UI;

public class LegacyInventoryManager : MonoBehaviour
{
    [Header("SLOTS INDIVIDUAIS")]

    [Header("Slot 1 – Rifle")]
    public Image rifleBackground;
    public Image rifleIcon;
    public Image rifleHighlight;
    public Text rifleCountText;

    [Header("Slot 2 – Shotgun")]
    public Image shotgunBackground;
    public Image shotgunIcon;
    public Image shotgunHighlight;
    public Text shotgunCountText;

    [Header("Slot 3 – Granada")]
    public Image grenadeBackground;
    public Image grenadeIcon;
    public Image grenadeHighlight;
    public Text grenadeCountText;

    [Header("Slot 4 – Porrete")]
    public Image clubBackground;
    public Image clubIcon;
    public Image clubHighlight;
    public Text clubCountText;

    [Header("Slot 5 – Bandagem")]
    public Image bandageBackground;
    public Image bandageIcon;
    public Image bandageHighlight;
    public Text bandageCountText;

    [Header("Slot 6 – Munição")]
    public Image ammoBackground;
    public Image ammoIcon;
    public Image ammoHighlight;
    public Text ammoCountText;


    [Header("SPRITES DOS ITENS")]
    public Sprite rifleSprite;
    public Sprite shotgunSprite;
    public Sprite grenadeSprite;
    public Sprite clubSprite;
    public Sprite bandageSprite;
    public Sprite ammoSprite;


    private Image[] backgrounds;
    private Image[] icons;
    private Image[] highlights;
    private Text[] countTexts;
    private Sprite[] itemSprites;
    private int[] counts = new int[6];
    private int selectedIndex;


    void Awake()
    {
        backgrounds = new[] {
            rifleBackground,
            shotgunBackground,
            grenadeBackground,
            clubBackground,
            bandageBackground,
            ammoBackground
        };
        icons = new[] {
            rifleIcon,
            shotgunIcon,
            grenadeIcon,
            clubIcon,
            bandageIcon,
            ammoIcon
        };
        highlights = new[] {
            rifleHighlight,
            shotgunHighlight,
            grenadeHighlight,
            clubHighlight,
            bandageHighlight,
            ammoHighlight
        };
        countTexts = new[] {
            rifleCountText,
            shotgunCountText,
            grenadeCountText,
            clubCountText,
            bandageCountText,
            ammoCountText
        };
        itemSprites = new[] {
            rifleSprite,
            shotgunSprite,
            grenadeSprite,
            clubSprite,
            bandageSprite,
            ammoSprite
        };
    }

    void Start()
    {
        counts = new[] { 1, 1, 2, 1, 0, 30 };
        selectedIndex = 0;
        RefreshAllSlots();
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
        if (d > 0f) SelectSlot((selectedIndex + 1) % 6);
        else if (d < 0f) SelectSlot((selectedIndex + 5) % 6);
    }

    private void HandleNumberKeys()
    {
        for (int i = 0; i < 6; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
    }

    private void SelectSlot(int idx)
    {
        selectedIndex = idx;
        UpdateHighlight();
        // Adicionar script de troca de arma
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < 6; i++)
        {
            if (counts[i] > 0)
            {
                icons[i].enabled = true;
                icons[i].sprite = itemSprites[i];
            }
            else
                icons[i].enabled = false;

            if (counts[i] > 1 || i >= 2)
                countTexts[i].text = counts[i].ToString();
            else
                countTexts[i].text = "";
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < 6; i++)
            highlights[i].gameObject.SetActive(i == selectedIndex);
    }

    public void AddBandage(int amount = 1)
    {
        counts[4] += amount;
        RefreshAllSlots();
    }

    public bool ConsumeAmmo(int amount)
    {
        if (counts[5] >= amount)
        {
            counts[5] -= amount;
            RefreshAllSlots();
            return true;
        }
        return false;
    }
}
