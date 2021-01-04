using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Cache : MonoBehaviour
{
    public Color Transparent;
    public Color Black;
    public Material Mask;
    public float Duration = .5f;
    public MeshRenderer mesh;

    private void OnEnable()
    {
        
        Tween.Color(Mask, Transparent, Duration,0f);
        Invoke("Disable", Duration);
    }

    private void OnDisable()
    {
        mesh.enabled = true;
        Tween.Color(Mask, Black, Duration,0f);        
    }

    void Disable()
    {
        mesh.enabled = false;
    }
}
