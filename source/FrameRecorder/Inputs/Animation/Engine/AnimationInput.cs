using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.Animations;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;
using UnityEngine.Rendering;

namespace UnityEngine.FrameRecorder.Input
{
    public class AnimationInput : RecorderInput
    {
        public GameObjectRecorder m_gameObjectRecorder;
        private float m_time;
        public override void BeginRecording(RecordingSession session)
        {
            var aniSettings = (settings as AnimationInputSettings);
            var srcGO= aniSettings.gameObject;
            m_gameObjectRecorder = new GameObjectRecorder {root = srcGO};
            m_gameObjectRecorder.BindComponent(srcGO, aniSettings.bindingType, true); 
            m_time = 0;
        }

        public void NewFrame(RecordingSession session)
        {
            if (session.recording && (settings as AnimationInputSettings).enabled )
            {
                m_gameObjectRecorder.TakeSnapshot(session.RecorderTime - m_time);
                m_time = session.RecorderTime;
            }
        }
    }
}