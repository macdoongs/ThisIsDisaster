using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Reflection;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu("Utils/RenderLayerChanger")]
[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class RenderLayerChanger : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer ReferenceRenderer;

    private void Start()
    {
        ReferenceRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateLayerInfo() {
        UpdateInfo(transform);
    }

    public void UpdateLayerInfo(int order) {
        ReferenceRenderer.sortingOrder = order;
        UpdateLayerInfo();  
    }

    void UpdateInfo(Transform tr)
    {
        foreach (Transform t in tr) {
            var fixer = t.GetComponent<RenderLayerFixer>();

            if (fixer != null)
            {
                if (fixer.IgnoreChilds)
                {
                    return;
                }
            }
            else {
                var sr = t.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = ReferenceRenderer.sortingLayerName;
                    sr.sortingOrder = ReferenceRenderer.sortingOrder;
                }
                var ps = t.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var ps_Render = ps.GetComponent<ParticleSystemRenderer>();
                    ps_Render.sortingLayerName = ReferenceRenderer.sortingLayerName;
                    ps_Render.sortingOrder = ReferenceRenderer.sortingOrder;
                }
            }

            UpdateInfo(t);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RenderLayerChanger)), CanEditMultipleObjects]
public class RenderLayerEditor : Editor {
    private SerializedObject m_object;

    RenderLayerChanger Changer
    {
        get { return target as RenderLayerChanger; }
    }

    SpriteRenderer renderer {
        get { return Changer.ReferenceRenderer; }
    }

    public void OnEnable()
    {
        m_object = new SerializedObject(targets);
    }

    public override void OnInspectorGUI()
    {
        try
        {
            m_object.Update();
            DrawDefaultInspector();

            var options = GetSortingLayerNames();
            var picks = new int[options.Length];
            var name = renderer.sortingLayerName;
            var choice = -1;
            for (int i = 0; i < options.Length; i++)
            {
                picks[i] = i;
                if (name == options[i]) choice = i;
            }

            choice = EditorGUILayout.IntPopup("Sorting Layer", choice, options, picks);
            renderer.sortingLayerName = options[choice];
            renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);

            if (GUILayout.Button("Update Info"))
            {
                Changer.UpdateLayerInfo();
            }
        }
        catch {

        }
    }

    public string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }
}
#endif
