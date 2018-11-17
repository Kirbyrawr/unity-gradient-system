using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(KR.Graphics.Gradient))]
public class GradientPropertyDrawer : PropertyDrawer
{
    private KR.Graphics.Gradient gradient;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawGradient(position, property, label); 
    }

    private void DrawGradient(Rect position, SerializedProperty property, GUIContent label)
    {
        //Update gradient
        DrawGradientPreviewTexture(property);

        EditorGUI.BeginProperty(position, label, property);

        //Draw the label of the variable and set the position
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Set the box position
        Rect boxRect = new Rect(position.x, position.y, position.width, 16);

        //Draw a Box behind the Gradient
        GUI.Box(boxRect, "");

        //Set the gradient position
        Rect gradientRect = new Rect(boxRect.position.x + 0.5f, boxRect.position.y + 1f, boxRect.width - 1, boxRect.height - 1.5f);

        //Draw gradient texture with a workaround method
        GradientGUI.DrawTexture(gradientRect, gradient.preview);

        //Set the Color of the Editor GUI to transparent
        GUI.color = Color.clear;

        //Draw the button of the Gradient Editor
        if (GUI.Button(gradientRect, ""))
        {
            GradientEditor.Init(gradient);
        }

        //Revert the Color of the Editor GUI
        GUI.color = Color.white;

        EditorGUI.EndProperty();
    }

    private void DrawGradientPreviewTexture(SerializedProperty property)
    {
        if (gradient == null)
        {
            gradient = GetActualObjectForSerializedProperty<KR.Graphics.Gradient>(fieldInfo, property);
        }

        GradientGUI.DrawGradientPreviewTexture(gradient);
    }

    private T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
    {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);

        if (obj == null)
        {
            return null;
        }

        T actualObject = null;

        if (obj.GetType().IsArray)
        {
            var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
            actualObject = ((T[])obj)[index];
        }
        else
        {
            actualObject = obj as T;
        }

        return actualObject;
    }
}
