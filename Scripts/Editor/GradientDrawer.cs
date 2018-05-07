using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

    [CustomPropertyDrawer(typeof(KR.Graphics.Gradient))]
    public class GradientDrawer : PropertyDrawer
    {
        Texture2D previewTexture = new Texture2D(256, 8);
        KR.Graphics.Gradient instance;
         
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawGradientTexture(property);
            DrawGradientVariable(position, property, label);
        }

        void DrawGradientVariable(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //Draw the label of the variable and set the position
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //Set the box position
            Rect boxRect = new Rect(position.x, position.y, position.width, 16);

            //Draw a Box behind the Gradient
            GUI.Box(boxRect, "");

            //Set the gradient position
            Rect gradientRect = new Rect(boxRect.position.x + 1, boxRect.position.y + 1, boxRect.width - 2, boxRect.height - 2);

            //Draw gradient texture with a workaround method
            PropertyDrawerMethods.DrawTexture(gradientRect, previewTexture);

            //Set the Color of the Editor GUI to transparent
            GUI.color = Color.clear;
            
            //Draw the button of the Gradient Editor
            if (GUI.Button(gradientRect, ""))
            {
                GradientEditor.Init(instance);
            }

            //Revert the Color of the Editor GUI
            GUI.color = Color.white;

            EditorGUI.EndProperty();
        }

        void DrawGradientTexture(SerializedProperty property)
        {
            if(instance == null) {
              instance = PropertyDrawerMethods.GetActualObjectForSerializedProperty<KR.Graphics.Gradient>(fieldInfo, property);
            }
      
            if (previewTexture == null)
            {
              previewTexture = new Texture2D(256, 8);
            }

            int width = previewTexture.width;
            int height = previewTexture.height;

            Color[] colors = new Color[width * height];

            Color bgColor = Color.clear;

            for (int i = 0; i < width; i++)
            {
                float t = Mathf.Clamp01(((float)i) / ((float)width));
                
                Color color = instance.Evaluate(t);

                colors[i] = color * (color.a) + bgColor * (1f - color.a);
                colors[i + width * 4] = color * (color.a) + bgColor * (1f - color.a);
            }

            for (int L = 0; L < height; L += 4)
            {
                for (int l = 0; l < 4; l++)
                {
                    if ((L % height / 2) < 4)
                        System.Array.Copy(colors, 0, colors, L * width + l * width, width);
                    else
                        System.Array.Copy(colors, width * 4, colors, L * width + l * width, width);
                }

            }

            previewTexture.SetPixels(0, 0, width, height, colors);
            previewTexture.Apply();
        }
    }
  