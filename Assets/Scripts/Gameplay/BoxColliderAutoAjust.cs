using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class AutoFitCollider : MonoBehaviour
{
    [Tooltip("Arraste aqui seu Tilemap (ou Sprite) que define a fase.")]
    public Tilemap tilemap;

    void Start()
    {
        if (tilemap == null) return;

        Bounds b = tilemap.localBounds;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.size = new Vector2(b.size.x, b.size.y);

        col.offset = b.center;
    }
}
