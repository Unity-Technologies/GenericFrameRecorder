using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine.Rendering;

namespace UnityEngine.FrameRecorder.Input
{
    public class CBRenderTextureInput : BaseRenderTextureInput
    {
        static int m_ModifiedResolutionCount = 0;
        bool m_ModifiedResulution = false;
        Shader          m_shCopy;
        Material        m_mat_copy;
        Mesh            m_quad;
        CommandBuffer   m_cbCopyFB;
        CommandBuffer   m_cbCopyGB;
        CommandBuffer   m_cbClearGB;
        CommandBuffer   m_cbCopyVelocity;
        Camera          m_Camera;
        bool            m_cameraChanged;

        public CBRenderTextureInputSettings cbSettings
        {
            get { return (CBRenderTextureInputSettings)settings; }
        }

        public Camera TargetCamera
        {
            get { return m_Camera; }

            set
            {
                if (m_Camera != value)
                {
                    ReleaseCamera();
                    m_Camera = value;
                    m_cameraChanged = true;
                }
            }
        }

        public Shader CopyShader
        {
            get
            {
                if (m_shCopy == null)
                {
                    m_shCopy = Shader.Find("Hidden/FrameRecorder/CopyFrameBuffer");
                }
                return m_shCopy;
            }

            set { m_shCopy = value; }
        }

        public override void BeginRecording(RecordingSession session)
        {
            m_quad = CreateFullscreenQuad();
            switch (cbSettings.source)
            {
                case EImageSource.GameDisplay:
                case EImageSource.MainCamera:
                {
                    int screenWidth  = Screen.width;
                    int screenHeight = Screen.height;
#if UNITY_EDITOR
                    switch (cbSettings.m_RenderSize)
                    {
                        case EImageDimension.Window:
                        {
                            GameViewSize.GetGameRenderSize(out screenWidth, out screenHeight);
                            outputWidth = screenWidth;
                            outputHeight = screenHeight;

                            if (cbSettings.m_ForceEvenSize)
                            {
                                outputWidth = (outputWidth + 1) & ~1;
                                outputHeight = (outputHeight + 1) & ~1;
                            }

                            break;
                        }

                        default:
                        {
                            outputHeight = (int)cbSettings.m_RenderSize;
                            outputWidth = (int)(outputHeight * AspectRatioHelper.GetRealAR(cbSettings.m_RenderAspect));

                            if (cbSettings.m_ForceEvenSize)
                            {
                                outputWidth = (outputWidth + 1) & ~1;
                                outputHeight = (outputHeight + 1) & ~1;
                            }

                            var size = GameViewSize.SetCustomSize(outputWidth, outputHeight);
                            if (size == null)
                                size = GameViewSize.AddSize(outputWidth, outputHeight);

                            if( m_ModifiedResolutionCount == 0 )
                                GameViewSize.BackupCurrentSize();
                            else
                            {
                                if (size != GameViewSize.currentSize)
                                {
                                    Debug.LogError("Requestion a resultion change while a recorder's input has already requested one! Undefined behaviour.");
                                }
                            }
                            m_ModifiedResolutionCount++;
                            m_ModifiedResulution = true;
                            GameViewSize.SelectSize(size);
                            break;
                        }
                    }
#endif
                        break;
                }
                case EImageSource.TaggedCamera:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void NewFrameStarting(RecordingSession session)
        {
            switch (cbSettings.source)
            {
                case EImageSource.GameDisplay:
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
                    break;
                }

                case EImageSource.MainCamera:
                {
                    if (TargetCamera != Camera.main)
                    {
                        TargetCamera = Camera.main;
                        m_cameraChanged = true;
                    }
                    break;
                }
                case EImageSource.TaggedCamera:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newTexture = PrepFrameRenderTexture();

            // initialize command buffer
            if (m_Camera != null && (m_cameraChanged || newTexture))
            {
                if (m_cbCopyFB != null)
                {
                    m_Camera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_cbCopyFB);
                    m_cbCopyFB.Release();
                }

                // TODO: This should not be here!!!
                m_mat_copy = new Material(CopyShader);
                if (m_Camera.targetTexture != null || cbSettings.m_FlipVertical )
                    m_mat_copy.EnableKeyword("OFFSCREEN");

                if( cbSettings.m_AllowTransparency )
                    m_mat_copy.EnableKeyword("TRANSPARENCY_ON");

                var tid = Shader.PropertyToID("_TmpFrameBuffer");
                m_cbCopyFB = new CommandBuffer { name = "Recorder: copy frame buffer" };
                m_cbCopyFB.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Bilinear);
                m_cbCopyFB.Blit(BuiltinRenderTextureType.CurrentActive, tid);
                m_cbCopyFB.SetRenderTarget(outputRT);
                m_cbCopyFB.DrawMesh(m_quad, Matrix4x4.identity, m_mat_copy, 0, 0);
                m_cbCopyFB.ReleaseTemporaryRT(tid);
                m_Camera.AddCommandBuffer(CameraEvent.AfterEverything, m_cbCopyFB);

                m_cameraChanged = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseCamera();

#if UNITY_EDITOR
                if (m_ModifiedResulution)
                {
                    m_ModifiedResolutionCount --;
                    if( m_ModifiedResolutionCount == 0 )
                        GameViewSize.RestoreSize();
                }
#endif

            }

            base.Dispose(disposing);
        }

        protected virtual void ReleaseCamera()
        {
            if (m_cbCopyFB != null)
            {
                if (m_Camera != null)
                    m_Camera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_cbCopyFB);

                m_cbCopyFB.Release();
                m_cbCopyFB = null;
            }

            if (m_mat_copy != null)
                UnityHelpers.Destroy(m_mat_copy);
        }

        bool PrepFrameRenderTexture()
        {
            if (outputRT != null)
            {
                if (outputRT.IsCreated() && outputRT.width == outputWidth && outputRT.height == outputHeight)
                {
                    return false;
                }

                ReleaseBuffer();
            }

            outputRT = new RenderTexture(outputWidth, outputHeight, 0, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Repeat
            };
            outputRT.Create();

            return true;
        }

        public static Mesh CreateFullscreenQuad()
        {
            var vertices = new Vector3[4]
            {
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, -1.0f, 0.0f),
            };
            var indices = new[] { 0, 1, 2, 2, 3, 0 };

            var r = new Mesh
            {
                vertices = vertices,
                triangles = indices
            };
            return r;
        }
    }
}
