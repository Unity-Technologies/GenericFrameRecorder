using System;
using UnityEditor.Recorder.FrameRecorder.Utilities;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UnityEditor.Recorder.FrameRecorder.Example
{
    [CustomEditor(typeof(PNGRecorderSettings))]
    public class PNGRecorderEditor : RecorderSettingsEditor
    {
        SerializedProperty m_DestinationPath;
        SerializedProperty m_BaseFileName;

        string[] m_Candidates;
        public override Vector2 minSize
        {
            get { return new Vector2(300, 340); }
        }
        
        [MenuItem("Window/Recorder/Video...")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            m_Candidates = new [] { "Command Buffered Camera", "Camera as RenderTexture" };
            var pf = new PropertyFinder<PNGRecorderSettings>(serializedObject);
            m_Inputs = pf.Find(w => w.m_SourceSettings);
            m_DestinationPath = pf.Find(w => w.m_DestinationPath);
            m_BaseFileName = pf.Find(w => w.m_BaseFileName);
        }

        protected override void OnEncodingGroupGui()
        {
            // hiding this group by not calling parent class's implementation.  
        }

        protected override void OnInputGui()
        {
            var input = m_Inputs.GetArrayElementAtIndex(0).objectReferenceValue;

            EditorGUI.indentLevel++;
            var index = input.GetType() == typeof(CBRenderTextureInputSettings) ? 0 : 1;
            var newIndex = EditorGUILayout.Popup("Image Generator", index, m_Candidates);
            EditorGUI.indentLevel--;

            if (index != newIndex)
            {
                var newType = newIndex == 0 ? typeof(CBRenderTextureInputSettings) : typeof(AdamBeautySourceSettings);
                var newSettings = (RecorderInputSetting)Activator.CreateInstance(newType);
                ChangeInputSettings(0, newSettings);
            }

            base.OnInputGui();
        }

        protected override void OnOutputGui()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Directory");
            m_DestinationPath.stringValue = EditorGUILayout.TextField(m_DestinationPath.stringValue);
            if (GUILayout.Button("...", GUILayout.Width(30)))
                m_DestinationPath.stringValue = EditorUtility.OpenFolderPanel( "Select output location", m_DestinationPath.stringValue, "");
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(m_BaseFileName, new GUIContent("File name"));
        }
    }
}
