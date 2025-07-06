using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Centraliza a criação de efeitos visuais sem depender de prefabs.
/// Permite gerar partículas ou linhas para diferentes habilidades em tempo de execução.
/// </summary>
public class FXManager : MonoBehaviour
{
    public static FXManager Instance { get; private set; }

    public enum FXType
    {
        Dust,
        Blood,
        Snow,
        EscudoColosso,
        Explosion,
        // Outros efeitos podem ser adicionados aqui
    }

    readonly Dictionary<FXType, Func<Vector3, Quaternion, Transform, GameObject>> builders =
        new Dictionary<FXType, Func<Vector3, Quaternion, Transform, GameObject>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        RegisterDefaults();
    }

    void RegisterDefaults()
    {
        Register(FXType.Dust, (p, r, parent) => CreateSimpleParticles("DustFX", p, r, parent, Color.gray, 1f, 20));
        Register(FXType.Blood, (p, r, parent) => CreateSimpleParticles("BloodFX", p, r, parent, Color.red, 1f, 30));
        Register(FXType.Snow, (p, r, parent) => CreateSimpleParticles("SnowFX", p, r, parent, Color.white, 2f, 10));
        Register(FXType.EscudoColosso, (p, r, parent) => CreateShield(p, r, parent));
        Register(FXType.Explosion, (p, r, parent) => CreateSimpleParticles("ExplosionFX", p, r, parent, new Color(1f, 0.5f, 0f), 1f, 40));
    }

    public void Register(FXType type, Func<Vector3, Quaternion, Transform, GameObject> builder)
    {
        builders[type] = builder;
    }

    public GameObject Play(FXType type, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        if (builders.TryGetValue(type, out var builder))
        {
            return builder.Invoke(position, rotation, parent);
        }
        Debug.LogWarning($"FX {type} not registered");
        return null;
    }

    static GameObject CreateSimpleParticles(string name, Vector3 pos, Quaternion rot, Transform parent, Color color, float lifetime, int rate)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent);
        go.transform.SetPositionAndRotation(pos, rot);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = lifetime;
        main.startColor = color;
        var emission = ps.emission;
        emission.rateOverTime = rate;
        GameObject.Destroy(go, lifetime + 0.5f);
        return go;
    }

    static GameObject CreateShield(Vector3 pos, Quaternion rot, Transform parent)
    {
        var go = new GameObject("FX_EscudoColosso");
        if (parent != null) go.transform.SetParent(parent);
        go.transform.SetPositionAndRotation(pos, rot);

        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 60;
        lr.loop = true;
        lr.useWorldSpace = false;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.color = new Color(0.2f, 0.8f, 1f, 0.7f);

        float angleStep = 360f / lr.positionCount;
        for (int i = 0; i < lr.positionCount; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            lr.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f));
        }

        GameObject.Destroy(go, 2f);
        return go;
    }
}
