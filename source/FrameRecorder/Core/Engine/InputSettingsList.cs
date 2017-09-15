using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.Recorder
{
    [Serializable]
    public class InputSettingsList : IEnumerable<RecorderInputSetting>
    {
        [SerializeField]
        List<RecorderInputSetting> m_InputsSettingsAssets;
        List<RecorderInputSetting> m_InputsSettings;
        public string m_ParentAssetId;

        public void OnEnable( string parentAssetId )
        {
            m_ParentAssetId = parentAssetId;
            Reset();
        }

        public void Reset()
        {
            if(m_InputsSettingsAssets == null)
                m_InputsSettingsAssets = new List<RecorderInputSetting>();
            m_InputsSettings = new List<RecorderInputSetting>();

            foreach (var input in m_InputsSettingsAssets)
                m_InputsSettings.Add(input);  
        }

        public bool isValid
        {
            get
            {
                foreach( var x in m_InputsSettings )
                    if (!x.isValid)
                        return false;
                return true;
            }
        }

        public bool hasBrokenBindings
        {
            get
            {
                foreach( var x in m_InputsSettings )
                    if (x == null || x is InputBinder)
                        return true;
                return false;
            }
        }

        public RecorderInputSetting this [int index]
        {
            get
            {
                return m_InputsSettings[index]; 
            }

            set
            {
                ReplaceAt(index, value);
            }
        }

        public IEnumerator<RecorderInputSetting> GetEnumerator()
        {
            return ((IEnumerable<RecorderInputSetting>)m_InputsSettings).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(List<RecorderInputSetting> list)
        {
            foreach (var value in list)
                Add(value);
        }

        public void Add(RecorderInputSetting input)
        {
            m_InputsSettings.Add(null);
            m_InputsSettingsAssets.Add(null);
            ReplaceAt(m_InputsSettings.Count - 1, input);
        }

        public int Count
        {
            get
            {
                return m_InputsSettings.Count; 
            }
        }

        public void Rebind(RecorderInputSetting input)
        {
            if (input is InputBinder)
            {
                Debug.LogError("Cannot bind a InputBinder object!");
                return;
            }

            for (int i = 0; i < m_InputsSettings.Count; i++)
            {
                var ib = m_InputsSettings[i] as InputBinder;
                if ( ib == null || ib.inputType == input.GetType() )
                {
                    m_InputsSettings[i] = input;
                    return;
                }
            }
        }

        public void Remove(RecorderInputSetting input)
        {
            for (int i = 0; i < m_InputsSettings.Count; i++)
            {
                if (m_InputsSettings[i] == input)
                {
                    ReleaseAt(i);
                    m_InputsSettings.RemoveAt(i);
                    m_InputsSettingsAssets.RemoveAt(i);
                }
            }
        }

        public void ReplaceAt(int index, RecorderInputSetting input)
        {
            if (m_InputsSettingsAssets == null || m_InputsSettings.Count <= index)
                throw new ArgumentException("Index out of range");

            // Release input
            ReleaseAt(index);

            m_InputsSettings[index] = input;
            if (input.storeInScene)
            {
                var binder = ScriptableObject.CreateInstance<InputBinder>();
                binder.name = "Scene-Stored";
                binder.m_DisplayName = input.m_DisplayName;
                binder.m_TypeName = input.GetType().FullName;
                m_InputsSettingsAssets[index] = binder;
                SceneHook.RegisterInputSettingObj(m_ParentAssetId, input);

#if UNITY_EDITOR
                var assetPath = AssetDatabase.GUIDToAssetPath(m_ParentAssetId);
                AssetDatabase.AddObjectToAsset(binder, assetPath);
                AssetDatabase.SaveAssets();
#endif

            }
            else
            {
                m_InputsSettingsAssets[index] = input;
#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(input, AssetDatabase.GUIDToAssetPath(m_ParentAssetId));
                AssetDatabase.SaveAssets();
#endif
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        void ReleaseAt(int index)
        {
            if (m_InputsSettingsAssets[index] is InputBinder ) 
                SceneHook.UnregisterInputSettingObj(m_ParentAssetId, m_InputsSettings[index]);

            UnityHelpers.Destroy(m_InputsSettingsAssets[index],true);

            m_InputsSettings[index] = null;
            m_InputsSettingsAssets[index] = null;
        }

#if UNITY_EDITOR
        public void RepareMissingBindings()
        {
            for (int i = 0; i < m_InputsSettingsAssets.Count; i++)
            {
                var ib = m_InputsSettingsAssets[i] as InputBinder;
                if (ib != null && m_InputsSettings[i] == null)
                {
                    var newInput = ScriptableObject.CreateInstance(ib.inputType) as RecorderInputSetting;
                    newInput.m_DisplayName = ib.m_DisplayName;
                    m_InputsSettings[i] = newInput;
                    SceneHook.RegisterInputSettingObj(m_ParentAssetId, newInput);
                }
            }            
        }
#endif

        public void OnDestroy()
        {
            for (int i = 0; i < m_InputsSettingsAssets.Count; i++)
            {
                if (m_InputsSettingsAssets[i] is InputBinder)
                    SceneHook.UnregisterInputSettingObj(m_ParentAssetId, m_InputsSettings[i]);

                UnityHelpers.Destroy(m_InputsSettingsAssets[i], true);
            }
        }

    }
}