namespace UnityEngine.FrameRecorder.Input
{
    public class RenderTextureInputSettings : InputSettings<RenderTextureInput>
    {
        public RenderTexture m_SourceRTxtr;

        public RenderTextureInputSettings()
        {
            m_DisplayName = "Pixels";
        }

        public override bool isValid {
            get
            {
                return m_SourceRTxtr != null; 
            }
        }

    }
}
