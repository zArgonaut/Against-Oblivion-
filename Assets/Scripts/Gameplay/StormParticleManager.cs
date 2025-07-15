using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class StormParticleManager : MonoBehaviour
{
    [Header("Particle Prefabs")]
    [Tooltip("Arraste aqui todos os prefabs de ParticleSystem para a tempestade.")]
    public GameObject[] particlePrefabs;

    [Header("Spawn Area (BoxCollider2D)")]
    [Tooltip("O BoxCollider2D deve estar em modo Trigger e define a área de spawn.")]
    public bool useColliderBounds = true;

    [Header("Spawn Settings")]
    [Tooltip("Quantas partículas spawnar a cada intervalo.")]
    public int spawnCount = 5;
    [Tooltip("Intervalo em segundos entre cada spawn.")]
    public float spawnInterval = 0.2f;

    private BoxCollider2D spawnArea;
    private Coroutine stormRoutine;

    void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        if (useColliderBounds && (spawnArea == null || !spawnArea.isTrigger))
            Debug.LogWarning($"{nameof(StormParticleManager)} precisa de um BoxCollider2D em modo Trigger para definir a área!");
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
            {
                SpawnParticle();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnParticle()
    {
        if (particlePrefabs == null || particlePrefabs.Length == 0)
            return;

        GameObject prefab = particlePrefabs[Random.Range(0, particlePrefabs.Length)];

        Vector3 spawnPos = transform.position;
        if (useColliderBounds && spawnArea != null)
        {
            Bounds b = spawnArea.bounds;
            spawnPos = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                spawnPos.z
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

        Instantiate(prefab, spawnPos, Quaternion.identity, transform);
    }

    void OnDrawGizmosSelected()
    {
        if (useColliderBounds)
        {
            var col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f);
                Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
