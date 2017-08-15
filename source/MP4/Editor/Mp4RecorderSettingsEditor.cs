using System;
using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(MP4RecorderSettings))]
    public class Mp4RecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;
            m_RTInputSelector = new RTInputSelector("Pixels", true);
            var pf = new PropertyFinder<MP4RecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField( serializedObject.FindProperty("m_MP4EncoderSettings"), new GUIContent("Encoding"), true);
        }

        protected override EFieldDisplayState GetFieldDisplayState( SerializedProperty property)
        {
            if( property.name == "m_CaptureEveryNthFrame" )
                return EFieldDisplayState.Hidden;

            return base.GetFieldDisplayState(property);
        }
    }
}
