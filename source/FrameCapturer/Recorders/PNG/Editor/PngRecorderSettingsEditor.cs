using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(PNGRecorderSettings))]
    [RecorderEditor(typeof(PNGRecorder))]
    public class PngRecorderSettingsEditor : RecorderEditorBase
    {
        [MenuItem("Window/Recorder/Video...")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PngEncoderSettings"), new GUIContent("Encoding"), true);
        }
    }
}
