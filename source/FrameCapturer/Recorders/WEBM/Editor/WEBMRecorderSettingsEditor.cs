using System;
using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(WEBMRecorderSettings))]
    [RecorderEditor(typeof(WEBMRecorder))]
    public class WEBMRecorderSettingsEditor : DefaultImageRecorderSettingsEditor
    {
        public override Vector2 minSize
        {
            get { return new Vector2(400, 370); }
        }

        [MenuItem("Window/Recorder/Video...")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEncodingGroupGui()
        {
            if (EditorGUILayout.PropertyField(serializedObject.FindProperty("m_WebmEncoderSettings"), new GUIContent("Encoding"), true))
            {
                m_LayoutHelper.indentLevel++;
                base.OnEncodingGui();
                m_LayoutHelper.indentLevel--;
            }
        }

        protected override void OnOutputGui()
        {
            var settingsObj = serializedObject.targetObject as WEBMRecorderSettings;

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
