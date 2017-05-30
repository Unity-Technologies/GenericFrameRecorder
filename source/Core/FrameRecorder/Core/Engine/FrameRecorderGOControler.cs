using System;

namespace UnityEngine.Recorder.FrameRecorder
{
    public class FrameRecorderGOControler
    {
        const string k_HostGoName = "UnityEngine-Recorder-FrameRecorder2";

        public static GameObject GetGameObject()
        {
            var go = GameObject.Find(k_HostGoName);
            if (go == null)
            {
                go = new GameObject(k_HostGoName);
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

        public static GameObject HookupRecorder()
        {
            var ctrl = GetRecordingSessionsRoot();

            var recorderGO = new GameObject();

            recorderGO.transform.parent = ctrl.transform;

            return recorderGO;
        }

        public static GameObject FindRecorder(FrameRecorderSettings settings)
        {
            var ctrl = GetRecordingSessionsRoot();

            for (int i = 0; i < ctrl.transform.childCount; i++)
            {
                var child = ctrl.transform.GetChild(i);
                var settingsHost = child.GetComponent<RecorderComponent>();
                if (settingsHost != null && settingsHost.session.settings == settings)
                    return settingsHost.gameObject;
            }

            return null;
        }
    }
}
