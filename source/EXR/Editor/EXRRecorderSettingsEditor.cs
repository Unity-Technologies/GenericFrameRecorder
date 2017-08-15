using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(EXRRecorderSettings))]
    public class EXRRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            m_RTInputSelector = new RTInputSelector("Pixels", false);

            var pf = new PropertyFinder<EXRRecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ExrEncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
