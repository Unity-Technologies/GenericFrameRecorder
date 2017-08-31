using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(WEBMRecorderSettings))]
    public class WEBMRecorderSettingsEditor : RecorderEditorBase
    {

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            var pf = new PropertyFinder<WEBMRecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
        }

        protected override void OnEncodingGroupGui()
        {
            if (EditorGUILayout.PropertyField(serializedObject.FindProperty("m_WebmEncoderSettings"), new GUIContent("Encoding"), true))
            {
                EditorGUI.indentLevel++;
                base.OnEncodingGui();
                EditorGUI.indentLevel--;
            }
        }

        protected override EFieldDisplayState GetFieldDisplayState( SerializedProperty property)
        {
            if( property.name == "m_CaptureEveryNthFrame" )
                return EFieldDisplayState.Hidden;

            return base.GetFieldDisplayState(property);
        }
    }
}
