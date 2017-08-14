using System;
using System.IO;
using UnityEngine;
using UnityEngine.FrameRecorder;

namespace UTJ.FrameCapturer.Recorders
{
    [FrameRecorder(typeof(PNGRecorderSettings),"Video", "UTJ/PNG" )]
    public class PNGRecorder : GenericRecorder<PNGRecorderSettings>
    {
        fcAPI.fcPngContext m_ctx;

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            m_Settings.m_DestinationPath.CreateDirectory();

            m_ctx = fcAPI.fcPngCreateContext(ref m_Settings.m_PngEncoderSettings);
            return m_ctx;
        }

        public override void EndRecording(RecordingSession session)
        {
            m_ctx.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (BaseRenderTextureInput)m_Inputs[0];
            var frame = input.outputRT;
            var fileName = m_Settings.m_BaseFileName.BuildFileName( session, recordedFramesCount, frame.width, frame.height, "mp4");
            var path = Path.Combine(m_Settings.m_DestinationPath.GetFullPath(), fileName);

            fcAPI.fcLock(frame, (data, fmt) =>
            {
                fcAPI.fcPngExportPixels(m_ctx, path, data, frame.width, frame.height, fmt, 0);
            });
        }

    }
}
