using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;
using UnityEngine.Recorder;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(GIFRecorderSettings))]
    public class GIFRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GifEncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
