using System;

namespace UnityEngine.FrameRecorder
{

    /// <summary>
    /// What is this: Class attribute that decorates classes that are Recorders and provides the information needed to register it with the RecorderInventory class.
    /// Motivation  : Provide a way to dynamically discover Recorder classes and provide a classification system and link between the recorder classes and their Settings classes.
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

    /// <summary>
    /// What is this: Indicate that a Input settings instance is scene specific and should not be shared accross scenes (not in a project wide asset)
    /// Motivation  : Some input settings target specific scenes, for example target a game object in the scene. Having the settings be stored in the 
    ///                 scene simplifies referencing.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StoreInSceneAttribute : Attribute
    {
        public StoreInSceneAttribute()
        {
        }
    }

}
