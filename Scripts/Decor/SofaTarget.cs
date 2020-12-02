using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SofaTarget : MonoBehaviour
{
    [SerializeField]
    Color color = Color.white;

    [SerializeField]
    MeshRenderer meshRenderer = null;

    void Start()
    {
        if (meshRenderer == null)
            return;

        var temp_color = meshRenderer.material.color;
        temp_color.r = color.r;
        temp_color.g = color.g;
        temp_color.b = color.b;
        meshRenderer.material.color = temp_color;
    }

    [ContextMenu("Set Color")]
    void SetColor()
    {
        if (meshRenderer != null && meshRenderer.sharedMaterial.color != Color.white)
        {
            color = meshRenderer.sharedMaterial.color;
            color.a = 255;
        }
    }
}
