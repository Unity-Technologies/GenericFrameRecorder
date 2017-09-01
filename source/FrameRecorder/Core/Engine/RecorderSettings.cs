using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.FrameRecorder
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
    public abstract class RecorderSettings : ScriptableObject
    {
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
        public bool m_Verbose = false;
        public bool m_SynchFrameRate = true;
        public FileNameGenerator m_BaseFileName;
        public OutputPath m_DestinationPath;   

        public RecorderInputSetting[] m_InputsSettings = new RecorderInputSetting[0];
            
        [SerializeField]
        string m_RecorderTypeName;

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
                    var valid = m_InputsSettings.All(x => x.isValid);
                    return valid;
                }

                return true;
            }
        }

        public virtual bool isPlatformSupported {get { return true; }}

        public virtual void OnEnable()
        {
            
        }

        public virtual void OnDestroy()
        {
            if (m_InputsSettings != null)
            {
                foreach( var settings in m_InputsSettings)
                    UnityHelpers.Destroy(settings, true);
            }
        }

        public abstract List<RecorderInputSetting> GetDefaultSourcesSettings();

        public T NewInputSettingsObj<T>( string title ) where T: class
        {
            return NewInputSettingsObj(typeof(T), title) as T;
        }

        public virtual RecorderInputSetting NewInputSettingsObj(Type type,string title )
        {
            var obj = (RecorderInputSetting)ScriptableObject.CreateInstance(type) ;
            obj.m_DisplayName = title;
            obj.name = Guid.NewGuid().ToString();
            return obj;
        }
        
    }
}
