using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEditor;
using System;
using System.Linq;

public class PropertyDrawerMethods 
{
    public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
    {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        if (obj == null) { return null; }

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

    
    private static GUIStyle s_TempStyle = new GUIStyle();

    public static void DrawTexture(Rect position, Texture2D texture) 
    {
      if (Event.current.type != EventType.Repaint)
        return;

      s_TempStyle.normal.background = texture;

      s_TempStyle.Draw(position, GUIContent.none, false, false, false, false);
    }
}
