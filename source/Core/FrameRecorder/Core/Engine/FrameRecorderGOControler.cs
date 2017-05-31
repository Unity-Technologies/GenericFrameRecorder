using System;

namespace UnityEngine.Recorder.FrameRecorder
{
    public class FrameRecorderGOControler
    {
        const string k_HostGoName = "UnityEngine-Recorder-FrameRecorder";

        static GameObject GetGameObject(bool createIfAbsent)
        {
            var go = GameObject.Find(k_HostGoName);
            if (go == null && createIfAbsent)
            {
                go = new GameObject(k_HostGoName);
            }
            return go;
        }

        static GameObject GetRecordingSessionsRoot( bool createIfAbsent )
        {
            var root = GetGameObject(createIfAbsent);
            if (root == null)
                return null;
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
            var ctrl = GetRecordingSessionsRoot(true);

            var recorderGO = new GameObject();

            recorderGO.transform.parent = ctrl.transform;

            return recorderGO;
        }

        public static GameObject FindRecorder(FrameRecorderSettings settings)
        {
            var ctrl = GetRecordingSessionsRoot(false);
            if (ctrl == null)
                return null;

            for (int i = 0; i < ctrl.transform.childCount; i++)
            {
                var child = ctrl.transform.GetChild(i);
                var settingsHost = child.GetComponent<RecorderComponent>();
                if (settingsHost != null && settingsHost.session != null && settingsHost.session.settings == settings)
                    return settingsHost.gameObject;
            }

            return null;
        }
    }
}
