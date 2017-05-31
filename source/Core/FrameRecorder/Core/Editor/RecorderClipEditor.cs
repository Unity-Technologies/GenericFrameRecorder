using Assets.Unity.FrameRecorder.Scripts.Editor;
using UnityEngine.Recorder.FrameRecorder.Utilities;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Recorder.FrameRecorder.Timeline
{
    [CustomEditor(typeof(FrameRecorderClip), true)]
    public class RecorderClipEditor : Editor
    {
        RecorderSettingsEditor m_SettingsEditor;
        TimelineAsset m_Timeline;
        RecorderSelector m_recorderSelector;

        public void OnEnable()
        {
            m_recorderSelector = null;
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            // Bug? work arround: on Stop play, Enable is not called.
            if (m_SettingsEditor != null && m_SettingsEditor.target == null)
            {
                UnityHelpers.Destroy(m_SettingsEditor);
                m_SettingsEditor = null;
                m_recorderSelector = null;
            }

            if (m_recorderSelector == null)
            {
                m_recorderSelector = new RecorderSelector( OnRecorderSelected, false );
                m_recorderSelector.Init((target as FrameRecorderClip).m_Settings);
            }

            m_recorderSelector.OnGui();

            if (m_SettingsEditor != null)
            {
                m_SettingsEditor.showBounds = false;
                m_Timeline = FindTimelineAsset();

                PushTimelineIntoRecorder();

                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    EditorGUILayout.Separator();

                    m_SettingsEditor.OnInspectorGUI();

                    EditorGUILayout.Separator();

                    PushRecorderIntoTimeline();

                    serializedObject.Update();
                }
            }
        }

        public void OnRecorderSelected()
        {
            var clip = this.target as FrameRecorderClip;

            if (m_SettingsEditor != null)
            {
                UnityHelpers.Destroy(m_SettingsEditor);
                m_SettingsEditor = null;
            }

            if (m_recorderSelector.selectedRecorder == null)
                return;

            if (clip.m_Settings != null && RecordersInventory.GetRecorderInfo(m_recorderSelector.selectedRecorder).settings != clip.m_Settings.GetType())
            {
                UnityHelpers.Destroy(clip.m_Settings, true);
                clip.m_Settings = null;
            }

            if(clip.m_Settings == null)
                clip.m_Settings = RecordersInventory.GenerateNewSettingsAsset(clip, m_recorderSelector.selectedRecorder );
            m_SettingsEditor = Editor.CreateEditor(clip.m_Settings) as RecorderSettingsEditor;
            AssetDatabase.SaveAssets();
        }

        TimelineAsset FindTimelineAsset()
        {
            if (!AssetDatabase.Contains(target))
                return null;

            var path = AssetDatabase.GetAssetPath(target);
            var objs = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var obj in objs)
            {
                if (obj != null && AssetDatabase.IsMainAsset(obj))
                    return obj as TimelineAsset;
            }
            return null;
        }

        void PushTimelineIntoRecorder()
        {
            if (m_Timeline == null)
                return;

            var settings = m_SettingsEditor.target as FrameRecorderSettings;
            settings.m_DurationMode = DurationMode.Indefinite;

            // Time
            settings.m_FrameRate = m_Timeline.editorSettings.fps;
        }

        void PushRecorderIntoTimeline()
        {
            if (m_Timeline == null)
                return;

            var settings = m_SettingsEditor.target as FrameRecorderSettings;
            settings.m_DurationMode = DurationMode.Indefinite;

            // Time
            m_Timeline.editorSettings.fps = (float)settings.m_FrameRate;
        }
    }
}
