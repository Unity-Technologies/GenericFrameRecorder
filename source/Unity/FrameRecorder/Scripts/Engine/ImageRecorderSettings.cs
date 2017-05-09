using System;

namespace UnityEngine.Recorder.FrameRecorder
{

    public enum EImageSizeMode
    {
        Dynamic,
        FullScreen,
        Width,
        Custom
    }

    public class ImageRecorderSettings : FrameRecorderSettings
    {
        public EImageSizeMode m_SizeMode = EImageSizeMode.Dynamic;
        public int m_Width = 1024;
        public int m_Height = 768;

        public EImageSourceType m_InputType = EImageSourceType.GameDisplay;
        public int m_ScreenID = 0;
        public string m_CameraTag;

        public override bool isValid
        {
            get { return base.isValid && m_Width > 0 && m_Height > 0; }
        }
    }
}
