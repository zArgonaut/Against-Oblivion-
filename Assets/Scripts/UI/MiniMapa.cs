using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMapa : MonoBehaviour
{
    [Header("Radar Settings")]
    public RectTransform area;
    public GameObject blipPrefab;
    public Transform player;
    public float range = 50f;

    Dictionary<Transform, RectTransform> blips = new Dictionary<Transform, RectTransform>();

    void Start()
    {
        if (player == null)
        {
            var p = FindObjectOfType<PlayerMovement>();
            if (p) player = p.transform;
        }
        CreateBlips();
    }

    void CreateBlips()
    {
        if (area == null || blipPrefab == null) return;

        foreach (var enemy in FindObjectsOfType<EnemyHealth>())
        {
            if (!blips.ContainsKey(enemy.transform))
            {
                var go = Instantiate(blipPrefab, area);
                blips[enemy.transform] = go.GetComponent<RectTransform>();
            }
        }
    }

    void Update()
    {
        if (player == null || area == null) return;

        // Ensure new enemies appear
        CreateBlips();

        float scale = area.rect.width / (range * 2f);
        var toRemove = new List<Transform>();

        foreach (var pair in blips)
        {
            if (pair.Key == null)
            {
                if (pair.Value != null)
                    Destroy(pair.Value.gameObject);
                toRemove.Add(pair.Key);
                continue;
            }

            Vector3 delta = pair.Key.position - player.position;
            Vector2 pos = new Vector2(delta.x, delta.z);
            if (pos.magnitude > range)
                pos = pos.normalized * range;
            pair.Value.anchoredPosition = pos * scale;
        }

        foreach (var rem in toRemove)
            blips.Remove(rem);
    }
}
