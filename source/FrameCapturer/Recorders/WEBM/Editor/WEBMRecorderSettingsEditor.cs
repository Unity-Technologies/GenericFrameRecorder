using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(WEBMRecorderSettings))]
    [RecorderEditor(typeof(WEBMRecorder))]
    public class WEBMRecorderSettingsEditor : RecorderEditorBase
    {
        [MenuItem("Window/Recorder/Video...")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEncodingGroupGui()
        {
            if (EditorGUILayout.PropertyField(serializedObject.FindProperty("m_WebmEncoderSettings"), new GUIContent("Encoding"), true))
            {
                EditorGUI.indentLevel++;
                base.OnEncodingGui();
                EditorGUI.indentLevel--;
            }
        }

    }
}
