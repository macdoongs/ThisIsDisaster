using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlurFilter : MonoBehaviour
{
    #region Inspector
    public Shader _shader;
    public float timeX = 1f;
    private Vector4 ScreenResolution;

    [Range(0,10)]
    public float Amount = 1f;
    [Range(0,1)]
    public float Glow = 1f;
    private Material _material;

    #endregion
    #region Properties
    Material Material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(_shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }
            return _material;
        }
    }
    #endregion

    private void Start()
    {
        _shader = Shader.Find("Custom/BlurShader");
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            _shader = Shader.Find("Custom/BlurShader");
        }
#endif
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_shader != null)
        {
            timeX += Time.deltaTime;
            if (timeX > 100f) timeX = 0f;
            Material.SetFloat("_TimeX", timeX);
            Material.SetFloat("_Amount", Amount);
            Material.SetFloat("_Glow", Glow);
            Material.SetVector("_ScreenResolution", new Vector4(source.width, source.height, 0.0f, 0.0f));
            Graphics.Blit(source, destination, Material);
        }
    }

    private void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
        }
    }
}
