using System.Collections.Generic;
using UnityEngine.FrameRecorder.Input;

namespace UnityEngine.FrameRecorder
{

    public enum PNGRecordeOutputFormat
    {
        PNG,
        JPEG,
        EXR
    }

    [ExecuteInEditMode]
    public class ImageRecorderSettings : RecorderSettings
    {
        public string m_BaseFileName  = "image";
        public string m_DestinationPath = "Recorder";
        public PNGRecordeOutputFormat m_OutputFormat = PNGRecordeOutputFormat.JPEG;

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            var defaultSettings = ScriptableObject.CreateInstance<CBRenderTextureInputSettings>();
            defaultSettings.m_FlipVertical = true;
            return new List<RecorderInputSetting>() { defaultSettings};
        }

        public override bool isValid
        {
            get
            {
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath) && !string.IsNullOrEmpty(m_BaseFileName);
            }
        }
    }
}
