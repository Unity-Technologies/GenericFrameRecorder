using UnityEngine;
using UnityEngine.FrameRecorder;

namespace UTJ.FrameCapturer.Recorders
{
    public abstract class BaseFCRecorderSettings : RecorderSettings
    {
        public override bool isValid
        {
            get
            {
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath.GetFullPath()) && !string.IsNullOrEmpty(m_BaseFileName);
            }
        }
    }
}
