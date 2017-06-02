using System.Collections.Generic;
using UnityEngine.FrameRecorder.Input;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace UnityEngine.FrameRecorder
{
    /// <summary>
    /// What is it: 
    /// Motivation: 
    /// Notes: 
    /// </summary>    
    public abstract class GenericRecorder<TSettings> : Recorder where TSettings : RecorderSettings
    {
        [SerializeField]
        protected TSettings m_Settings;
        public override RecorderSettings settings
        {
            get { return m_Settings; }
            set { m_Settings = (TSettings)value; }
        }
    }
}
