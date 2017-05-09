using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEditor.Recorder.FrameRecorder.Utilities;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(MP4RecorderSettings))]
    [RecorderEditor(typeof(MP4Recorder))]
    public class Mp4RecorderSettingsEditor : DefaultImageRecorderSettingsEditor
    {
        public override Vector2 minSize
        {
            get { return new Vector2(400, 370); }
        }

        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField( serializedObject.FindProperty("m_MP4EncoderSettings"), new GUIContent("Encoding"), true);
        }

        protected override void OnOutputGui()
        {
            var settingsObj = serializedObject.targetObject as MP4RecorderSettings;

            GUILayout.BeginHorizontal();
            m_LayoutHelper.AddPropertyLabel("Directory");
            settingsObj.m_DestinationPath = EditorGUILayout.TextField(settingsObj.m_DestinationPath);
            if (GUILayout.Button("...", GUILayout.Width(30)))
                settingsObj.m_DestinationPath = EditorUtility.OpenFolderPanel(m_LayoutHelper + "Select output location", settingsObj.m_DestinationPath, "");
            GUILayout.EndHorizontal();
            //settingsObj.m_DestinationPath = DestinationDirectoryGui(settingsObj.m_DestinationPath);
            m_LayoutHelper.AddStringProperty("File name", serializedObject, () => settingsObj.m_BaseFileName);
        }
    }
}
