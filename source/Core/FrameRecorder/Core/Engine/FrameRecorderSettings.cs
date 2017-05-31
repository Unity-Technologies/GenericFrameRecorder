using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Recorder.FrameRecorder.DataSource;
using UnityEngine.Recorder.FrameRecorder.Utilities;

namespace UnityEngine.Recorder.FrameRecorder
{
    [Flags]
    public enum EImageSource
    {       
        GameDisplay = 1,
        SceneView = 2,
        MainCamera  = 4,
        TaggedCamera = 8,
        RenderTexture = 16
    }

    public enum FrameRateMode
    {
        Variable,
        Fixed,
    }

    public enum DurationMode
    {
        Indefinite,
        SingleFrame,
        FrameInterval,
        TimeInterval
    }

    public abstract class FrameRecorderSettings : ScriptableObject
    {
        public int m_CaptureEveryNthFrame = 1;
        public FrameRateMode m_FrameRateMode = FrameRateMode.Fixed;
        public double m_FrameRate = 24.0;
        public int m_StartFrame;
        public int m_EndFrame = 10;
        public float m_StartTime = 0.0f;
        public float m_EndTime = 1.0f;
        public DurationMode m_DurationMode;
        public bool m_Verbose = false;

        public RecorderInputSetting[] m_SourceSettings = new RecorderInputSetting[0];
            
        [SerializeField]
        string m_RecorderTypeName;

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
            get { return m_DurationMode != DurationMode.Indefinite; }
        }

        public virtual bool isValid
        {
            get { return m_FrameRate > 0; }
        }

        public virtual void OnEnable()
        {
            
        }

        public virtual void OnDestroy()
        {
            if (m_SourceSettings != null)
            {
                foreach( var settings in m_SourceSettings)
                    UnityHelpers.Destroy(settings, true);
            }
        }

        public abstract List<RecorderInputSetting> GetDefaultSourcesSettings();
        
    }
}
