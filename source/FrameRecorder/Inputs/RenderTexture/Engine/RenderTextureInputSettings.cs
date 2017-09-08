namespace UnityEngine.Recorder.Input
{
    public class RenderTextureInputSettings : InputSettings<RenderTextureInput>
    {
        public RenderTexture m_SourceRTxtr;

        public override bool isValid
        {
            get
            {
                return m_SourceRTxtr != null; 
            }
        }

    }
}
