using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditorInternal;
using System.Reflection;

#if UNITY_EDITOR
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

    void UpdateInfo(Transform tr)
    {
        foreach (Transform t in tr) {
            var sr = t.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sortingLayerName = ReferenceRenderer.sortingLayerName;
                sr.sortingOrder = ReferenceRenderer.sortingOrder;
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
        m_object.Update();
        DrawDefaultInspector();

        var options = GetSortingLayerNames();
        var picks = new int[options.Length];
        var name = renderer.sortingLayerName;
        var choice = -1;
        for (int i = 0; i < options.Length; i++) {
            picks[i] = i;
            if (name == options[i]) choice = i;
        }

        choice = EditorGUILayout.IntPopup("Sorting Layer", choice, options, picks);
        renderer.sortingLayerName = options[choice];
        renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);

        if (GUILayout.Button("Update Info")) {
            Changer.UpdateLayerInfo();
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
