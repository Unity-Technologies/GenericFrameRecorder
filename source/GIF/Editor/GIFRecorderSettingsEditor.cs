using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(GIFRecorderSettings))]
    public class GIFRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;
            m_RTInputSelector = new RTInputSelector("Pixels", false);
            var pf = new PropertyFinder<GIFRecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GifEncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
