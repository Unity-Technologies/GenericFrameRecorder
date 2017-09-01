using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.FrameRecorder
{
    public class RecorderInfo
    {
        public Type recorderType;
        public Type settings;
        public Type settingsEditor;
        public string category;
        public string displayName;
    }


    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    

    // to be internal once inside unity code base
    public static class RecordersInventory
    {
        internal static SortedDictionary<string, RecorderInfo> m_Recorders { get; private set; }


        static IEnumerable<KeyValuePair<Type, object[]>> FindRecorders()
        {
            var attribType = typeof(FrameRecorderAttribute);
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetTypes())
                {
                    var attributes = t.GetCustomAttributes(attribType, false);
                    if (attributes.Length != 0)
                        yield return new KeyValuePair<Type, object[]>(t, attributes);
                }
            }
        }

        static void Init()
        {
#if UNITY_EDITOR
            if (m_Recorders != null)
                return;

            m_Recorders = new SortedDictionary<string, RecorderInfo>();
            foreach (var recorder in FindRecorders() )
                AddRecorder(recorder.Key);
#endif
        }

#if UNITY_EDITOR
        static SortedDictionary<string, List<RecorderInfo>> m_RecordersByCategory;

        public static SortedDictionary<string, List<RecorderInfo>> recordersByCategory
        {
            get
            {
                Init();
                return m_RecordersByCategory;
            }
        }

        static string[] m_AvailableCategories;
        public static string[] availableCategories
        {
            get
            {
                if (m_AvailableCategories == null)
                {
                    m_AvailableCategories = RecordersInventory.ListRecorders()
                        .GroupBy(x => x.category)
                        .Select(x => x.Key)
                        .OrderBy(x => x)
                        .ToArray();
                }
                return m_AvailableCategories;
            }
        }
#endif

        static bool AddRecorder(Type recorderType)
        {
            var recorderAttribs = recorderType.GetCustomAttributes(typeof(FrameRecorderAttribute), false);
            if (recorderAttribs.Length == 1)
            {
                var recorderAttrib = recorderAttribs[0] as FrameRecorderAttribute;
            
                if (m_Recorders == null)
                    m_Recorders = new SortedDictionary<string, RecorderInfo>();

                var info = new RecorderInfo()
                {
                    recorderType = recorderType,
                    settings = recorderAttrib.settings,
                    category = recorderAttrib.category,
                    displayName = recorderAttrib.displayName
                };

                m_Recorders.Add(info.recorderType.FullName, info);

#if UNITY_EDITOR
                if (m_RecordersByCategory == null)
                    m_RecordersByCategory = new SortedDictionary<string, List<RecorderInfo>>();

                if (!m_RecordersByCategory.ContainsKey(info.category))
                    m_RecordersByCategory.Add(info.category, new List<RecorderInfo>());

                m_RecordersByCategory[info.category].Add(info);


                // Find associated editor to recorder's settings type.

                 

#endif
                return true;
            }
            else
            {
                Debug.LogError(String.Format("The class '{0}' does not have a FrameRecorderAttribute attached to it. ", recorderType.FullName));
            }

            return false;
        }

        public static RecorderInfo GetRecorderInfo<TRecorder>() where TRecorder : class
        {
            return GetRecorderInfo(typeof(TRecorder));
        }

        public static RecorderInfo GetRecorderInfo(Type recorderType)
        {
            Init();
            if (m_Recorders.ContainsKey(recorderType.FullName))
                return m_Recorders[recorderType.FullName];

#if UNITY_EDITOR
            return null;
#else
            if (AddRecorder(recorderType))
                return m_Recorders[recorderType.FullName];
            else
                return null;
#endif
        }

        public static IEnumerable<RecorderInfo> ListRecorders()
        {
            Init();

            foreach (var recorderInfo in m_Recorders)
            {
                yield return recorderInfo.Value;
            }
        }

        public static Recorder GenerateNewRecorder(Type recorderType, RecorderSettings settings)
        {
            Init();
            var factory = GetRecorderInfo(recorderType);
            if (factory != null)
            {
                var recorder = ScriptableObject.CreateInstance(recorderType) as Recorder;
                recorder.Reset();
                recorder.settings = settings;
                return recorder;
            }
            else
                throw new ArgumentException("No factory was registered for " + recorderType.Name);
        }
#if UNITY_EDITOR
        public static RecorderSettings GenerateNewSettingsAsset(UnityEngine.Object parentAsset, Type recorderType)
        {
            Init();
            var recorderinfo = GetRecorderInfo(recorderType);
            if (recorderinfo != null)
            {
                RecorderSettings settings = null;
                settings = ScriptableObject.CreateInstance(recorderinfo.settings) as RecorderSettings;
                settings.name = "Recorder Settings";
                settings.recorderType = recorderType;
                settings.m_InputsSettings = settings.GetDefaultSourcesSettings().ToArray();
                AssetDatabase.AddObjectToAsset(settings, parentAsset);
                foreach (var obj in settings.m_InputsSettings)
                    AssetDatabase.AddObjectToAsset(obj, parentAsset);

                AssetDatabase.Refresh();

                return settings;
            }
            else
                throw new ArgumentException("No factory was registered for " + recorderType.Name);            
        }
#endif
    }
}
