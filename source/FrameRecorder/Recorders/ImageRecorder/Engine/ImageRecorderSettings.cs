using System;
using System.Collections.Generic;
using UnityEngine.Recorder.Input;

namespace UnityEngine.Recorder
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
            m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>()
            {
                NewInputSettingsObj<CBRenderTextureInputSettings>("Pixels") 
            };
        }

        public override bool isValid
        {
            get
            {
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath.GetFullPath()) && !string.IsNullOrEmpty(m_BaseFileName.pattern);
            }
        }

        public override RecorderInputSetting NewInputSettingsObj(Type type, string title )
        {
            var obj = base.NewInputSettingsObj(type, title);
            return obj ;
        }

        public override bool SelfAdjustSettings()
        {
            if (inputsSettings.Count == 0 || !(inputsSettings[0] is RenderTextureSamplerSettings))
                return false;

            var input = (RenderTextureSamplerSettings)inputsSettings[0];

            var colorSpace = m_OutputFormat == PNGRecordeOutputFormat.EXR ? ColorSpace.Linear : ColorSpace.Gamma;
            if (input.m_ColorSpace != colorSpace)
            {
                input.m_ColorSpace = colorSpace;
                return true;
            }

            return false;
        }
    }
}
