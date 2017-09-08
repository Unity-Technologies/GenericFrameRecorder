using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.Recorder
{

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public class SceneHook
    {
        const string k_HostGoName = "UnityEngine-Recorder";

        static GameObject GetGameObject(bool createIfAbsent)
        {
            var go = GameObject.Find(k_HostGoName);
            if (go == null && createIfAbsent)
            {
                go = new GameObject(k_HostGoName);
                if (!RecorderSettings.m_Verbose)
                    go.hideFlags = HideFlags.HideInHierarchy;
            }

            return go;
        }

        static GameObject GetRecordingSessionsRoot(bool createIfAbsent)
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

        public static GameObject GetSettingsRoot(bool createIfAbsent)
        {
            var root = GetGameObject(createIfAbsent);
            if (root == null)
                return null;

            var settingsTr = root.transform.Find("Settings");
            GameObject settingsGO;
            if (settingsTr == null)
            {
                settingsGO = new GameObject("Settings");
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

        public static GameObject FindRecorder(RecorderSettings settings)
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

        public static void RegisterInputSettingObj(string assetId, RecorderInputSetting input)
        {
            var settingsRoot = GetInputsComponent(assetId);
            settingsRoot.m_Settings.Add(input);
        }
        
        public static void UnregisterInputSettingObj(string assetId, RecorderInputSetting input)
        {
            var settingsRoot = GetInputsComponent(assetId);
            settingsRoot.m_Settings.Remove(input);
            UnityHelpers.Destroy(input);
        }

        public static InputSettingsComponent GetInputsComponent(string assetId)
        {
            var ctrl = GetSettingsRoot(true);
            var parentRoot = ctrl.transform.Find(assetId);
            if (parentRoot == null)
            {
                parentRoot = (new GameObject()).transform;
                parentRoot.name = assetId;
                parentRoot.parent = ctrl.transform;
            }
            var settings = parentRoot.GetComponent<InputSettingsComponent>();

            if (settings == null)
            {
                settings = parentRoot.gameObject.AddComponent<InputSettingsComponent>();
                settings.m_Settings = new List<RecorderInputSetting>();
            }

            return settings;
        }
    }
}
