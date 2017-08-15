using System;

namespace UnityEngine.FrameRecorder
{
    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public abstract class  RecorderInputSetting : ScriptableObject
    {
        public abstract Type inputType { get; }
        public abstract bool isValid { get; }
        public string m_DisplayName;
    }

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public abstract class InputSettings<TInput> : RecorderInputSetting
    {
        public override Type inputType
        {
            get { return typeof(TInput); }
        }
    }
}
