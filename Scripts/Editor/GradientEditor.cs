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
        public bool foldout;
        public bool pressed;
    }

    List<KeyData> keysData = new List<KeyData>();

    private Texture2D markerTex;

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
            keysData.Add(new KeyData());
        }
    }

    void OnGUI()
    {
        DrawEditor();
        Repaint();
    }

    void DrawEditor()
    {
        EditorGUILayout.LabelField("Gradient Editor", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        GradientGUI.DrawGradientLayout(gradient, GUILayout.Height(50));   
        DrawMarkers();
        

        keysScrollPos = EditorGUILayout.BeginScrollView(keysScrollPos);
        for (int i = 0; i < gradient.keys.Count; i++)
        {
            KR.Graphics.Gradient.GradientKey key = gradient.keys[i];
            KeyData keyData = keysData[i];

            GUILayout.BeginHorizontal();
            keyData.foldout = EditorGUILayout.Foldout(keyData.foldout, "Key " + i);
            if (gradient.keys.Count > 1)
            {
                if (i != 0)
                {
                    if (GUILayout.Button("↑", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Swap(gradient.keys, i, i - 1);
                    }
                }

                if (i != gradient.keys.Count - 1)
                {
                    if (GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Swap(gradient.keys, i, i + 1);
                    }
                }

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
                key.color = EditorGUILayout.ColorField("Color", key.color);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "Color");
                    //EditorUtility.SetDirty(gradient);
                }

                key.time = EditorGUILayout.Slider("Time", key.time, 0, 1);
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add"))
        {
            keysData.Add(new KeyData());
            gradient.keys.Add(new KR.Graphics.Gradient.GradientKey(Color.white, 1));
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

    private void DrawDragMarker(int index)
    {
        KeyData keyData = keysData[index];
        KR.Graphics.Gradient.GradientKey key = gradient.keys[index];

        Rect rect = GUILayoutUtility.GetLastRect();
        rect.x = key.time * (position.width - 16);
        rect.size = new Vector2(16, 16);

        if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
        {
            keyData.pressed = true;
        }

        if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseLeaveWindow)
        {
            keyData.pressed = false;
        }

        if (keyData.pressed && Event.current.type == EventType.MouseDrag)
        {
            float x = rect.x + Event.current.delta.x;
            key.time = Mathf.Clamp01(x / (position.width - 16f));
        }

        GUI.Button(rect, markerTex, EditorStyles.label);
    }

    public void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
}
