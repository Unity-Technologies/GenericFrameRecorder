using System;

namespace UnityEngine.FrameRecorder
{
    public enum ESuperSamplingCount
    {
        x1 = 1,
        x2 = 2,
        x4 = 4,
        x8 = 8,
        x16 = 16,
    }

    public enum EImageDimension
    {
        //x8640p_16K = 8640,
        x4320p_8K = 4320,
        x2880p_5K = 2880,
        x2160p_4K = 2160,
        x1440p_QHD = 1440,
        x1080p_FHD = 1080,
        x720p_HD = 720,
        x480p = 480,
        x240p = 240,
        Window = 0,
        //Manual = -1, for user entering his own resolution
    }

    public enum EImageAspect
    {
        x16_9,
        x16_10,
        x19_10,
        x5_4,
        x4_3,
    }

    public class AspectRatioHelper
    {
        public static float GetRealAR(EImageAspect aspectRatio)
        {
            switch (aspectRatio)
            {
                case EImageAspect.x16_9:
                    return 16.0f / 9.0f;
                case EImageAspect.x16_10:
                    return 16.0f / 10.0f;
                case EImageAspect.x19_10:
                    return 19.0f / 10.0f;
                case EImageAspect.x5_4:
                    return 5.0f / 4.0f;
                case EImageAspect.x4_3:
                    return 4.0f / 3.0f;
                default:
                    throw new ArgumentOutOfRangeException("aspectRatio", aspectRatio, null);
            }
        }
    }

}
