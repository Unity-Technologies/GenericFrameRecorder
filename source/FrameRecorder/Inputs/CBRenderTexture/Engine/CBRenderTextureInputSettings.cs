namespace UnityEngine.FrameRecorder.Input
{
    public class CBRenderTextureInputSettings : InputSettings<CBRenderTextureInput>
    {
        public EImageSource source = EImageSource.GameDisplay;
        public EImageDimension m_RenderSize = EImageDimension.Window;
        public EImageAspect m_RenderAspect = EImageAspect.x5_4;
        public string m_CameraTag;
        public bool m_FlipVertical = false;
    }
}
