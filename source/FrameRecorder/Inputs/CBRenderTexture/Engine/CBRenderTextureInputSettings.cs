namespace UnityEngine.FrameRecorder.Input
{
    public class CBRenderTextureInputSettings : InputSettings<CBRenderTextureInput>
    {
        public EImageSource source = EImageSource.MainCamera;
        public EImageDimension m_RenderSize = EImageDimension.Window;
        public EImageAspect m_RenderAspect = EImageAspect.x5_4;
        public string m_CameraTag;
        public bool m_FlipVertical = false;
        public bool m_ForceEvenSize = false;
        public bool m_AllowTransparency = false;

        public override bool isValid {
            get { return source != EImageSource.TaggedCamera || !string.IsNullOrEmpty(m_CameraTag); }
        }
    }
}
