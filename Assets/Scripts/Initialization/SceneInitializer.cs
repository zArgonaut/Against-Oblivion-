using UnityEngine;

// SceneInitializer monta programaticamente cada fase ao iniciar a cena.
// Ele instancia terreno, luz, clima opcional, jogador, spawners e HUD.
public class SceneInitializer : MonoBehaviour
{
    public enum SceneType { Fase1_Deserto, Fase2_Floresta, Fase3_Montanhas, Fase4_Complexo, Modo_Horda }

    [Header("Configuração da Fase")]
    public SceneType tipoCena = SceneType.Fase1_Deserto;
    public bool ativarClima = true;

    [Header("Prefabs Básicos")]
    public GameObject playerPrefab;
    public GameObject spawnerPrefab;
    public GameObject hudPrefab;
    public GameObject fxManagerPrefab;

    void Awake()
    {
        CriarTerreno();
        CriarIluminacao();
        if (ativarClima)
            CriarClima();
        CriarPlayer();
        CriarSpawner();
        CriarHUD();
        CriarBossTrigger();
    }

    // Cria um plano simples representando o terreno da fase
    void CriarTerreno()
    {
        GameObject terreno = GameObject.CreatePrimitive(PrimitiveType.Plane);
        terreno.name = "Terreno";
        terreno.transform.localScale = new Vector3(10f, 1f, 10f); // 100x100

        var renderer = terreno.GetComponent<Renderer>();
        Material mat = CarregarMaterial();
        if (renderer != null && mat != null)
            renderer.material = mat;
    }

    Material CarregarMaterial()
    {
        string matPath = "Materials/Areia";
        switch (tipoCena)
        {
            case SceneType.Fase2_Floresta:
                matPath = "Materials/Neve";
                break;
            case SceneType.Fase3_Montanhas:
                matPath = "Materials/Rocha";
                break;
            case SceneType.Fase4_Complexo:
                matPath = "Materials/Metal";
                break;
        }
        return Resources.Load<Material>(matPath);
    }

    // Define luz principal e ambiente
    void CriarIluminacao()
    {
        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(60f, 45f, 0f);

        switch (tipoCena)
        {
            case SceneType.Fase1_Deserto:
                light.color = new Color(1f, 0.95f, 0.8f);
                RenderSettings.ambientLight = new Color(0.8f, 0.7f, 0.5f);
                break;
            case SceneType.Fase2_Floresta:
                light.color = new Color(0.8f, 0.9f, 1f);
                RenderSettings.ambientLight = new Color(0.6f, 0.7f, 0.9f);
                break;
            case SceneType.Fase3_Montanhas:
                light.color = new Color(1f, 0.6f, 0.5f);
                RenderSettings.ambientLight = new Color(0.5f, 0.4f, 0.4f);
                break;
            case SceneType.Fase4_Complexo:
                light.color = Color.white * 0.8f;
                RenderSettings.ambientLight = Color.gray * 0.2f;
                break;
        }
    }

    // Instancia efeitos climáticos opcionais
    void CriarClima()
    {
        GameObject prefab = null;
        switch (tipoCena)
        {
            case SceneType.Fase1_Deserto:
                prefab = Resources.Load<GameObject>("Prefabs/FX/TempestadeAreia");
                break;
            case SceneType.Fase2_Floresta:
                prefab = Resources.Load<GameObject>("Prefabs/FX/Nevasca");
                break;
            case SceneType.Fase3_Montanhas:
                prefab = Resources.Load<GameObject>("Prefabs/FX/PoeiraPedra");
                break;
            case SceneType.Fase4_Complexo:
                prefab = Resources.Load<GameObject>("Prefabs/FX/Faiscas");
                break;
        }

        if (prefab == null && fxManagerPrefab != null)
            prefab = fxManagerPrefab;

        if (prefab != null)
        {
            var clima = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            clima.SetActive(false); // inicia desativado
        }
    }

    // Instancia jogador no centro
    void CriarPlayer()
    {
        if (FindObjectOfType<PlayerMovement>() != null)
            return;

        GameObject prefab = playerPrefab != null ? playerPrefab : PrefabFactory.CreatePlayer();
        if (prefab != null)
            Instantiate(prefab, new Vector3(0f, 1f, 0f), Quaternion.identity);
    }

    // Cria spawner de inimigos ao fundo
    void CriarSpawner()
    {
        if (spawnerPrefab == null)
            spawnerPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Spawner");
        if (spawnerPrefab == null)
            return;

        Instantiate(spawnerPrefab, new Vector3(0f, 0f, 50f), Quaternion.identity);
    }

    // Instancia HUD principal
    void CriarHUD()
    {
        if (hudPrefab == null)
            hudPrefab = Resources.Load<GameObject>("Prefabs/UI/CanvasPrincipal");
        if (hudPrefab == null) hudPrefab = PrefabFactory.CreateScoreHUD();

        GameObject hudObj = null;
        if (FindObjectOfType<ScoreManager>() == null && hudPrefab != null)
            hudObj = Instantiate(hudPrefab);

        if (hudObj != null)
        {
            var canvas = hudObj.GetComponentInChildren<Canvas>();
            if (canvas == null) canvas = hudObj.GetComponent<Canvas>();

            GameObject miniPrefab = Resources.Load<GameObject>("Prefabs/UI/MiniMapa");
            GameObject miniInstance = null;
            if (miniPrefab != null)
                miniInstance = Instantiate(miniPrefab, canvas != null ? canvas.transform : hudObj.transform);
            else
            {
                miniInstance = PrefabFactory.CreateMiniMapa();
                if (canvas != null)
                    miniInstance.transform.SetParent(canvas.transform, false);
            }
        }
    }

    // Gatilho de boss no final
    void CriarBossTrigger()
    {
        var triggerObj = new GameObject("BossTrigger");
        triggerObj.transform.position = new Vector3(0f, 0f, tipoCena == SceneType.Fase2_Floresta ? 90f : tipoCena == SceneType.Fase3_Montanhas ? 100f : 80f);
        triggerObj.AddComponent<BoxCollider>().isTrigger = true;
        triggerObj.AddComponent<BossTrigger>();
    }
}
