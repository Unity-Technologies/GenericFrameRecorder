using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;
using UTJ.FrameCapturer;

namespace UTJ.FrameCapturer.Recorders
{
    [ExecuteInEditMode]
    public class EXRRecorderSettings : BaseFCRecorderSettings
    {

        public fcAPI.fcExrConfig m_ExrEncoderSettings = fcAPI.fcExrConfig.default_value;

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            var settings = ScriptableObject.CreateInstance<CBRenderTextureInputSettings>();
            return new List<RecorderInputSetting>() { ScriptableObject.CreateInstance<CBRenderTextureInputSettings>() };
        }
    }
}
