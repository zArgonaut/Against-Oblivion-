using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AutoFitCollider : MonoBehaviour
{
    [Header("Terrain Prefab (ou Instância)")]
    [Tooltip("Arraste aqui o GameObject que contém o componente Terrain na cena.")]
    public GameObject terrainObject;

    void Start()
    {
        if (terrainObject == null)
        {
            Debug.LogWarning($"{nameof(AutoFitCollider)}: nenhum Terrain atribuído.");
            return;
        }

        var terrain = terrainObject.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning($"{nameof(AutoFitCollider)}: o objeto atribuído não possui componente Terrain.");
            return;
        }

        Vector3 terrainSize = terrain.terrainData.size;

        var col2D = GetComponent<BoxCollider2D>();
        col2D.size = new Vector2(terrainSize.x, terrainSize.z);

        Vector3 terrainCenterWorld = terrainObject.transform.position + new Vector3(
            terrainSize.x * 0.5f,
            0f,
            terrainSize.z * 0.5f
        );

        Vector3 selfPos = transform.position;
        col2D.offset = new Vector2(
            terrainCenterWorld.x - selfPos.x,
            terrainCenterWorld.z - selfPos.z
        );
    }

    void OnDrawGizmosSelected()
    {
        var col2D = GetComponent<BoxCollider2D>();
        if (col2D != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            var center = (Vector3)col2D.offset + transform.position;
            var size = new Vector3(col2D.size.x, col2D.size.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
