using UnityEngine;
using UnityEngine.FrameRecorder;

namespace UnityEditor.FrameRecorder
{
    [CustomPropertyDrawer(typeof(OutputPath))]
    class OutputPathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float rootWidth = 70;
            float btnWidth = 10;
            float leafWidth = position.width - rootWidth -  btnWidth - 10;
            Rect rootRect = new Rect(position.x, position.y, rootWidth, position.height);
            Rect leafRect = new Rect(position.x + rootWidth + 5, position.y, leafWidth, position.height);
            Rect btnRect = new Rect(position.x + rootWidth + 5 + leafWidth, position.y, btnWidth, position.height);

            EditorGUI.PropertyField(rootRect, property.FindPropertyRelative("m_root"), GUIContent.none);
            EditorGUI.PropertyField(leafRect, property.FindPropertyRelative("m_leaf"), GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
