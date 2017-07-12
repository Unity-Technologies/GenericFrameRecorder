namespace UnityEngine.FrameRecorder
{

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public abstract class BaseRenderTextureInput : RecorderInput
    {
        public RenderTexture outputRT { get; set; }

        public int outputWidth { get; protected set; }
        public int outputHeight { get; protected set; }

        public void ReleaseBuffer()
        {
            if (outputRT != null)
            {
                if (outputRT == RenderTexture.active)
                    RenderTexture.active = null;

                outputRT.Release();
                outputRT = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                ReleaseBuffer();
        }
    }
}
