using System;

namespace UnityEngine.Recorder.Input
{
    public class CBRenderTextureInputSettings : ImageInputSettings
    {
        public EImageSource source = EImageSource.ActiveCameras;
        public string m_CameraTag;
        public bool m_FlipFinalOutput = false;
        public bool m_AllowTransparency = false;
        public bool m_CaptureUI = false;

        public override Type inputType
        {
            get { return typeof(CBRenderTextureInput); }
        }

        public override bool isValid
        {
            get
            {
                return base.isValid && 
                       (source != EImageSource.TaggedCamera || !string.IsNullOrEmpty(m_CameraTag)); 
            }
        }
    }
}
