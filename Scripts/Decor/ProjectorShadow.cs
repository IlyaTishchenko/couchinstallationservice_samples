using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for rendering fake shadows to texture under the mesh
public class ProjectorShadow : MonoBehaviour
{
    [SerializeField]
    public Camera ShadowCamera = null;

    [SerializeField]
    public GameObject ShadowOrigin = null;

    [SerializeField]
    public bool AnimatedObject = false;

    [SerializeField]
    public int TextureSize = 256;

    [SerializeField]
    Material shadowMaterialResource = null;

    [SerializeField]
    Shader unlitShader = null;

    void Start()
    {
        ShadowCamera.enabled = false;

        targetTexture = new RenderTexture(TextureSize, TextureSize, 16);
        ShadowCamera.targetTexture = targetTexture;

        if (shadowMaterialResource == null)
            return;

        var shadowMaterial = new Material(shadowMaterialResource);
        shadowMaterial.SetTexture("_MainTex", targetTexture);

        if (ShadowOrigin == null)
            return;

        // Generating a quad mesh
        var quad = new Mesh();
        var vertices = new List<Vector3>();
        vertices.Add(new Vector3(-1.0f, 0.0f,  1.0f) * ShadowCamera.orthographicSize);
        vertices.Add(new Vector3( 1.0f, 0.0f,  1.0f) * ShadowCamera.orthographicSize);
        vertices.Add(new Vector3( 1.0f, 0.0f, -1.0f) * ShadowCamera.orthographicSize);
        vertices.Add(new Vector3(-1.0f, 0.0f, -1.0f) * ShadowCamera.orthographicSize);
        var uvs = new List<Vector2>();
        uvs.Add(new Vector2(0.0f, 1.0f));
        uvs.Add(new Vector2(1.0f, 1.0f));
        uvs.Add(new Vector2(1.0f, 0.0f));
        uvs.Add(new Vector2(0.0f, 0.0f));
        var triangles = new List<int>{0, 1, 3, 1, 2, 3};
        quad.vertices = vertices.ToArray();
        quad.triangles = triangles.ToArray();
        quad.uv = uvs.ToArray();

        quad.RecalculateNormals();


        var meshFilter = ShadowOrigin.AddComponent<MeshFilter>();
        var meshRenderer = ShadowOrigin.AddComponent<MeshRenderer>();

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        meshFilter.mesh = quad;
        meshRenderer.material = shadowMaterial;

        RenderShadow();
    }
    RenderTexture targetTexture = null;

    void FixedUpdate()
    {
        if (AnimatedObject)
        {
            RenderShadow();
        }
    }

    // Actual shadow texture rendering
    void RenderShadow()
    {
        if (ShadowCamera == null || unlitShader == null)
            return;

        ShadowCamera.RenderWithShader(unlitShader, "");
    }
}
