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
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath.GetFullPath()) && !string.IsNullOrEmpty(m_BaseFileName.pattern);
            }
        }

        public override bool isPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsEditor || 
                        Application.platform == RuntimePlatform.WindowsPlayer ||
                        Application.platform == RuntimePlatform.OSXEditor ||
                        Application.platform == RuntimePlatform.OSXPlayer ||
                        Application.platform == RuntimePlatform.LinuxEditor ||
                        Application.platform == RuntimePlatform.LinuxPlayer;
            }
        }
    }
}
