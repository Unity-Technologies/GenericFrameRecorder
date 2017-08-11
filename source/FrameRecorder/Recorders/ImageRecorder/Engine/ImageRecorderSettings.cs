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
        public PNGRecordeOutputFormat m_OutputFormat = PNGRecordeOutputFormat.JPEG;

        ImageRecorderSettings()
        {
            m_BaseFileName = "image_<0000>.<ext>";
        }

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
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath.GetFullPath()) && !string.IsNullOrEmpty(m_BaseFileName);
            }
        }
    }
}
