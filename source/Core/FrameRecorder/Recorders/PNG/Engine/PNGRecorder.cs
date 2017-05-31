using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Recorder.FrameRecorder.DataSource;
using UnityEngine.Recorder.FrameRecorder.Utilities;

namespace UnityEngine.Recorder.FrameRecorder
{
    [FrameRecorder(typeof(PNGRecorderSettings),"Video", "PNG" )]
    public class PNGRecorder : BaseImageRecorder<PNGRecorderSettings>
    {
        string MakeFileName(RecordingSession session)
        {
            var fileName = m_Settings.m_DestinationPath;
            if (fileName.Length > 0 && !fileName.EndsWith("/"))
                fileName += "/";
            fileName += m_Settings.m_BaseFileName + recordedFramesCount + ".png";
            return fileName;
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (m_Sources.Count != 1)
                throw new Exception("Unsupported number of sources");

            var source = (RenderTextureInput)m_Sources[0];

            var width = source.outputRT.width;
            var height = source.outputRT.height;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            var backupActive = RenderTexture.active;
            RenderTexture.active = source.outputRT;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = backupActive;

            var bytes = tex.EncodeToPNG();
            UnityHelpers.Destroy(tex);

            if (!Directory.Exists(m_Settings.m_DestinationPath))
                Directory.CreateDirectory(m_Settings.m_DestinationPath);

            File.WriteAllBytes(MakeFileName(session), bytes);
        }

    }
}
