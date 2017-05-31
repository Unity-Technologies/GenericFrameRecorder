using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEditor.Recorder.FrameRecorder.Utilities;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(MP4RecorderSettings))]
    public class Mp4RecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField( serializedObject.FindProperty("m_MP4EncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
