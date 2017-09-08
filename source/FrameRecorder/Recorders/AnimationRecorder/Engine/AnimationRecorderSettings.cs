using System;
using System.Collections.Generic;

namespace UnityEngine.FrameRecorder
{
    [ExecuteInEditMode]
    [Serializable]
    public class AnimationRecorderSettings : RecorderSettings
    {
        public string outputPath = "AnimRecorder/"+goToken+"_"+bindingToken+"_"+takeToken;
        public int take = 1;
        public static string goToken = "<goName>";
        public static string takeToken = "<take>";
        public static string bindingToken = "<binding>";
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