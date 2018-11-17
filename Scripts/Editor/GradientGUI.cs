using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GradientGUI 
{
	private static GUIStyle tempStyle = new GUIStyle();

    public static void DrawGradientLayout(KR.Graphics.Gradient gradient, params GUILayoutOption[] options) 
	{ 
		Rect rect = EditorGUILayout.GetControlRect(options);
        EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(rect.width), GUILayout.Height(rect.height));
        DrawGradient(rect, gradient);
	}

	public static void DrawGradient(Rect rect, KR.Graphics.Gradient gradient) 
	{
		DrawGradientPreviewTexture(gradient);

        //Set the gradient position
        Rect gradientRect = new Rect(rect.position.x, rect.position.y, rect.width, rect.height);

        //Draw gradient texture with a workaround method
        DrawTexture(gradientRect, gradient.preview);

        //Set the Color of the Editor GUI to transparent
        GUI.color = Color.clear;

        //Draw the button of the Gradient Editor
        if (GUI.Button(gradientRect, ""))
        {
            GradientEditor.Init(gradient);
        }

        //Revert the Color of the Editor GUI
        GUI.color = Color.white;
	}

	public static void DrawGradientPreviewTexture(KR.Graphics.Gradient gradient)
    {
        if(gradient.preview == null) 
        {
            gradient.preview = new Texture2D(256, 20);
        }

        int width = gradient.preview.width;
        int height = gradient.preview.height;

        Color[] colors = new Color[width * height];

        Color bgColor = Color.clear;

        for (int i = 0; i < width; i++)
        {
            float t = Mathf.Clamp01(((float)i) / ((float)width));

            Color color = gradient.Evaluate(t);

            colors[i] = color * (color.a) + bgColor * (1f - color.a);
            colors[i + width * 4] = color * (color.a) + bgColor * (1f - color.a);
        }

        for (int L = 0; L < height; L += 4)
        {
            for (int l = 0; l < 4; l++)
            {
                if ((L % height / 2) < 4)
                {
                    Array.Copy(colors, 0, colors, L * width + l * width, width);
                }
                else
                {
                    Array.Copy(colors, width * 4, colors, L * width + l * width, width);
                }
            }

        }

        gradient.preview.SetPixels(0, 0, width, height, colors);
        gradient.preview.Apply();
    }

	public static void DrawTexture(Rect position, Texture2D preview)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        tempStyle.normal.background = preview;

        tempStyle.Draw(position, GUIContent.none, false, false, false, false);
    }
}
