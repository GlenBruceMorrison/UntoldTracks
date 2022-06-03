using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GUID))]
public class GuidDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var value0 = property.FindPropertyRelative("m_Value0");
        var value1 = property.FindPropertyRelative("m_Value1");
        var value2 = property.FindPropertyRelative("m_Value2");
        var value3 = property.FindPropertyRelative("m_Value3");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.SelectableLabel(position,$"{(uint)value0.intValue:X8}{(uint)value1.intValue:X8}{(uint)value2.intValue:X8}{(uint)value3.intValue:X8}");

        EditorGUI.EndProperty();
    }
}