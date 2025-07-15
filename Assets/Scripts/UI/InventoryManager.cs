using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab; // Prefab para usar na cena
    public int count;
    public bool showCount = true; // Se deve mostrar contador
}

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

    [Header("Sistema de Itens")]
    [SerializeField] private ItemData[] inventoryItems = new ItemData[6];
    
    [Header("Configurações")]
    [SerializeField] private bool enablePrefabSystem = true;
    [SerializeField] private KeyCode[] slotKeys = {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, 
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6
    };

    // Propriedades públicas para compatibilidade
    public Weapon[] armas = new Weapon[0];
    public int bandagens = 0;
    public int powerUps = 0;

    private int selectedIndex = -1;
    private GameObject currentSelectedPrefab;

    // Eventos para notificar outros sistemas
    public System.Action<int> OnSlotSelected;
    public System.Action<GameObject> OnPrefabSelected;

    void Start()
    {
        InitializeInventory();
        RefreshUI();
        SelectSlot(0);
    }

    void Update()
    {
        // Teste direto das teclas
        if (Input.GetKeyDown(KeyCode.Alpha1)) { Debug.Log("Alpha1 pressionada"); SelectSlot(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { Debug.Log("Alpha2 pressionada"); SelectSlot(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { Debug.Log("Alpha3 pressionada"); SelectSlot(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { Debug.Log("Alpha4 pressionada"); SelectSlot(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { Debug.Log("Alpha5 pressionada"); SelectSlot(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { Debug.Log("Alpha6 pressionada"); SelectSlot(5); }
        
        // Keypad backup
        if (Input.GetKeyDown(KeyCode.Keypad1)) { Debug.Log("Keypad1 pressionada"); SelectSlot(0); }
        if (Input.GetKeyDown(KeyCode.Keypad2)) { Debug.Log("Keypad2 pressionada"); SelectSlot(1); }
        if (Input.GetKeyDown(KeyCode.Keypad3)) { Debug.Log("Keypad3 pressionada"); SelectSlot(2); }
        if (Input.GetKeyDown(KeyCode.Keypad4)) { Debug.Log("Keypad4 pressionada"); SelectSlot(3); }
        if (Input.GetKeyDown(KeyCode.Keypad5)) { Debug.Log("Keypad5 pressionada"); SelectSlot(4); }
        if (Input.GetKeyDown(KeyCode.Keypad6)) { Debug.Log("Keypad6 pressionada"); SelectSlot(5); }
    }

    private void InitializeInventory()
    {
        // Inicializa dados padrão se não configurado
        if (inventoryItems.Length != 6)
        {
            inventoryItems = new ItemData[6];
        }

        // Desabilita todos os highlights inicialmente
        for (int i = 0; i < slotHighlights.Length; i++)
        {
            if (slotHighlights[i])
                slotHighlights[i].gameObject.SetActive(false);
        }

        // Configuração padrão se itens não estiverem configurados
        SetupDefaultItems();
    }

    private void SetupDefaultItems()
    {
        // Apenas configura se os itens estão vazios
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = new ItemData();
            }
        }

        // Configuração padrão (você pode remover isso e configurar pelo Inspector)
        if (string.IsNullOrEmpty(inventoryItems[0].itemName))
        {
            inventoryItems[0] = new ItemData { itemName = "Rifle", count = 1, showCount = false };
            inventoryItems[1] = new ItemData { itemName = "Shotgun", count = 1, showCount = false };
            inventoryItems[2] = new ItemData { itemName = "Porrete", count = 1, showCount = false };
            inventoryItems[3] = new ItemData { itemName = "Granada", count = 2, showCount = true };
            inventoryItems[4] = new ItemData { itemName = "Bandagem", count = 0, showCount = true };
            inventoryItems[5] = new ItemData { itemName = "Munição", count = 30, showCount = true };
        }
    }

    private void HandleInput()
    {
        // Debug para verificar se está chegando no HandleInput
        if (Input.anyKeyDown)
        {
            Debug.Log("HandleInput: Detectou tecla pressionada");
        }

        // Verifica teclas Alpha (1-6)
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                Debug.Log($"Tecla Alpha{i + 1} pressionada - Selecionando slot {i}");
                SelectSlot(i);
                return;
            }
        }

        // Verifica teclas Keypad (1-6)
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                Debug.Log($"Tecla Keypad{i + 1} pressionada - Selecionando slot {i}");
                SelectSlot(i);
                return;
            }
        }

        // Debug específico para teclas numéricas
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Tecla 2 detectada especificamente");
        }
    }

    private void SelectSlot(int idx)
    {
        if (idx < 0 || idx >= slotHighlights.Length)
        {
            Debug.LogWarning($"Índice inválido: {idx}");
            return;
        }

        // Se já está selecionado, não faz nada
        if (idx == selectedIndex)
        {
            Debug.Log($"Slot {idx} já está selecionado");
            return;
        }

        // Desabilita highlight anterior
        if (selectedIndex >= 0 && selectedIndex < slotHighlights.Length)
        {
            if (slotHighlights[selectedIndex] != null)
            {
                slotHighlights[selectedIndex].gameObject.SetActive(false);
                Debug.Log($"Highlight do slot {selectedIndex} desativado");
            }
        }

        // Seleciona novo slot
        selectedIndex = idx;
        
        // Habilita novo highlight
        if (slotHighlights[selectedIndex] != null)
        {
            slotHighlights[selectedIndex].gameObject.SetActive(true);
            Debug.Log($"Highlight do slot {selectedIndex} ativado");
        }
        else
        {
            Debug.LogError($"Highlight do slot {selectedIndex} é null! Verifique a configuração no Inspector.");
        }

        // Sistema de prefabs
        HandlePrefabSelection();

        // Notifica outros sistemas
        OnSlotSelected?.Invoke(selectedIndex);
        
        RefreshUI();
    }

    private void HandlePrefabSelection()
    {
        if (!enablePrefabSystem)
            return;

        currentSelectedPrefab = null;
        
        if (selectedIndex >= 0 && selectedIndex < inventoryItems.Length)
        {
            ItemData selectedItem = inventoryItems[selectedIndex];
            if (selectedItem != null && selectedItem.itemPrefab != null)
            {
                currentSelectedPrefab = selectedItem.itemPrefab;
                OnPrefabSelected?.Invoke(currentSelectedPrefab);
                Debug.Log($"Prefab selecionado: {selectedItem.itemName}");
            }
        }
    }

    private void RefreshUI()
    {
        for (int i = 0; i < inventoryItems.Length && i < slotIcons.Length; i++)
        {
            ItemData item = inventoryItems[i];
            
            // Atualiza ícone
            if (slotIcons[i])
            {
                slotIcons[i].sprite = item?.itemSprite;
                slotIcons[i].enabled = item?.itemSprite != null;
            }

            // Atualiza contador
            if (slotCounts[i])
            {
                if (item != null && item.showCount)
                    slotCounts[i].text = item.count.ToString();
                else
                    slotCounts[i].text = "";
            }
        }
    }

    // Métodos públicos para manipular itens
    public void AddItem(int slotIndex, int amount = 1)
    {
        if (slotIndex >= 0 && slotIndex < inventoryItems.Length)
        {
            inventoryItems[slotIndex].count += amount;
            RefreshUI();
        }
    }

    public bool ConsumeItem(int slotIndex, int amount = 1)
    {
        if (slotIndex >= 0 && slotIndex < inventoryItems.Length)
        {
            if (inventoryItems[slotIndex].count >= amount)
            {
                inventoryItems[slotIndex].count -= amount;
                RefreshUI();
                return true;
            }
        }
        return false;
    }

    public void SetItemCount(int slotIndex, int count)
    {
        if (slotIndex >= 0 && slotIndex < inventoryItems.Length)
        {
            inventoryItems[slotIndex].count = count;
            RefreshUI();
        }
    }

    public int GetItemCount(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventoryItems.Length)
            return inventoryItems[slotIndex].count;
        return 0;
    }

    public GameObject GetSelectedPrefab()
    {
        return currentSelectedPrefab;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public ItemData GetSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < inventoryItems.Length)
            return inventoryItems[selectedIndex];
        return null;
    }

    // Métodos de compatibilidade com código antigo
    public void AddBandage(int v = 1)
    {
        AddItem(4, v);
        bandagens = GetItemCount(4);
    }

    public bool ConsumirMunicao(int v = 1)
    {
        return ConsumeAmmo(v);
    }

    public bool ConsumeAmmo(int v = 1)
    {
        bool result = ConsumeItem(5, v);
        if (result)
        {
            // Atualiza variável de compatibilidade se necessário
        }
        return result;
    }

    // Método para configurar item via código
    public void ConfigureItem(int slotIndex, string name, Sprite sprite, GameObject prefab, int count, bool showCount = true)
    {
        if (slotIndex >= 0 && slotIndex < inventoryItems.Length)
        {
            inventoryItems[slotIndex] = new ItemData
            {
                itemName = name,
                itemSprite = sprite,
                itemPrefab = prefab,
                count = count,
                showCount = showCount
            };
            RefreshUI();
        }
    }
}