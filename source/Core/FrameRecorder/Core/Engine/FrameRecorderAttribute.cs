using System;

namespace UnityEngine.FrameRecorder
{

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FrameRecorderAttribute : Attribute
    {
        public Type settings;
        public string category;
        public string displayName;

        public FrameRecorderAttribute(Type settingsType, string category, string displayName)
        {
            this.settings = settingsType;
            this.category = category;
            this.displayName = displayName;
        }
    }

}
