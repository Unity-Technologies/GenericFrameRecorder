using System;
using System.IO;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UTJ.FrameCapturer.Recorders
{
    [FrameRecorder(typeof(GIFRecorderSettings),"Video", "UTJ/GIF" )]
    public class GIFRecorder : BaseImageRecorder<GIFRecorderSettings>
    {
        fcAPI.fcGifContext m_ctx;
        fcAPI.fcStream m_stream;

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            if (!Directory.Exists(m_Settings.m_DestinationPath))
                Directory.CreateDirectory(m_Settings.m_DestinationPath);

            return true;
        }

        public override void EndRecording(RecordingSession session)
        {
            m_ctx.Release();
            m_stream.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (m_Sources.Count != 1)
                throw new Exception("Unsupported number of sources");

            var source = (RenderTextureInput)m_Sources[0];
            var frame = source.outputRT;

            if(!m_ctx)
            {
                var settings = m_Settings.m_GifEncoderSettings;
                settings.width = frame.width;
                settings.height = frame.height;
                m_ctx = fcAPI.fcGifCreateContext(ref settings);
                m_stream = fcAPI.fcCreateFileStream(BuildOutputPath(session));
                fcAPI.fcGifAddOutputStream(m_ctx, m_stream);
            }

            fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                fcAPI.fcGifAddFramePixels(m_ctx, data, fmt, session.m_CurrentFrameStartTS);
            });
        }

        string BuildOutputPath(RecordingSession session)
        {
            var outputPath = m_Settings.m_DestinationPath;
            if (outputPath.Length > 0 && !outputPath.EndsWith("/"))
                outputPath += "/";
            outputPath += (settings as GIFRecorderSettings).m_BaseFileName + ".gif";
            return outputPath;
        }
    }
}
