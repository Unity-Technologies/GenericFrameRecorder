using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.FrameRecorder.Input;

namespace UnityEngine.FrameRecorder
{
    [FrameRecorder(typeof(ImageRecorderSettings),"Video", "PNG, Jpeg, OpenEXR" )]
    public class ImageRecorder : GenericRecorder<ImageRecorderSettings>
    {
        string MakeFileName(RecordingSession session)
        {
            var fileName = m_Settings.m_DestinationPath;
            if (fileName.Length > 0 && !fileName.EndsWith("/"))
                fileName += "/";
            fileName += m_Settings.m_BaseFileName + recordedFramesCount + "." + m_Settings.m_OutputFormat;
            return fileName;
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (BaseRenderTextureInput)m_Inputs[0];

            var width = input.outputRT.width;
            var height = input.outputRT.height;
            
            var tex = new Texture2D(width, height, m_Settings.m_OutputFormat !=  PNGRecordeOutputFormat.EXR ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat, false);
            var backupActive = RenderTexture.active;
            RenderTexture.active = input.outputRT;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = backupActive;

            byte[] bytes;
            switch (m_Settings.m_OutputFormat)
            {
                case PNGRecordeOutputFormat.PNG:
                    bytes = tex.EncodeToPNG();
                    break;
                case PNGRecordeOutputFormat.JPEG:
                    bytes = tex.EncodeToJPG();
                    break;
                case PNGRecordeOutputFormat.EXR:
                    bytes = tex.EncodeToEXR();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UnityHelpers.Destroy(tex);

            if (!Directory.Exists(m_Settings.m_DestinationPath))
                Directory.CreateDirectory(m_Settings.m_DestinationPath);

            File.WriteAllBytes(MakeFileName(session), bytes);
        }
    }
}
