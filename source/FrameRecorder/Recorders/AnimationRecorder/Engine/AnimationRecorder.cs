using System;
using System.Collections.Generic;
using System.Linq;
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
        public static string rootAssetPath = "Assets/AnimRecorder/";
        
   
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
            var path = settings.m_BaseFileName.BuildFileName(ctx, 0, 0, 0, "anim");
            System.IO.Directory.CreateDirectory(rootAssetPath);
            for (int i = 0; i < m_Inputs.Count; ++i)
            {
                var set = (settings.inputsSettings[i] as AnimationInputSettings);
                if (set.enabled)
                {
                    var aInput = m_Inputs[i] as AnimationInput;
                    AnimationClip clip = new AnimationClip();
                    var clipname = rootAssetPath + set.gameObject.name+"-"+System.IO.Path.GetRandomFileName() + ".anim";
                    AssetDatabase.CreateAsset(clip, clipname);
                    aInput.m_gameObjectRecorder.SaveToClip(clip);
                    aInput.m_gameObjectRecorder.ResetRecording();
                }
            }

            base.EndRecording(ctx);
        }
    }
}