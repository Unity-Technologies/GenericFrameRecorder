using System;
using System.IO;

namespace UnityEngine.FrameRecorder
{
    [FrameRecorder(typeof(ImageRecorderSettings),"Video", "Unity/Image sequence" )]
    public class ImageRecorder : GenericRecorder<ImageRecorderSettings>
    {

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            m_Settings.m_DestinationPath.CreateDirectory();

            return true;
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
            string ext;
            switch (m_Settings.m_OutputFormat)
            {
                case PNGRecordeOutputFormat.PNG:
                    bytes = tex.EncodeToPNG();
                    ext = "png";
                    break;
                case PNGRecordeOutputFormat.JPEG:
                    bytes = tex.EncodeToJPG();
                    ext = "jpg";
                    break;
                case PNGRecordeOutputFormat.EXR:
                    bytes = tex.EncodeToEXR();
                    ext = "exr";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UnityHelpers.Destroy(tex);

            var fileName = m_Settings.m_BaseFileName.BuildFileName( session, recordedFramesCount, width, height, ext);
            var path = Path.Combine( m_Settings.m_DestinationPath.GetFullPath(), fileName);


            File.WriteAllBytes( path, bytes);
        }
    }
}
