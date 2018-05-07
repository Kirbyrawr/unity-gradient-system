using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class GradientEditor : EditorWindow
{
    public enum EditMode { Gradient, Keys, Custom }

    public EditMode editMode;

    Vector2 keysScrollPos;

    KR.Graphics.Gradient gradient;

    //Editor
   List<bool> foldouts = new List<bool>();

    public static void Init(KR.Graphics.Gradient gradient)
    {
        GradientEditor instance = GetWindow<GradientEditor>();
        instance.Setup(gradient);
    }

    public void Setup(KR.Graphics.Gradient gradient)
    {
        this.gradient = gradient;
        for (int i = 0; i < gradient.keys.Count; i++)
        {
            foldouts.Add(false);
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Gradient Mode"))
        {
            editMode = EditMode.Gradient;
        }

        if (GUILayout.Button("Keys Mode"))
        {
            editMode = EditMode.Keys;
        }
        GUILayout.EndHorizontal();

        switch (editMode)
        {
            case EditMode.Gradient:
                DrawGradientMode();
                break;

            case EditMode.Keys:
                DrawKeysMode();
                break;
        }
        Repaint();
    }

    void DrawGradientMode()
    {

    }

    void DrawKeysMode()
    {
        keysScrollPos = EditorGUILayout.BeginScrollView(keysScrollPos);
        for (int i = 0; i < gradient.keys.Count; i++)
        {
            GUILayout.BeginHorizontal();
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "Key " + i);
            if (gradient.keys.Count > 1)
            {
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    gradient.keys.RemoveAt(i);
                    foldouts.RemoveAt(i);
                    break;
                }
            }
            GUILayout.EndHorizontal();

            if (foldouts[i])
            {
                EditorGUI.indentLevel++;
                gradient.keys[i].color = EditorGUILayout.ColorField("Color", gradient.keys[i].color);
                gradient.keys[i].time = EditorGUILayout.Slider("Time", gradient.keys[i].time, 0, 1);
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add"))
        {
            foldouts.Add(false);
            gradient.keys.Add(new KR.Graphics.Gradient.GradientKey(Color.white, 1));
        }
    }

}
