using System.Collections.Generic;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UTJ.FrameCapturer.Recorders
{
    [ExecuteInEditMode]
    public class WEBMRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcWebMConfig m_WebmEncoderSettings = fcAPI.fcWebMConfig.default_value;

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            var settings = ScriptableObject.CreateInstance<CBRenderTextureInputSettings>();
            settings.m_PadSize = true;
            return new List<RecorderInputSetting>() { settings };
        }
    }
}
