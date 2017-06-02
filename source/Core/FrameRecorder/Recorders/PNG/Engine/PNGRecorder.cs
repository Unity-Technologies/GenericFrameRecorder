using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.FrameRecorder.Input;

namespace UnityEngine.FrameRecorder
{
    [FrameRecorder(typeof(PNGRecorderSettings),"Video", "PNG" )]
    public class PNGRecorder : GenericRecorder<PNGRecorderSettings>
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
            if (m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (RenderTextureInput)m_Inputs[0];

            var width = input.outputRT.width;
            var height = input.outputRT.height;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            var backupActive = RenderTexture.active;
            RenderTexture.active = input.outputRT;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = backupActive;

            var bytes = tex.EncodeToPNG();
            UnityHelpers.Destroy(tex);

            if (!Directory.Exists(m_Settings.m_DestinationPath))
                Directory.CreateDirectory(m_Settings.m_DestinationPath);

            File.WriteAllBytes(MakeFileName(session), bytes);
        }

        public override List<RecorderInputSetting> DefaultInputs()
        {
            var settings = new List<RecorderInputSetting>();
            var setting = ScriptableObject.CreateInstance(typeof(AdamBeautyInputSettings)) as AdamBeautyInputSettings;

            settings.Add(setting);
            return settings;
        }

    }
}
