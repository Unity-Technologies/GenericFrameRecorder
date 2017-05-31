using System;
using Assets.Unity.FrameRecorder.Scripts.Editor;
using UnityEngine.Recorder.FrameRecorder.Utilities;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;

namespace UnityEditor.Recorder.FrameRecorder
{
    public class RecorderWindow : EditorWindow
    {
        enum EState
        {
            Idle,
            WaitingForPlayModeToStartRecording,
            Recording
        }

        RecorderSettingsEditor m_SettingsEditor;
        EState m_State = EState.Idle;

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

        DateTime m_LastRepaint = DateTime.MinValue;
        protected void Update()
        {
            if( m_State == EState.Recording  &&  (DateTime.Now - m_LastRepaint).TotalMilliseconds > 50) 
            {
                Repaint();
            }
        }

        public void OnGUI()
        {
            m_LastRepaint = DateTime.Now;

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

            if (m_State == EState.WaitingForPlayModeToStartRecording && EditorApplication.isPlaying)
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
                RecordButtonOnGui();
            }

            minSize = size;
        }

        public void OnDestroy()
        {
            StopRecording();
            UnityHelpers.Destroy(m_SettingsEditor);
            m_SettingsEditor = null;
        }

        void RecordButtonOnGui()
        {
            if (m_SettingsEditor == null || m_SettingsEditor.target == null)
                return;

            switch (m_State)
            {
                case EState.Idle:
                {
                    using (new EditorGUI.DisabledScope(!m_SettingsEditor.isValid))
                    {
                        if (GUILayout.Button("Start Recording"))
                            StartRecording();
                    }
                    break;
                }
                case EState.WaitingForPlayModeToStartRecording:
                {
                    using (new EditorGUI.DisabledScope(true))
                        GUILayout.Button("Stop Recording"); // passive
                    break;
                }

                case EState.Recording:
                {
                    var recorderGO = FrameRecorderGOControler.FindRecorder((FrameRecorderSettings)m_SettingsEditor.target);
                    if (recorderGO == null)
                    {
                        GUILayout.Button("Start Recording"); // just to keep the ui system happy.
                        m_State = EState.Idle;
                    }
                    else
                    {
                        if (GUILayout.Button("Stop Recording"))
                            StopRecording();
                        UpdateRecordingProgress(recorderGO);

                    }
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void UpdateRecordingProgress( GameObject go)
        {
            var rect = EditorGUILayout.BeginHorizontal(  );
            rect.height = 20;
            var recComp = go.GetComponent<RecorderComponent>();
            if (recComp == null || recComp.session == null)
                return;

            var session = recComp.session;
            var settings = recComp.session.m_Recorder.settings;
            switch (settings.m_DurationMode)
            {
                case DurationMode.Indefinite:
                {
                    var label = string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect, 0, label );

                    break;
                }
                case DurationMode.SingleFrame:
                    // Display nothing
                    break;
                case DurationMode.FrameInterval:
                {
                    var label = (session.m_FrameIndex < settings.m_StartFrame) ? 
                            string.Format("Skipping first {0} frames..", settings.m_StartFrame) : 
                            string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect, (session.m_FrameIndex +1) / (float)(settings.m_EndFrame +1), label );
                    break;
                }
                case DurationMode.TimeInterval:
                {
                    var label = (session.m_CurrentFrameStartTS < settings.m_StartTime) ?
                        string.Format("Skipping first {0} seconds...", settings.m_StartTime) :
                        string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect,(float)session.m_CurrentFrameStartTS / (settings.m_EndTime == 0f ? 0.0001f : settings.m_EndTime), label );
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        void StartRecording()
        {
            m_State = EState.WaitingForPlayModeToStartRecording;
            EditorApplication.isPlaying = true;
            return;
        }

        void DelayedStartRecording()
        {
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
            m_State = EState.Recording;
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
