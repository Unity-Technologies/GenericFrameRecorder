using System;
using System.Collections.Generic;
using UnityEditor.Experimental.Recorder.Input;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;

namespace UnityEditor.Experimental.Recorder
{
    [ExecuteInEditMode]
    [Serializable]
    public class AnimationRecorderSettings : RecorderSettings
    {
        public string outputPath = "AnimRecorder"+takeToken+"/"+goToken+"_"+inputToken;
        public int take = 1;
        public static string goToken = "<goName>";
        public static string takeToken = "<take>";
        public static string inputToken = "<input>";
        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>()
            {
                NewInputSettingsObj<AnimationInputSettings>("Animation") 
            };
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