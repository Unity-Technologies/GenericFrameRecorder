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
        x8640p_16K = 8640,
        x4320p_8K = 4320,
        x2880p_5K = 2880,
        x2160p_4K = 2160,
        x1440p_QHD = 1440,
        x1080p_FHD = 1080,
        x720p_HD = 720,
        x480p = 480,
        x240p = 240,
        Manual = 0
    }

    public enum EImageAspect
    {
        x16_9 = 17777,
        x16_10 = 16000,
        x19_10 = 19000,
        x5_4 = 12500,
        x4_3 = 13333,
    }

}
