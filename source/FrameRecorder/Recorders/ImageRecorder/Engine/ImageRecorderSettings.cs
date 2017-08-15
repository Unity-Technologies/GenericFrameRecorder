using System;
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
            m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
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
            if (type == typeof(CBRenderTextureInputSettings))
                (obj as CBRenderTextureInputSettings).m_FlipVertical = true;

            return obj ;
        }
    }
}
