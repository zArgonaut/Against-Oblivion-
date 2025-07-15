using UnityEngine;
using TMPro;

public static class PrefabFactory
{
    public static GameObject CreatePlayer()
    {
        GameObject go = new GameObject("Player");
        go.AddComponent<CharacterController>();
        go.AddComponent<PlayerMovement>();
        go.AddComponent<PlayerHealth>();
        go.AddComponent<PlayerEnergy>();
        go.AddComponent<PlayerStamina>();
        go.AddComponent<InventoryManager>();

        var weapon = go.AddComponent<WeaponController>();
        weapon.firePoint = new GameObject("FirePoint").transform;
        weapon.firePoint.parent = go.transform;

        var bulletPrefab = CreateProjectile("Bullet_Player");
        var poolGO = new GameObject("ProjectilePool");
        poolGO.transform.SetParent(go.transform);
        var pool = poolGO.AddComponent<ProjectilePool>();
        pool.prefab = bulletPrefab.GetComponent<ProjectileController>();
        weapon.projectilePrefab = pool.prefab;
        weapon.pool = pool;

        var melee = go.AddComponent<PlayerMeleeCombat>();
        melee.pontoAtaque = new GameObject("MeleePoint").transform;
        melee.pontoAtaque.parent = go.transform;

        return go;
    }

    public static GameObject CreateProjectile(string nome)
    {
        GameObject go = new GameObject(nome);
        var renderer = go.AddComponent<SpriteRenderer>();
        var sprite = Resources.Load<Sprite>(nome);
        renderer.sprite = sprite;
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        go.layer = LayerMask.NameToLayer("Default");
        go.AddComponent<ProjectileController>();
        return go;
    }

    public static GameObject CreateScoreHUD()
    {
        var go = new GameObject("ScoreHUD");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var textGO = new GameObject("TextoPontos");
        textGO.transform.SetParent(go.transform);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        var manager = go.AddComponent<ScoreManager>();
        manager.textoPontos = text;

        var energyGO = new GameObject("EnergyText");
        energyGO.transform.SetParent(go.transform);
        var energyText = energyGO.AddComponent<TextMeshProUGUI>();
        var hud = go.AddComponent<HUDController>();
        hud.energyText = energyText;

        return go;
    }

    public static GameObject CreateMiniMapa()
    {
        var go = new GameObject("MiniMapa");
        var areaGO = new GameObject("Area");
        areaGO.transform.SetParent(go.transform);
        var areaRect = areaGO.AddComponent<RectTransform>();

        var blipGO = new GameObject("Blip");
        blipGO.transform.SetParent(areaGO.transform);
        blipGO.AddComponent<UnityEngine.UI.Image>();
        blipGO.SetActive(false);

        var mini = go.AddComponent<MiniMapa>();
        mini.area = areaRect;
        mini.blipPrefab = blipGO;

        return go;
    }
}
