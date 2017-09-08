using System.IO;
using UnityEditor;
using UnityEngine.FrameRecorder.Input;

namespace UnityEngine.FrameRecorder
{
    [FrameRecorder(typeof(AnimationRecorderSettings), "Animation", "Unity/Animation Recording")]
    public class AnimationRecorder : GenericRecorder<AnimationRecorderSettings>
    {
        public override void RecordFrame(RecordingSession session)
        {
            foreach (RecorderInput t in m_Inputs)
            {
                var aInput = t as AnimationInput;
                aInput.NewFrame(session);
            }
        }


        public override void EndRecording(RecordingSession ctx)
        {
            var ars = ctx.settings as AnimationRecorderSettings;
            var dir = Path.GetDirectoryName("Assets/"+ars.outputPath);
            Directory.CreateDirectory(dir);

            for (int i = 0; i < m_Inputs.Count; ++i)
            {
                var set = (settings.inputsSettings[i] as AnimationInputSettings);
                if (set.enabled)
                {
                    var aInput = m_Inputs[i] as AnimationInput;
                    AnimationClip clip = new AnimationClip();
                    var clipName = "Assets/" + ars.outputPath
                                       .Replace(AnimationRecorderSettings.goToken, set.gameObject.name)
                                       .Replace(AnimationRecorderSettings.inputToken,(i+1).ToString("000"))
                                       .Replace(AnimationRecorderSettings.takeToken, ars.take.ToString("000"))+".anim";
                    AssetDatabase.CreateAsset(clip, clipName);
                    aInput.m_gameObjectRecorder.SaveToClip(clip);
                    aInput.m_gameObjectRecorder.ResetRecording();
                }
            }

            ars.take++;
            base.EndRecording(ctx);
        }
    }
}