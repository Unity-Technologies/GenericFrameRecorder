using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;
using UnityEngine.Recorder.FrameRecorder.Utilities;

public class AdamBeautyInput : RenderTextureInput
{
    [HideInInspector]
    public Shader superShader;
    [HideInInspector]
    public Shader accumulateShader;
    [HideInInspector]
    public Shader normalizeShader;

    RenderTexture m_renderRT;
    RenderTexture[] m_accumulateRTs = new RenderTexture[2];
    int m_renderWidth, m_renderHeight, m_outputWidth, m_outputHeight;

    Material m_superMaterial;
    Material m_accumulateMaterial;
    Material m_normalizeMaterial;

    class HookedCamera
    {
        public Camera camera;
        public RenderTexture textureBackup;
    }

    List<HookedCamera> m_hookedCameras;

    Vector2[] m_samples;

    AdamBeautyInputSettings adamSettings
    {
        get { return (AdamBeautyInputSettings)settings; }
    }

    void GenerateSamplesMSAA(Vector2[] samples, ESuperSamplingCount sc)
    {
        switch (sc)
        {
            case ESuperSamplingCount.x1:
                samples[0] = new Vector2(0.0f, 0.0f);
                break;
            case ESuperSamplingCount.x2:
                samples[0] = new Vector2(4.0f, 4.0f);
                samples[1] = new Vector2(-4.0f, -4.0f);
                break;
            case ESuperSamplingCount.x4:
                samples[0] = new Vector2(-2.0f, -6.0f);
                samples[1] = new Vector2(6.0f, -2.0f);
                samples[2] = new Vector2(-6.0f, 2.0f);
                samples[3] = new Vector2(2.0f, 6.0f);
                break;
            case ESuperSamplingCount.x8:
                samples[0] = new Vector2(1.0f, -3.0f);
                samples[1] = new Vector2(-1.0f, 3.0f);
                samples[2] = new Vector2(5.0f, 1.0f);
                samples[3] = new Vector2(-3.0f, -5.0f);

                samples[4] = new Vector2(-5.0f, 5.0f);
                samples[5] = new Vector2(-7.0f, -1.0f);
                samples[6] = new Vector2(3.0f, 7.0f);
                samples[7] = new Vector2(7.0f, -7.0f);
                break;
            case ESuperSamplingCount.x16:
                samples[0] = new Vector2(1.0f, 1.0f);
                samples[1] = new Vector2(-1.0f, -3.0f);
                samples[2] = new Vector2(-3.0f, 2.0f);
                samples[3] = new Vector2(4.0f, -1.0f);

                samples[4] = new Vector2(-5.0f, -2.0f);
                samples[5] = new Vector2(2.0f, 5.0f);
                samples[6] = new Vector2(5.0f, 3.0f);
                samples[7] = new Vector2(3.0f, -5.0f);

                samples[8] = new Vector2(-2.0f, 6.0f);
                samples[9] = new Vector2(0.0f, -7.0f);
                samples[10] = new Vector2(-4.0f, -6.0f);
                samples[11] = new Vector2(-6.0f, 4.0f);

                samples[12] = new Vector2(-8.0f, 0.0f);
                samples[13] = new Vector2(7.0f, -4.0f);
                samples[14] = new Vector2(6.0f, 7.0f);
                samples[15] = new Vector2(-7.0f, -8.0f);
                break;
            default:
                Debug.LogError("Not expected sample count: " + sc);
                return;
        }
        const float oneOverSixteen = 1.0f / 16.0f;
        Vector2 halfHalf = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = samples[i] * oneOverSixteen + halfHalf;
        }
    }

    public override void BeginRecording(RecordingSession session)
    {
        superShader = Shader.Find("Hidden/Volund/BS4SuperShader");
        accumulateShader = Shader.Find("Hidden/BeautyShot/Accumulate");
        normalizeShader = Shader.Find("Hidden/BeautyShot/Normalize");

        // Below here is considered 'void Start()', but we run it for directly "various reasons".
        if (adamSettings.m_FinalSize > adamSettings.m_RenderSize)
            throw new UnityException("Upscaling is not supported! Output dimension must be smaller or equal to render dimension.");

        // Calculate aspect and render/output sizes
        // Clamp size to 16K, which is the min always supported size in d3d11
        // Force output to divisible by two as x264 doesn't approve of odd image dimensions.
        var aspect = (int)adamSettings.m_AspectRatio / 10000f;
        m_renderHeight = (int)adamSettings.m_RenderSize;
        m_renderWidth = Mathf.Min(16 * 1024, Mathf.RoundToInt(m_renderHeight * aspect));
        m_outputHeight = (int)adamSettings.m_FinalSize;
        m_outputWidth = Mathf.Min(16 * 1024, Mathf.RoundToInt(m_outputHeight * aspect)) & ~1;

        m_superMaterial = new Material(superShader);
        m_superMaterial.hideFlags = HideFlags.DontSave;

        m_accumulateMaterial = new Material(accumulateShader);
        m_accumulateMaterial.hideFlags = HideFlags.DontSave;

        m_normalizeMaterial = new Material(normalizeShader);
        m_normalizeMaterial.hideFlags = HideFlags.DontSave;

        m_renderRT = new RenderTexture(m_renderWidth, m_renderHeight, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
        if (adamSettings.m_SuperSampling != ESuperSamplingCount.x1)
        {
            for (int i = 0; i < 2; ++i)
            {
                m_accumulateRTs[i] = new RenderTexture(m_renderWidth, m_renderHeight, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
                m_accumulateRTs[i].Create();
            }
        }
        var rt = new RenderTexture(m_outputWidth, m_outputHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        rt.Create();
        outputRT = rt;
        m_samples = new Vector2[(int)adamSettings.m_SuperSampling];
        GenerateSamplesMSAA(m_samples, adamSettings.m_SuperSampling);

        m_hookedCameras = new List<HookedCamera>();
    }

    public override void NewFrameStarting(RecordingSession session)
    {
        switch (adamSettings.source)
        {
            case EImageSource.GameDisplay:
            {
                // Find all cameras targetting Display
                foreach (var cam in Resources.FindObjectsOfTypeAll<Camera>())
                {
                    var hookedCam = m_hookedCameras.Find((x) => cam == x.camera);
                    if ( hookedCam != null)
                    {
                        // Should we keep it?
                        if (cam.targetDisplay != 0 || !cam.enabled)
                        {
                            cam.targetTexture = hookedCam.textureBackup;
                            m_hookedCameras.Remove(hookedCam);
                        }
                        continue;
                    }

                    if (!cam.enabled || cam.targetDisplay != 0)
                        continue;

                    hookedCam = new HookedCamera() {camera = cam, textureBackup = cam.targetTexture };
                    cam.targetTexture = m_renderRT;
                    m_hookedCameras.Add(hookedCam);
                }
                break;
            }
            case EImageSource.MainCamera:
            {
                var cam = Camera.main;
                if (m_hookedCameras.Count > 0  )
                {
                    if (m_hookedCameras[0].camera != cam)
                    {
                        m_hookedCameras[0].camera.targetTexture = m_hookedCameras[0].textureBackup;
                        m_hookedCameras.Clear();
                    }
                    else
                        break;
                }
                if (!cam.enabled )
                    break;

                var hookedCam = new HookedCamera() {camera = cam, textureBackup = cam.targetTexture };
                m_hookedCameras.Add(hookedCam);
                break;
            }
            case EImageSource.TaggedCamera:
                break;
            case EImageSource.RenderTexture:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (m_hookedCameras != null)
            {
                foreach (var c in m_hookedCameras)
                {
                    if (c != null)
                    {
                        c.camera.targetTexture = c.textureBackup;
                    }
                }
                m_hookedCameras.Clear();
            }

            UnityHelpers.Destroy(m_renderRT);
            foreach (var rt in m_accumulateRTs)
                UnityHelpers.Destroy(rt);
            UnityHelpers.Destroy(m_superMaterial);
            UnityHelpers.Destroy(m_accumulateMaterial);
            UnityHelpers.Destroy(m_normalizeMaterial);
        }

        base.Dispose(disposing);
    }

    public override void NewFrameReady(RecordingSession session)
    {
        foreach (var hookedCam in m_hookedCameras)
        {
            RenderTexture src;
            if (adamSettings.m_SuperSampling == ESuperSamplingCount.x1 || hookedCam.textureBackup == hookedCam.camera.targetTexture)
            {
                src = hookedCam.camera.targetTexture;
            }
            else
            {
                src = PerformSubSampling(hookedCam.camera);
            }

            if (adamSettings.m_RenderSize == adamSettings.m_FinalSize)
            {
                // Blit with normalization if sizes match.
                m_normalizeMaterial.SetFloat("_NormalizationFactor", 1.0f / (float)adamSettings.m_SuperSampling);
                Graphics.Blit(src, outputRT, m_normalizeMaterial);
            }
            else
            {
                // Ideally we would use a separable filter here, but we're massively bound by readback and disk anyway for hi-res.
                m_superMaterial.SetVector("_Target_TexelSize", new Vector4(1f / m_outputWidth, 1f / m_outputHeight, m_outputWidth, m_outputHeight));
                m_superMaterial.SetFloat("_KernelCosPower", adamSettings.m_SuperKernelPower);
                m_superMaterial.SetFloat("_KernelScale", adamSettings.m_SuperKernelScale);
                m_superMaterial.SetFloat("_NormalizationFactor", 1.0f / (float)adamSettings.m_SuperSampling);
                Graphics.Blit(src, outputRT, m_superMaterial);
            }
        }
    }

    void ShiftProjectionMatrix(Camera camera, Vector2 sample)
    {
        camera.ResetProjectionMatrix();
        Matrix4x4 projectionMatrix = camera.projectionMatrix;
        float dx = sample.x / m_renderWidth;
        float dy = sample.y / m_renderHeight;
        projectionMatrix.m02 += dx;
        projectionMatrix.m12 += dy;
        camera.projectionMatrix = projectionMatrix;
    }

    RenderTexture PerformSubSampling(Camera cam)
    {
        var src = cam.targetTexture;
        src.wrapMode = TextureWrapMode.Clamp;
        src.filterMode = FilterMode.Point;

        RenderTexture dst = null;

        Graphics.SetRenderTarget(m_accumulateRTs[0]);
        GL.Clear(false, true, Color.black);

        // Render n times the camera and accumulate renders.
        var oldProjectionMatrix = cam.projectionMatrix;
        for (int i = 0, n = (int)adamSettings.m_SuperSampling; i < n; i++)
        {
            ShiftProjectionMatrix(cam, m_samples[i] - new Vector2(0.5f, 0.5f));
            cam.Render();

            var accumulateInto = m_accumulateRTs[(i + 1) % 2];
            var accumulatedWith = m_accumulateRTs[i % 2];
            m_accumulateMaterial.SetTexture("_PreviousTexture", accumulatedWith);
            Graphics.Blit(src, accumulateInto, m_accumulateMaterial);

            dst = accumulateInto;
        }
        cam.projectionMatrix = oldProjectionMatrix;

        return dst;
    }
}
