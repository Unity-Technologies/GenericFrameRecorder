using System;

namespace UnityEngine.Recorder.FrameRecorder
{
    public class FrameRecorderGOControler
    {
        const string k_HostGoName = "UnityEngine.Recorder.FrameRecorder";

        public static GameObject GetGameObject()
        {
            var go = GameObject.Find(k_HostGoName);
            if (go == null)
            {
                go = new GameObject(k_HostGoName);
                go.hideFlags = HideFlags.HideInHierarchy;
            }
            return go;
        }

        public static GameObject GetRecordingSessionsRoot()
        {
            var root = GetGameObject();
            var settingsTr = root.transform.Find("RecordingSessions");
            GameObject settingsGO;
            if (settingsTr == null)
            {
                settingsGO = new GameObject("RecordingSessions");
                settingsGO.transform.parent = root.transform;
            }
            else
                settingsGO = settingsTr.gameObject;

            return settingsGO;
        }

        public static GameObject HookupRecorder(FrameRecorderSettings settings)
        {
            var ctrl = GetRecordingSessionsRoot();

            var recorderGO = new GameObject();
            var settingsHost = recorderGO.AddComponent<SettingsObjHost>();
            settingsHost.m_Settings = settings;

            recorderGO.transform.parent = ctrl.transform;

            return recorderGO;
        }

        public static GameObject FindRecorder(FrameRecorderSettings settings)
        {
            var ctrl = GetRecordingSessionsRoot();

            for (int i = 0; i < ctrl.transform.childCount; i++)
            {
                var child = ctrl.transform.GetChild(i);
                var settingsHost = child.GetComponent<SettingsObjHost>();
                if (settingsHost == settings)
                    return settingsHost.gameObject;
            }

            return null;
        }
    }
}
