using System;
using UnityEngine;

public class PlayerWeaponSwitcher : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Ponto vazio dentro de RightHand onde a arma será parented")]
    [SerializeField] private Transform weaponMount;

    [Tooltip("Prefabs das armas, na mesma ordem dos slots (0 = rifle, 1 = shotgun, ...)")]
    [SerializeField] private GameObject[] weaponPrefabs = new GameObject[6];

    private GameObject currentWeapon;

    private void Awake()
    {
        if (weaponMount.childCount > 0)
            foreach (Transform c in weaponMount) Destroy(c.gameObject);
    }

    private void OnEnable()
    {
        InventoryManager.OnSlotChangedStatic += HandleSlotChanged;
    }

    private void OnDisable()
    {
        InventoryManager.OnSlotChangedStatic -= HandleSlotChanged;
    }

    private void HandleSlotChanged(int slotIndex)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        var prefab = weaponPrefabs[slotIndex];
        if (prefab != null)
        {
            currentWeapon = Instantiate(prefab, weaponMount);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }
}
