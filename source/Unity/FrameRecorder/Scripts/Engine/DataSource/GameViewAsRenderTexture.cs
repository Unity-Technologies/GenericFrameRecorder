using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Recorder.FrameRecorder.Utilities;

namespace UnityEngine.Recorder.FrameRecorder.DataSource
{
    public class GameViewAsRenderTexture : CameraAsRenderTexture
    {
        GameViewWindow m_GameView;

        public void PrepareNewFrame(RecordingSession session)
        {
            if (TargetCamera == null)
            {
                var displayGO = new GameObject();
                displayGO.name = "CameraHostGO-" + displayGO.GetInstanceID();
                displayGO.transform.parent = session.m_RecorderGO.transform;
                var camera = displayGO.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Nothing;
                camera.cullingMask = 0;
                camera.renderingPath = RenderingPath.DeferredShading;
                camera.targetDisplay = 0;
                camera.rect = new Rect(0, 0, 1, 1);
                camera.depth = float.MaxValue;

                TargetCamera = camera;
            }

            base.PrepareNewFrame();
        }

        public GameViewAsRenderTexture( EImageSizeMode sizeMode, int width, int height)
            : base(sizeMode, width, height)
        {
            
        }

        public void BeginRecording()
        {
            m_GameView = new GameViewWindow();
            m_GameView.FullScreenGameWindow( m_SizeMode, m_Width, m_Height );
            m_Width = (int)m_GameView.size.x;
            m_Height = (int)m_GameView.size.y;
        }

        public void EndRecording()
        {
            if (m_GameView != null)
                m_GameView.RestoreGameWindow();
        }
    }
}
