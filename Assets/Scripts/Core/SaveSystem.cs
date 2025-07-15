using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int faseAtual;
    public int pontos;
    public GameManager.Difficulty dificuldade;
    // Inventory and upgrades
    public WeaponType[] weaponSlots;
    public int[] weaponAmmo;
    public int bandagens;
    public int powerUps;
    public int weaponTier;
    public int[] ammoCapacidade;
}

public static class SaveSystem
{
    const string Slot1 = "SaveSlot1";
    const string Slot2 = "SaveSlot2";

    public static void Save(int slot, SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(slot == 0 ? Slot1 : Slot2, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load(int slot)
    {
        string key = slot == 0 ? Slot1 : Slot2;
        if (!PlayerPrefs.HasKey(key)) return null;
        string json = PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(json)) return null;
        return JsonUtility.FromJson<SaveData>(json);
    }
}
