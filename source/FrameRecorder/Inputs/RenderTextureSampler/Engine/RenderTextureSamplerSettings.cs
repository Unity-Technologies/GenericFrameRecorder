namespace UnityEngine.Recorder.Input
{
    /// <summary>
    /// What is it: 
    /// Motivation: 
    /// </summary>
    public enum ESuperSamplingCount
    {
        x1 = 1,
        x2 = 2,
        x4 = 4,
        x8 = 8,
        x16 = 16,
    }

    public class RenderTextureSamplerSettings : InputSettings<RenderTextureSampler>
    {
        public EImageSource source = EImageSource.GameDisplay;
        public EImageDimension m_RenderSize = EImageDimension.x720p_HD;
        public EImageDimension m_FinalSize = EImageDimension.x720p_HD;
        public EImageAspect m_AspectRatio = EImageAspect.x16_9;
        public ESuperSamplingCount m_SuperSampling = ESuperSamplingCount.x1;
        public float m_SuperKernelPower = 16f;
        public float m_SuperKernelScale = 1f;
        public RenderTexture m_RenderTexture;
        public string m_CameraTag;
        public bool m_ForceEvenSize;
        public ColorSpace m_ColorSpace = ColorSpace.Gamma;
        public bool m_FlipFinalOutput = false;

        public override bool isValid {
            get
            {
                return source == EImageSource.GameDisplay || source == EImageSource.MainCamera; 
            }
        }
    }
}