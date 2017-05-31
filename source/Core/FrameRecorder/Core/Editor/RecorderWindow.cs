using System;
using Assets.Unity.FrameRecorder.Scripts.Editor;
using UnityEngine.Recorder.FrameRecorder.Utilities;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;

namespace UnityEditor.Recorder.FrameRecorder
{
    public class RecorderWindow : EditorWindow
    {
        RecorderSettingsEditor m_SettingsEditor;
        bool m_PendingStartRecording;
        RecorderSelector m_recorderSelector;
        string m_StartingCategory = string.Empty;

        RecorderWindowSettings m_WindowSettingsAsset;

        public static void ShowAndPreselectCategory( string category )
        {
            var window = GetWindow(typeof(RecorderWindow), false, "Recorder") as RecorderWindow;

            if( RecordersInventory.recordersByCategory.ContainsKey(category) )
                window.m_StartingCategory = category;
        }

        public void OnEnable()
        {
            m_recorderSelector = null;
        }

        public void OnGUI()
        {
            // Bug? work arround: on Stop play, Enable is not called.
            if (m_SettingsEditor != null && m_SettingsEditor.target == null)
            {
                UnityHelpers.Destroy(m_SettingsEditor);
                m_SettingsEditor = null;
                m_recorderSelector = null;
            }

            if (m_recorderSelector == null)
            {
                if (m_WindowSettingsAsset == null)
                {
                    var candidates = AssetDatabase.FindAssets("t:RecorderWindowSettings");
                    if (candidates.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(candidates[0]);
                        m_WindowSettingsAsset = AssetDatabase.LoadAssetAtPath<RecorderWindowSettings>(path);
                    }
                    else
                    {
                        m_WindowSettingsAsset = ScriptableObject.CreateInstance<RecorderWindowSettings>();
                        AssetDatabase.CreateAsset(m_WindowSettingsAsset, "Assets/FrameRecordingSettings.asset");
                        AssetDatabase.SaveAssets();
                    }
                }

                m_recorderSelector = new RecorderSelector(OnRecorderSelected, true );
                m_recorderSelector.Init( m_WindowSettingsAsset.m_Settings, m_StartingCategory);
            }

            if (m_PendingStartRecording && EditorApplication.isPlaying)
                DelayedStartRecording();

            var size = new Vector2(300, 400);

            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                m_recorderSelector.OnGui();

            if (m_SettingsEditor != null)
            {
                m_SettingsEditor.showBounds = true;
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    EditorGUILayout.Separator();

                    var editorMinSize = m_SettingsEditor.minSize;
                    if (editorMinSize.x > minSize.x) size.x = editorMinSize.x;
                    if (editorMinSize.y > minSize.y) size.y = editorMinSize.y;

                    m_SettingsEditor.OnInspectorGUI();

                    EditorGUILayout.Separator();
                }
                RecordButton();
            }

            minSize = size;
        }

        public void OnDestroy()
        {
            StopRecording();
            UnityHelpers.Destroy(m_SettingsEditor);
            m_SettingsEditor = null;
        }

        void RecordButton()
        {
            var settings = (FrameRecorderSettings)m_SettingsEditor.target;
            var recorderGO = FrameRecorderGOControler.FindRecorder(settings);

            if (recorderGO == null)
            {
                using (new EditorGUI.DisabledScope(!m_SettingsEditor.isValid))
                {
                    if (GUILayout.Button("Start Recording"))
                        StartRecording();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Recording"))
                    StopRecording();
            }
        }

        void StartRecording()
        {
            if (!EditorApplication.isPlaying || EditorApplication.isPlaying)
            {
                m_PendingStartRecording = true;
                EditorApplication.isPlaying = true;
                return;
            }
            else
                StartRecording(false);
        }

        void DelayedStartRecording()
        {
            m_PendingStartRecording = false;
            StartRecording(true);
        }

        void StartRecording(bool autoExitPlayMode)
        {
            var settings = (FrameRecorderSettings)m_SettingsEditor.target;
            var go = FrameRecorderGOControler.HookupRecorder();
            var session = new RecordingSession()
            {
                m_Recorder = RecordersInventory.GenerateNewRecorder(m_recorderSelector.selectedRecorder, settings),
                m_RecorderGO = go,
                m_RecordingStartTS = Time.time / Time.timeScale,
                m_FrameIndex = 0
            };

            var component = go.AddComponent<RecorderComponent>();
            component.session = session;
            component.autoExitPlayMode = autoExitPlayMode;

            session.BeginRecording();
        }

        void StopRecording()
        {
            if (m_SettingsEditor != null)
            {
                var settings = (FrameRecorderSettings)m_SettingsEditor.target;
                if (settings != null)
                {
                    var recorderGO = FrameRecorderGOControler.FindRecorder(settings);
                    if (recorderGO != null)
                    {
                        UnityHelpers.Destroy(recorderGO);
                    }
                }
            }
        }

        public void OnRecorderSelected()
        {
            if (m_SettingsEditor != null)
            {
                UnityHelpers.Destroy(m_SettingsEditor);
                m_SettingsEditor = null;
            }

            if (m_recorderSelector.selectedRecorder == null)
                return;

            if (m_WindowSettingsAsset.m_Settings != null && RecordersInventory.GetRecorderInfo(m_recorderSelector.selectedRecorder).settings != m_WindowSettingsAsset.m_Settings.GetType())
            {
                UnityHelpers.Destroy(m_WindowSettingsAsset.m_Settings, true);
                m_WindowSettingsAsset.m_Settings = null;
            }

            if( m_WindowSettingsAsset.m_Settings == null )
                m_WindowSettingsAsset.m_Settings = RecordersInventory.GenerateNewSettingsAsset(m_WindowSettingsAsset, m_recorderSelector.selectedRecorder );
            m_SettingsEditor = Editor.CreateEditor( m_WindowSettingsAsset.m_Settings ) as RecorderSettingsEditor;
            AssetDatabase.SaveAssets();

        }

    }
}
