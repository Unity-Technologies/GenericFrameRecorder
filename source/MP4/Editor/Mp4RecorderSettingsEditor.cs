using System;
using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;
using UnityEngine.FrameRecorder;

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
            
            var pf = new PropertyFinder<MP4RecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField( serializedObject.FindProperty("m_MP4EncoderSettings"), new GUIContent("Encoding"), true);
        }

        protected override EFieldDisplayState GetFieldDisplayState( SerializedProperty property)
        {
            if( property.name == "m_CaptureEveryNthFrame" || property.name == "m_AllowTransparency" )
                return EFieldDisplayState.Hidden;
            return base.GetFieldDisplayState(property);
        }

    }
}
