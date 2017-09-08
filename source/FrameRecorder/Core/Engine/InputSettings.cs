using System;

namespace UnityEngine.Recorder
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

        public bool storeInScene
        {
            get { return Attribute.GetCustomAttribute(GetType(), typeof(StoreInSceneAttribute)) != null; }
        }
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
