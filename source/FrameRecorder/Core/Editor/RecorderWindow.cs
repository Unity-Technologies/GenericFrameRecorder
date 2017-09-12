using System;
using UnityEngine.FrameRecorder;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{
    public class RecorderWindow : EditorWindow
    {
        enum EState
        {
            Idle,
            WaitingForPlayModeToStartRecording,
            Recording
        }

        RecorderEditor m_Editor;
        EState m_State = EState.Idle;

        RecorderSelector m_recorderSelector;
        string m_StartingCategory = string.Empty;

        RecorderWindowSettings m_WindowSettingsAsset;

        public static void ShowAndPreselectCategory(string category)
        {
            var window = GetWindow(typeof(RecorderWindow), false, "Recorder") as RecorderWindow;

            if (RecordersInventory.recordersByCategory.ContainsKey(category))
            {
                window.m_StartingCategory = category;
                window.m_recorderSelector = null;
            }
        }

        public void OnEnable()
        {
            m_recorderSelector = null;
        }

        DateTime m_LastRepaint = DateTime.MinValue;

        protected void Update()
        {
            if (m_State == EState.Recording && (DateTime.Now - m_LastRepaint).TotalMilliseconds > 50)
            {
                Repaint();
            }
        }

        Vector2 m_ScrollPos;

        public void OnGUI()
        {
            try
            {
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                try
                {
                    m_LastRepaint = DateTime.Now;

                    // Bug? work arround: on Stop play, Enable is not called.
                    if (m_Editor != null && m_Editor.target == null)
                    {
                        UnityHelpers.Destroy(m_Editor);
                        m_Editor = null;
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
                                if (m_WindowSettingsAsset == null)
                                {
                                    AssetDatabase.DeleteAsset(path);
                                }
                            }
                            if(m_WindowSettingsAsset == null)
                            {
                                m_WindowSettingsAsset = ScriptableObject.CreateInstance<RecorderWindowSettings>();
                                AssetDatabase.CreateAsset(m_WindowSettingsAsset, "Assets/FrameRecordingSettings.asset");
                                AssetDatabase.Refresh();
                            }
                        }

                        m_recorderSelector = new RecorderSelector(OnRecorderSelected, true);
                        m_recorderSelector.Init(m_WindowSettingsAsset.m_Settings, m_StartingCategory);
                    }

                    if (m_State == EState.WaitingForPlayModeToStartRecording && EditorApplication.isPlaying)
                        DelayedStartRecording();

                    using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                        m_recorderSelector.OnGui();

                    if (m_Editor != null)
                    {
                        m_Editor.showBounds = true;
                        using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                        {
                            EditorGUILayout.Separator();
                            m_Editor.OnInspectorGUI();
                            EditorGUILayout.Separator();
                        }
                        RecordButtonOnGui();
                        GUILayout.Space(50);
                    }
                }
                finally
                {
                    EditorGUILayout.EndScrollView();
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                if (m_State == EState.Recording)
                {
                    try
                    {
                        Debug.LogError("Aborting recording due to an exception!\n" + ex.ToString());
                        StopRecording();
                    }
                    catch (Exception) {}
                }
                else
                {
                    EditorGUILayout.HelpBox("An exception was raised while editing the settings. This can be indicative of corrupted settings.", MessageType.Warning);

                    if (GUILayout.Button("Reset settings to default"))
                    {
                        ResetSettings();
                    }                    
                }
                Debug.LogException(ex);
            }
        }

        void ResetSettings()
        {
            UnityHelpers.Destroy(m_Editor);
            m_Editor = null;
            m_recorderSelector = null;
            var path = AssetDatabase.GetAssetPath(m_WindowSettingsAsset);
            UnityHelpers.Destroy(m_WindowSettingsAsset, true);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            m_WindowSettingsAsset = null;
        }

        public void OnDestroy()
        {
            StopRecording();
            UnityHelpers.Destroy(m_Editor);
            m_Editor = null;
        }

        void RecordButtonOnGui()
        {
            if (m_Editor == null || m_Editor.target == null)
                return;

            switch (m_State)
            {
                case EState.Idle:
                {
                    using (new EditorGUI.DisabledScope(!m_Editor.isValid ))
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
                    var recorderGO = FrameRecorderGOControler.FindRecorder((RecorderSettings)m_Editor.target);
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
                case DurationMode.Manual:
                {
                    var label = string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect, 0, label );

                    break;
                }
                case DurationMode.SingleFrame:
                case DurationMode.FrameInterval:
                {
                    var label = (session.frameIndex < settings.m_StartFrame) ? 
                            string.Format("Skipping first {0} frames...", settings.m_StartFrame-1) : 
                            string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect, (session.frameIndex +1) / (float)(settings.m_EndFrame +1), label );
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
            var settings = (RecorderSettings)m_Editor.target;
            var go = FrameRecorderGOControler.HookupRecorder(!settings.m_Verbose);
            var session = new RecordingSession()
            {
                m_Recorder = RecordersInventory.GenerateNewRecorder(m_recorderSelector.selectedRecorder, settings),
                m_RecorderGO = go,
            };

            var component = go.AddComponent<RecorderComponent>();
            component.session = session;
            component.autoExitPlayMode = autoExitPlayMode;

            if (session.SessionCreated() && session.BeginRecording())
                m_State = EState.Recording;
            else
            {
                m_State = EState.Idle;
                StopRecording();
            }
        }

        void StopRecording()
        {
            if (m_Editor != null)
            {
                var settings = (RecorderSettings)m_Editor.target;
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
            if (m_Editor != null)
            {
                UnityHelpers.Destroy(m_Editor);
                m_Editor = null;
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
            m_Editor = Editor.CreateEditor( m_WindowSettingsAsset.m_Settings ) as RecorderEditor;
            AssetDatabase.Refresh();

        }

    }
}
