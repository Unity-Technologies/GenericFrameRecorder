using System.Collections.Generic;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UnityEngine.Recorder.FrameRecorder
{
    [ExecuteInEditMode]
    public class PNGRecorderSettings : FrameRecorderSettings
    {
        public string m_BaseFileName  = "pngFile";
        public string m_DestinationPath = "Recorder";

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            return new List<RecorderInputSetting>() { ScriptableObject.CreateInstance<AdamBeautySourceSettings>() };
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
