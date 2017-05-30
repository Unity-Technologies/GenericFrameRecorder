using UnityEditor;
using UnityEditor.Recorder.FrameRecorder;
using UnityEngine;
using UnityEditor.Recorder.FrameRecorder.Utilities;

namespace UTJ.FrameCapturer.Recorders
{
    public class RecorderEditorBase : RecorderSettingsEditor
    {
        public string m_BaseFileName;
        public string m_DestinationPath;

        SerializedProperty m_DestPathProp;
        SerializedProperty m_BaseFileNameProp;


        protected void OnEnable()
        {
            base.OnEnable();
            if (target != null)
            {
                m_DestPathProp = serializedObject.FindProperty<BaseFCRecorderSettings>(x => x.m_DestinationPath);
                m_BaseFileNameProp = serializedObject.FindProperty<BaseFCRecorderSettings>(x => x.m_BaseFileName);
            }
        }

        public override Vector2 minSize
        {
            get { return new Vector2(400, 370); }
        }

        protected override void OnOutputGui()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Directory");
            m_DestPathProp.stringValue = EditorGUILayout.TextField(m_DestPathProp.stringValue);
            if (GUILayout.Button("...", GUILayout.Width(30)))
                m_DestPathProp.stringValue = EditorUtility.OpenFolderPanel( "Select output location", m_DestPathProp.stringValue, "");
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(m_BaseFileNameProp, new GUIContent("File name"));

            base.OnOutputGui();
        }
    }
}
