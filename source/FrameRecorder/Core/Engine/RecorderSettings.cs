using System;
using System.Collections.Generic;

namespace UnityEngine.Recorder
{
    [Flags]
    public enum EImageSource
    {       
        GameDisplay = 1,
        SceneView = 2,
        MainCamera  = 4,
        TaggedCamera = 8,
        RenderTexture = 16,
    }

    public enum FrameRateMode
    {
        Variable,
        Constant,
    }

    public enum DurationMode
    {
        Manual,
        SingleFrame,
        FrameInterval,
        TimeInterval
    }


  
    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    [ExecuteInEditMode]
    public abstract class RecorderSettings : ScriptableObject
    {
        [SerializeField]
        string m_AssetID;
        public int m_CaptureEveryNthFrame = 1;
        public FrameRateMode m_FrameRateMode = FrameRateMode.Constant;
        [Range(1,120)]
        public double m_FrameRate = 30.0;
        public EFrameRate m_FrameRateExact = EFrameRate.FR_CUSTOM;
        public int m_StartFrame;
        public int m_EndFrame = 10;
        public float m_StartTime = 0.0f;
        public float m_EndTime = 1.0f;
        public DurationMode m_DurationMode;
        public bool m_SynchFrameRate = true;
        public FileNameGenerator m_BaseFileName;
        public OutputPath m_DestinationPath;

        [SerializeField]
        private InputSettingsList m_InputsSettings = new InputSettingsList();

        public InputSettingsList inputsSettings
        {
            get
            {
                return m_InputsSettings;
            }
        }

        [SerializeField]
        string m_RecorderTypeName;

        public string assetID
        {
            get { return m_AssetID; }
            set
            {
                m_AssetID = value;
                m_InputsSettings.m_ParentAssetId = value;
            }
        }

        public RecorderSettings()
        {
            m_DestinationPath.root = OutputPath.ERoot.Current;
            m_DestinationPath.leaf = "Recordings";
        }

        public Type recorderType
        {
            get
            {
                if (string.IsNullOrEmpty(m_RecorderTypeName))
                    return null;
                return Type.GetType(m_RecorderTypeName); 
            }
            set { m_RecorderTypeName = value == null ? string.Empty : value.AssemblyQualifiedName; }
        }

        public bool fixedDuration
        {
            get { return m_DurationMode != DurationMode.Manual; }
        }

        public virtual bool isValid
        {
            get
            {
                if (m_FrameRate == 0 || m_CaptureEveryNthFrame <= 0)
                    return false;

                if (m_InputsSettings != null)
                {
                    return m_InputsSettings.isValid;
                }

                return true;
            }
        }

        public virtual bool isPlatformSupported {get { return true; }}

        public virtual void OnEnable()
        {
            m_InputsSettings.OnEnable(m_AssetID);
            BindSceneInputSettings();
        }

        public void BindSceneInputSettings()
        {
            if (!m_InputsSettings.hasBrokenBindings)
                return;

            var sceneInputs = SceneHook.GetInputsComponent(m_AssetID);
            foreach (var input in sceneInputs.m_Settings)
            {
                m_InputsSettings.Rebind(input);
            }
            
#if UNITY_EDITOR
            if (m_InputsSettings.hasBrokenBindings)
            {
                // only supported case is scene stored input settings are missing (for example: new scene loaded that does not contain the scene stored inputs.)
                m_InputsSettings.RepareMissingBindings();
            }
#endif

            if (m_InputsSettings.hasBrokenBindings)
                Debug.LogError("Recorder: missing input settings");

        }

        public virtual void OnDestroy()
        {
            if (m_InputsSettings != null)
                m_InputsSettings.OnDestroy();
        }

        public abstract List<RecorderInputSetting> GetDefaultInputSettings();

        public T NewInputSettingsObj<T>( string title ) where T: class
        {
            return NewInputSettingsObj(typeof(T), title) as T;
        }

        public virtual RecorderInputSetting NewInputSettingsObj(Type type, string title )
        {
            var obj = (RecorderInputSetting)ScriptableObject.CreateInstance(type) ;
            obj.m_DisplayName = title;
            obj.name = Guid.NewGuid().ToString();
            return obj;
        }

        /// <summary>
        /// Allows for recorder specific settings logic to correct/adjust settings that might be missed by it's editor.
        /// </summary>
        /// <returns>true if setting where changed</returns>
        public virtual bool SelfAdjustSettings()
        {
            return false; 
        }
        
    }
}
