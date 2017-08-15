using System;
using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(PNGRecorderSettings))]
    public class PngRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;
            m_RTInputSelector = new RTInputSelector("Pixels", false);
            var pf = new PropertyFinder<PNGRecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PngEncoderSettings"), new GUIContent("Encoding"), true);
        }
    }
}
