namespace UnityEngine.Recorder.Input
{
    public class CBRenderTextureInputSettings : InputSettings<CBRenderTextureInput>
    {
        public EImageSource source = EImageSource.GameDisplay;
        public EImageDimension m_RenderSize = EImageDimension.x720p_HD;
        public EImageAspect m_RenderAspect = EImageAspect.x16_9;
        public string m_CameraTag;
        public bool m_FlipFinalOutput = false;
        public bool m_ForceEvenSize = false;
        public bool m_AllowTransparency = false;
        public bool m_CaptureUI = false;

        public override bool isValid {
            get { return source != EImageSource.TaggedCamera || !string.IsNullOrEmpty(m_CameraTag); }
        }
    }
}
