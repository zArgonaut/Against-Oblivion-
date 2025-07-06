using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public ProjectileController projectilePrefab;
    public ProjectilePool pool;
    public Transform firePoint;
    public float fireRate = 0.5f;
    public int damage = 25;
    public int energyCost = 10;

    private float fireTimer = 0f;
    private PlayerEnergy energy;
    private InventoryManager inventory;

    void Start()
    {
        energy = GetComponent<PlayerEnergy>();
        inventory = GetComponent<InventoryManager>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && fireTimer <= 0f)
        {
            if (energy == null || energy.Consume(energyCost))
            {
                if (inventory == null || inventory.ConsumirMunicao())
                {
                    Shoot();
                    fireTimer = fireRate;
                }
            }
        }
    }

    void Shoot()
    {
        if (pool != null && firePoint != null)
        {
            ProjectileController proj = pool.Pegar();
            proj.transform.position = firePoint.position;
            proj.transform.rotation = firePoint.rotation;
            proj.dano = damage;
        }
    }
}
