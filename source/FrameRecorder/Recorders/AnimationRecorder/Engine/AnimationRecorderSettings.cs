using System;
using System.Collections.Generic;

namespace UnityEngine.FrameRecorder
{
    [ExecuteInEditMode]
    [Serializable]
    public class AnimationRecorderSettings : RecorderSettings
    { 
        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return  new List<RecorderInputSetting>();
        }
        
        public override bool isPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.LinuxEditor ||
                       Application.platform == RuntimePlatform.OSXEditor ||
                       Application.platform == RuntimePlatform.WindowsEditor;
            }
        }
    }
}