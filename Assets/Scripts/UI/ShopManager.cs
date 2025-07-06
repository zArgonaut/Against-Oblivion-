using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// ShopManager exibe os itens compráveis entre as fases e aplica as melhorias
// escolhidas ao SaveData antes de carregar a próxima cena.
public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI txtScore;
    public Button upgradeWeaponBtn;
    public Button capacityBtn;
    public Button ammoBtn;

    public int weaponUpgradeCost = 200;
    public int capacityCost = 100;
    public int ammoCost = 50;

    public int capacityIncrease = 10;
    public int ammoIncrease = 20;

    InventoryManager inventory;
    UpgradeSystem upgradeSystem;
    int pontosInicio = 0;

    void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
        upgradeSystem = FindObjectOfType<UpgradeSystem>();

        if (ScoreManager.instance != null)
            pontosInicio = ScoreManager.instance.pontos;

        if (upgradeWeaponBtn) upgradeWeaponBtn.onClick.AddListener(() => ComprarItem("upgrade"));
        if (capacityBtn) capacityBtn.onClick.AddListener(() => ComprarItem("capacidade"));
        if (ammoBtn) ammoBtn.onClick.AddListener(() => ComprarItem("municao"));

        AtualizarUI();
        VerificarBagueteSecreto();
    }

    // Botão genérico chamado pelo Canvas
    public void ComprarItem(string id)
    {
        if (ScoreManager.instance == null) return;
        switch (id)
        {
            case "upgrade":
                if (ScoreManager.instance.SpendPoints(weaponUpgradeCost))
                    upgradeSystem?.AplicarUpgrade();
                break;
            case "capacidade":
                if (ScoreManager.instance.SpendPoints(capacityCost) && inventory != null && inventory.ArmaEquipada != null)
                    inventory.AumentarCapacidade(inventory.ArmaEquipada.tipo, capacityIncrease);
                break;
            case "municao":
                if (ScoreManager.instance.SpendPoints(ammoCost) && inventory != null && inventory.ArmaEquipada != null)
                    inventory.AdicionarMunicao(inventory.ArmaEquipada.tipo, ammoIncrease);
                break;
        }
        AtualizarUI();
        SalvarProgresso();
    }

    void AtualizarUI()
    {
        if (txtScore && ScoreManager.instance != null)
            txtScore.text = "Pontos: " + ScoreManager.instance.pontos;

        if (upgradeWeaponBtn) upgradeWeaponBtn.interactable = ScoreManager.instance.pontos >= weaponUpgradeCost;
        if (capacityBtn) capacityBtn.interactable = ScoreManager.instance.pontos >= capacityCost;
        if (ammoBtn) ammoBtn.interactable = ScoreManager.instance.pontos >= ammoCost;
    }

    void SalvarProgresso()
    {
        if (GameManager.Instance)
            GameManager.Instance.SaveGame(GameManager.Instance.CurrentSlot);
    }

    void VerificarBagueteSecreto()
    {
        if (ScoreManager.instance == null || inventory == null) return;
        if (SceneManager.GetActiveScene().buildIndex >= 4 && ScoreManager.instance.pontosGastos == 0)
        {
            foreach (var slot in inventory.armas)
            {
                if (slot != null && slot.tipo == WeaponType.Porrete)
                {
                    slot.tipo = WeaponType.PaoBaguete;
                    slot.municao = slot.capacidade;
                    break;
                }
            }
        }
    }

    public void FecharLoja()
    {
        SalvarProgresso();
        if (GameManager.Instance)
            GameManager.Instance.ExitShop();
    }
}
