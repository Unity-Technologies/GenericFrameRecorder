using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UTJ.FrameCapturer.Recorders
{
    [ExecuteInEditMode]
    public class PNGRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcPngConfig m_PngEncoderSettings = fcAPI.fcPngConfig.default_value;

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            return new List<RecorderInputSetting>() { ScriptableObject.CreateInstance<CBRenderTextureInputSettings>() };
        }
    }
}
