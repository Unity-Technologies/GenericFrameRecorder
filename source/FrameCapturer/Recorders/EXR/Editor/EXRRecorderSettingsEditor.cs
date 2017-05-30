using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(EXRRecorderSettings))]
    [RecorderEditor(typeof(EXRRecorder))]
    public class EXRRecorderSettingsEditor : RecorderEditorBase
    {
        [MenuItem("Window/Recorder/Video...")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ExrEncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
