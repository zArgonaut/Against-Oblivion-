using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public ProjectileController prefab;
    public int tamanhoInicial = 10;
    Queue<ProjectileController> pool = new Queue<ProjectileController>();

    void Awake()
    {
        for (int i = 0; i < tamanhoInicial; i++)
            CriarNovo();
    }

    ProjectileController CriarNovo()
    {
        var proj = Instantiate(prefab, transform);
        proj.gameObject.SetActive(false);
        proj.pool = this;
        pool.Enqueue(proj);
        return proj;
    }

    public ProjectileController Pegar()
    {
        if (pool.Count == 0)
            CriarNovo();
        var proj = pool.Dequeue();
        proj.gameObject.SetActive(true);
        return proj;
    }

    public void Devolver(ProjectileController proj)
    {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
}
