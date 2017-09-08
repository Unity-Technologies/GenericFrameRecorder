using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.Animations;
using UnityEditor;
using UnityEngine.FrameRecorder.Input;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

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
                                       .Replace(AnimationRecorderSettings.bindingToken,set.bindingType.Name)
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