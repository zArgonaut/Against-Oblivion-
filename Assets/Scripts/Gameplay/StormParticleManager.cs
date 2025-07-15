using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class StormManager : MonoBehaviour
{
    [Header("Particle VFX Assets")]
    [Tooltip("Arraste aqui os VisualEffectAsset dos seus VFX Graph (ex: Sandstorm_FundoVFX).")]
    public VisualEffectAsset[] vfxAssets;

    [Header("Spawn Area (BoxCollider2D)")]
    [Tooltip("O BoxCollider2D deve estar em modo Trigger e define a área de spawn.")]
    public bool useColliderBounds = true;

    [Header("Spawn Settings")]
    [Tooltip("Quantas instâncias de FX spawnar a cada intervalo.")]
    public int spawnCount = 5;
    [Tooltip("Intervalo em segundos entre cada spawn.")]
    public float spawnInterval = 0.2f;

    private BoxCollider2D spawnArea;
    private Coroutine stormRoutine;

    void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        if (useColliderBounds && (spawnArea == null || !spawnArea.isTrigger))
            Debug.LogWarning($"{nameof(StormManager)} precisa de um BoxCollider2D em modo Trigger para definir a área!");
    }

    void Start()
    {
        StartStorm();
    }

    [ContextMenu("Start Storm")]
    public void StartStorm()
    {
        if (stormRoutine != null) StopCoroutine(stormRoutine);
        stormRoutine = StartCoroutine(StormRoutine());
    }

    [ContextMenu("Stop Storm")]
    public void StopStorm()
    {
        if (stormRoutine != null) StopCoroutine(stormRoutine);
    }

    private IEnumerator StormRoutine()
    {
        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
                SpawnVFX();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnVFX()
    {
        if (vfxAssets == null || vfxAssets.Length == 0)
            return;

        VisualEffectAsset asset = vfxAssets[Random.Range(0, vfxAssets.Length)];

        Vector3 spawnPos = transform.position;
        if (useColliderBounds && spawnArea != null)
        {
            Bounds b = spawnArea.bounds;
            spawnPos = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                transform.position.z
            );
        }
        else
        {
            spawnPos += new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f),
                0f
            );
        }

        GameObject go = new GameObject($"StormVFX_{asset.name}");
        go.transform.SetParent(transform, false);
        go.transform.position = spawnPos;

        var vfx = go.AddComponent<VisualEffect>();
        vfx.visualEffectAsset = asset;
        vfx.Play();
    }

    void OnDrawGizmosSelected()
    {
        if (!useColliderBounds) return;
        var col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
