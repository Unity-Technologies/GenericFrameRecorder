using System;

namespace UnityEngine.Recorder.FrameRecorder.DataSource
{
    public abstract class RenderTextureInput : RecorderInput
    {
        public RenderTexture outputRT { get; set; }

        public int outputWidth { get; protected set; }
        public int outputHeight { get; protected set; }

        public void ReleaseBuffer()
        {
            if (outputRT != null)
            {
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
