using UnityEngine;
using TMPro;

/// <summary>
/// Factory responsável por criar objetos em tempo de execução
/// quando os prefabs de exemplo não estão presentes no projeto.
/// Isso permite que as cenas continuem funcionais mesmo sem os
/// assets originais exportados do Unity.
/// </summary>
public static class PrefabFactory
{
    // Cria o jogador completo com scripts e pontos de tiro
    public static GameObject CreatePlayer()
    {
        GameObject go = new GameObject("Player");
        var sprite = Resources.Load<Sprite>("Player_Masc");
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        go.AddComponent<CharacterController>();
        go.AddComponent<PlayerMovement>();
        go.AddComponent<PlayerHealth>();
        go.AddComponent<PlayerEnergy>();
        go.AddComponent<PlayerStamina>();
        go.AddComponent<InventoryManager>();

        var weapon = go.AddComponent<WeaponController>();
        weapon.firePoint = new GameObject("FirePoint").transform;
        weapon.firePoint.parent = go.transform;

        // Cria projétil e pool para armas de fogo
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

    // Armas usadas pelo jogador
    public static GameObject CreateWeapon(WeaponType tipo)
    {
        GameObject go = new GameObject(tipo.ToString());
        var sprite = Resources.Load<Sprite>(tipo.ToString());
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        go.AddComponent<BoxCollider>();
        go.AddComponent<WeaponController>();
        return go;
    }

    // Itens de pickup espalhados pelo cenário
    public static GameObject CreateItem(TipoItem tipo)
    {
        GameObject go = new GameObject("Item_" + tipo);
        var sprite = Resources.Load<Sprite>("Icon_" + tipo);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        var item = go.AddComponent<ItemPickup>();
        item.tipo = tipo;
        item.valor = 10;
        return go;
    }

    // Projétil genérico
    public static GameObject CreateProjectile(string nome)
    {
        GameObject go = new GameObject(nome);
        var sprite = Resources.Load<Sprite>(nome);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        go.layer = LayerMask.NameToLayer("Default");
        go.AddComponent<ProjectileController>();
        return go;
    }

    // Enemies básicos para testes
    public static GameObject CreateEnemy(string nome)
    {
        GameObject go = new GameObject(nome);
        var sprite = Resources.Load<Sprite>(nome);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        go.AddComponent<Rigidbody>();
        go.AddComponent<BoxCollider>();
        go.AddComponent<EnemyHealth>();
        go.AddComponent<EnemyPatrol>();
        var atk = go.AddComponent<EnemyAttack>();
        atk.pontoAtaque = new GameObject("AttackPoint").transform;
        atk.pontoAtaque.parent = go.transform;
        return go;
    }

    // HUD simples para exibir pontuação
    public static GameObject CreateScoreHUD()
    {
        var go = new GameObject("ScoreHUD");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var textGO = new GameObject("TextoPontos");
        textGO.transform.SetParent(go.transform);
        var text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
        var manager = go.AddComponent<ScoreManager>();
        manager.textoPontos = text;

        var energyGO = new GameObject("EnergyText");
        energyGO.transform.SetParent(go.transform);
        var energyText = energyGO.AddComponent<TMPro.TextMeshProUGUI>();
        var hud = go.AddComponent<HUDController>();
        hud.energyText = energyText;

        return go;
    }

    // Painel administrativo básico para debug
    public static GameObject CreateAdminPanel()
    {
        var go = new GameObject("AdminDebugPanel");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<AdminDebugPanel>();
        return go;
    }

    // Mini mapa simples caso o prefab não exista
    public static GameObject CreateMiniMapa()
    {
        var go = new GameObject("MiniMapa");
        var areaGO = new GameObject("Area");
        areaGO.transform.SetParent(go.transform);
        var areaRect = areaGO.AddComponent<RectTransform>();

        var blipGO = new GameObject("Blip");
        blipGO.transform.SetParent(areaGO.transform);
        var img = blipGO.AddComponent<UnityEngine.UI.Image>();
        blipGO.SetActive(false);

        var mini = go.AddComponent<MiniMapa>();
        mini.area = areaRect;
        mini.blipPrefab = blipGO;

        return go;
    }
}
