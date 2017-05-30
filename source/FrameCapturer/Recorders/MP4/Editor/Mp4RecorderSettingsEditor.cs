using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEditor.Recorder.FrameRecorder.Utilities;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(MP4RecorderSettings))]
    [RecorderEditor(typeof(MP4Recorder))]
    public class Mp4RecorderSettingsEditor : RecorderEditorBase
    {
        [MenuItem("Window/Recorder/Video...")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField( serializedObject.FindProperty("m_MP4EncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
