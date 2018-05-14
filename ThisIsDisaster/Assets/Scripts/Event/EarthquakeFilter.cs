using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EarthquakeFilter : MonoBehaviour {
    #region Inspector
    public Shader _shader;
    public float timeX = 1f;
    private Vector4 ScreenResolution;
    private Material _material;

    [Range(0f,100f)]
    public float Speed = 15f;
    [Range(0f, 0.2f)]
    public float x = 0.0008f;
    [Range(0f, 0.2f)]
    public float y = 0.0008f;
    [Range(0f, 0.2f)]
    private float Value4 = 1f;
    #endregion
    #region Properties
    Material Material {
        get {
            if (_material == null) {
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
        _shader = Shader.Find("Custom/EarthquakeShader");
        if (!SystemInfo.supportsImageEffects) {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) {
            _shader = Shader.Find("Custom/EarthquakeShader");
        }
#endif
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_shader != null) {
            timeX += Time.deltaTime;
            if (timeX > 100f) timeX = 0f;
            Material.SetFloat("_TimeX", timeX);
            Material.SetFloat("_Value", Speed);
            Material.SetFloat("_Value2", x);
            Material.SetFloat("_Value3", y);
            Material.SetFloat("_Value4", Value4);
            Material.SetVector("_ScreenResolution", new Vector4(source.width, source.height, 0.0f, 0.0f));
            Graphics.Blit(source, destination, Material);
        }
    }

    private void OnDisable()
    {
        if (_material) {
            DestroyImmediate(_material);
        }
    }
}
