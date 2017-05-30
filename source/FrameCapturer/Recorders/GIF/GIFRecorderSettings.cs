using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UTJ.FrameCapturer.Recorders
{
    [ExecuteInEditMode]
    public class GIFRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcGifConfig m_GifEncoderSettings = fcAPI.fcGifConfig.default_value;

        public override bool isValid
        {
            get
            {
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath) && !string.IsNullOrEmpty(m_BaseFileName);
            }
        }

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            return new List<RecorderInputSetting>() { ScriptableObject.CreateInstance<CBRenderTextureInputSettings>() };
        }
    }
}
