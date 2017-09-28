using UnityEditor.Experimental.Animations;

namespace UnityEngine.Recorder.Input
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
            foreach (var binding in aniSettings.bindingType)
            {
                m_gameObjectRecorder.BindComponent(srcGO, binding, aniSettings.recursive); 
            }
            m_time = session.RecorderTime;
        }

        public override void NewFrameReady(RecordingSession session)
        {
            if (session.recording && (settings as AnimationInputSettings).enabled )
            {
                m_gameObjectRecorder.TakeSnapshot(session.RecorderTime - m_time);
                m_time = session.RecorderTime;
            }
        }
    }
}