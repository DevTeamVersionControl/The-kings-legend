using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class Tray : MonoBehaviour
{
    private static readonly int EmissionStrength = Shader.PropertyToID("_emissionStrength");
    public List<Tile> tiles;
    [SerializeField] private Light light;
    private Material material;
    private bool _locked = false;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    public void SetLocked(bool locked)
    {
        _locked = locked;
        foreach (var tile in tiles)
        {
            tile.SetLocked(locked);
        }
        material.SetFloat(EmissionStrength, locked ? 0f : 1f);
        if (light != null)
        {
            light.enabled = !locked;
        }
    }

    public bool GetLocked()
    {
        return _locked;
    }

    public void SetLight(bool isOn)
    {
        material.SetFloat(EmissionStrength, isOn ? 1f : 0f);
    }
}
