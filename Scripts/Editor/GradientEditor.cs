using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class GradientEditor : EditorWindow
{
    public enum EditMode { Gradient, Keys, Custom }

    public EditMode editMode;

    Vector2 keysScrollPos;

    private KR.Graphics.Gradient gradient;

    //Editor
    public class KeyData
    {
        public KR.Graphics.Gradient.GradientKey key;
        public bool foldout;
    }

    List<KeyData> keysData = new List<KeyData>();

    private Texture2D markerTex;

    private KeyData m_currentKeyData = null;

    public static void Init(KR.Graphics.Gradient gradient)
    {
        GradientEditor instance = GetWindow<GradientEditor>();
        instance.Setup(gradient);
    }

    public void Setup(KR.Graphics.Gradient gradient)
    {
        this.gradient = gradient;
        markerTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GradientSystem/Assets/Marker.png");
        for (int i = 0; i < gradient.keys.Count; i++)
        {
            KeyData keyData = new KeyData();
            keyData.key = gradient.keys[i];
            keysData.Add(keyData);
        }
    }

    void OnGUI()
    {
        DrawEditor();
        CheckEvents();
        Repaint();
    }

    void DrawEditor()
    {
        EditorGUILayout.LabelField("Gradient Editor", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        GradientGUI.DrawGradientLayout(gradient, GUILayout.Height(50));
        DrawMarkers();
        DrawKeys();

        if (GUILayout.Button("Add"))
        {
            KR.Graphics.Gradient.GradientKey key = new KR.Graphics.Gradient.GradientKey(Color.white, 1);
            gradient.keys.Add(key);

            KeyData keyData = new KeyData();
            keyData.key = key;
            keysData.Add(keyData);
        }
    }

    private void DrawMarkers()
    {
        for (int i = 0; i < gradient.keys.Count; i++)
        {
            DrawDragMarker(i);
        }

        GUILayout.Space(18);
    }

    private void DrawKeys()
    {
        keysScrollPos = EditorGUILayout.BeginScrollView(keysScrollPos);
        for (int i = 0; i < keysData.Count; i++)
        {
            KeyData keyData = keysData[i];

            GUILayout.BeginHorizontal();
            keyData.foldout = EditorGUILayout.Foldout(keyData.foldout, "Key " + i);
            if (gradient.keys.Count > 1)
            {
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    gradient.keys.RemoveAt(i);
                    keysData.RemoveAt(i);
                    break;
                }
            }
            GUILayout.EndHorizontal();

            if (keyData.foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                keyData.key.color = EditorGUILayout.ColorField("Color", keyData.key.color);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "Color");
                    //EditorUtility.SetDirty(gradient);
                }

                keyData.key.time = EditorGUILayout.Slider("Time", keyData.key.time, 0, 1);
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawDragMarker(int index)
    {
        KeyData keyData = keysData[index];

        Rect rect = GUILayoutUtility.GetLastRect();
        rect.x = keyData.key.time * (position.width - 16);
        rect.size = new Vector2(16, 16);

        if (m_currentKeyData == null && rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
        {
            m_currentKeyData = keyData;
        }

        EditorGUI.LabelField(rect, new GUIContent(markerTex));
    }

    private void CheckEvents()
    {
        if (m_currentKeyData == null) { return; }

        if (Event.current.type == EventType.MouseUp || EditorWindow.mouseOverWindow != this)
        {
            m_currentKeyData = null;
        }

        if (Event.current.type == EventType.MouseDrag)
        {
            float x = m_currentKeyData.key.time * (position.width - 16) + Event.current.delta.x;
            m_currentKeyData.key.time = Mathf.Clamp01(x / (position.width - 16f));
            gradient.SortKeys();
            keysData.Sort((p1, p2) => (p1.key.time.CompareTo(p2.key.time)));
        }   
    }
}
