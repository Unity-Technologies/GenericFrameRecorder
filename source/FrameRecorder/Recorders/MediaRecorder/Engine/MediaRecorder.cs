using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Collections;
using UnityEngine.FrameRecorder.Input;
using UnityEditor;
using UnityEditor.Media;

namespace UnityEngine.FrameRecorder
{
#if RECORD_AUDIO_MIXERS
    class WavWriter
    {
        BinaryWriter binwriter;

        // Use this for initialization
        public void Start (string filename)
        {
            var stream = new FileStream (filename, FileMode.Create);
            binwriter = new BinaryWriter (stream);
            for(int n = 0; n < 44; n++)
                binwriter.Write ((byte)0);
        }

        public void Stop()
        {
            var closewriter = binwriter;
            binwriter = null;
            int subformat = 3; // float
            int numchannels = 2;
            int numbits = 32;
            int samplerate = AudioSettings.outputSampleRate;
            Debug.Log ("Closing file");
            long pos = closewriter.BaseStream.Length;
            closewriter.Seek (0, SeekOrigin.Begin);
            closewriter.Write ((byte)'R'); closewriter.Write ((byte)'I'); closewriter.Write ((byte)'F'); closewriter.Write ((byte)'F');
            closewriter.Write ((uint)(pos - 8));
            closewriter.Write ((byte)'W'); closewriter.Write ((byte)'A'); closewriter.Write ((byte)'V'); closewriter.Write ((byte)'E');
            closewriter.Write ((byte)'f'); closewriter.Write ((byte)'m'); closewriter.Write ((byte)'t'); closewriter.Write ((byte)' ');
            closewriter.Write ((uint)16);
            closewriter.Write ((ushort)subformat);
            closewriter.Write ((ushort)numchannels);
            closewriter.Write ((uint)samplerate);
            closewriter.Write ((uint)((samplerate * numchannels * numbits) / 8));
            closewriter.Write ((ushort)((numchannels * numbits) / 8));
            closewriter.Write ((ushort)numbits);
            closewriter.Write ((byte)'d'); closewriter.Write ((byte)'a'); closewriter.Write ((byte)'t'); closewriter.Write ((byte)'a');
            closewriter.Write ((uint)(pos - 36));
            closewriter.Seek ((int)pos, SeekOrigin.Begin);
            closewriter.Flush ();
        }

        public void Feed(NativeArray<float> data)
        {
            Debug.Log ("Writing wav chunk " + data.Length);

            if (binwriter == null)
                return;

            for(int n = 0; n < data.Length; n++)
                binwriter.Write (data[n]);
        }
    }
#endif

    [FrameRecorder(typeof(MediaRecorderSettings),"Video", "Unity/Movie" )]
    public class MediaRecorder : GenericRecorder<MediaRecorderSettings>
    {
        private string       m_OutputFilePath;
        private MediaEncoder m_Encoder;
#if RECORD_AUDIO_MIXERS
	private WavWriter[]  m_WavWriters;
#endif
	private Texture2D    m_ReadBackTexture;
	private bool         m_OriginalFlipVertical;

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session))
                return false;

            if (!Directory.Exists(m_Settings.m_DestinationPath))
	    {
                try
		{
		    Directory.CreateDirectory(m_Settings.m_DestinationPath);
		}
		catch
		{
		    Debug.LogError(string.Format(
				       "MediaRecorder output directory \"{0}\" could not be created.",
				       m_Settings.m_DestinationPath));
		    return false;
		}
	    }

            var input = (BaseRenderTextureInput)m_Inputs[0];
            if (input == null)
            {
                if (settings.m_Verbose)
                    Debug.Log("MediaRecorder could not find input.");
                return false;
            }

            var width = input.outputWidth;
            var height = input.outputHeight;
            if (width <= 0 || height <= 0)
            {
                if (settings.m_Verbose)
                    Debug.Log(string.Format(
                        "MovieRecorder got invalid input resolution {0} x {1}.", width, height));
                return false;
            }

            m_OutputFilePath = BuildOutputPath(m_Settings.m_BaseFileName);

	    var cbRenderTextureInput = input as CBRenderTextureInput;
	    if (cbRenderTextureInput != null)
	    {
		m_OriginalFlipVertical = cbRenderTextureInput.cbSettings.m_FlipVertical;
		// Video recording expects first line to be the topmost.
		cbRenderTextureInput.cbSettings.m_FlipVertical = true;
	    }

	    bool includeAlphaFromTexture = cbRenderTextureInput != null && cbRenderTextureInput.cbSettings.m_AllowTransparency;
	    if (includeAlphaFromTexture && m_Settings.m_OutputFormat == MediaRecorderOutputFormat.MP4)
	    {
		Debug.LogWarning("Mp4 format does not support alpha.");
		includeAlphaFromTexture = false;
	    }

            var videoAttrs = new VideoTrackAttributes()
            {
                frameRate = RationalFromDouble(session.settings.m_FrameRate),
                width = (uint)width,
                height = (uint)height,
                includeAlpha = includeAlphaFromTexture
            };

            if (settings.m_Verbose)
                Debug.Log(
                    string.Format(
                        "MovieRecorder starting to write video {0}x{1}@[{2}/{3}] fps into {4}",
                        width, height, videoAttrs.frameRate.numerator,
                        videoAttrs.frameRate.denominator, m_OutputFilePath));
	    
	    var audioInput = (AudioInput)m_Inputs[1];
            var audioAttrsList = new List<UnityEditor.Media.AudioTrackAttributes>();
            var audioAttrs =
                new UnityEditor.Media.AudioTrackAttributes()
                {
                    sampleRate = new MediaRational
                    {
                        numerator = audioInput.sampleRate,
                        denominator = 1
                    },
                    channelCount = audioInput.channelCount,
                    language = ""
                };
            audioAttrsList.Add(audioAttrs);

            if (settings.m_Verbose)
                Debug.Log(
                    string.Format(
                        "MovieRecorder starting to write audio {0}ch @ {1}Hz",
                        audioAttrs.channelCount, audioAttrs.sampleRate.numerator));


#if RECORD_AUDIO_MIXERS
	    var audioSettings = input.audioSettings;
            m_WavWriters = new WavWriter [audioSettings.m_AudioMixerGroups.Length];

            for (int n = 0; n < m_WavWriters.Length; n++)
            {
                if (audioSettings.m_AudioMixerGroups[n].m_MixerGroup == null)
                    continue;

                var path = Path.Combine(
		    m_Settings.m_DestinationPath,
		    "recording of " + audioSettings.m_AudioMixerGroups[n].m_MixerGroup.name + ".wav");
		if (settings.m_Verbose)
		    Debug.Log("Starting wav recording into file " + path);
                m_WavWriters[n].Start(path);
            }
#endif

            try
            {
                m_Encoder = new UnityEditor.Media.MediaEncoder(
		    m_OutputFilePath, videoAttrs, audioAttrsList.ToArray());
                return true;
            }
            catch
            {
                if (settings.m_Verbose)
                    Debug.LogError("MovieRecorder unable to create MovieEncoder.");
            }

            return false;
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 2)
                throw new Exception("Unsupported number of sources");

            var textureInput = (BaseRenderTextureInput)m_Inputs[0];
            var width = textureInput.outputWidth;
            var height = textureInput.outputHeight;

            if (settings.m_Verbose)
                Debug.Log(string.Format("MovieRecorder.RecordFrame {0} x {1} (wanted: {2} x {3})",
                                        textureInput.outputRT.width, textureInput.outputRT.height,
					width, height));

	    if (!m_ReadBackTexture)
		m_ReadBackTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var backupActive = RenderTexture.active;
            RenderTexture.active = textureInput.outputRT;
            m_ReadBackTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            m_ReadBackTexture.Apply();
            m_Encoder.AddFrame(m_ReadBackTexture);
            RenderTexture.active = backupActive;

	    var audioInput = (AudioInput)m_Inputs[1];
            if (!audioInput.audioSettings.m_PreserveAudio)
		return;

#if RECORD_AUDIO_MIXERS
	    for (int n = 0; n < m_WavWriters.Length; n++)
		if (m_WavWriters[n] != null)
		    m_WavWriters[n].Feed(audioInput.mixerGroupAudioBuffer(n));
#endif

	    m_Encoder.AddSamples(audioInput.mainBuffer);
        }

        public override void EndRecording(RecordingSession session)
        {
            base.EndRecording(session);
	    if (m_Encoder != null)
	    {
		m_Encoder.Dispose();
		m_Encoder = null;
	    }

            // When adding a file to Unity's assets directory, trigger a refresh so it is detected.
            if (m_Settings.m_DestinationPath.Contains("Assets/"))
                AssetDatabase.Refresh();

            var input = (BaseRenderTextureInput)m_Inputs[0];
            if (input == null)
		return;
	    
	    var cbRenderTextureInput = input as CBRenderTextureInput;
	    if (cbRenderTextureInput == null)
		return;
	    
	    cbRenderTextureInput.cbSettings.m_FlipVertical = m_OriginalFlipVertical;
        }

        private string BuildOutputPath(string baseFileName)
        {
            var outputPath = Path.Combine(
                m_Settings.m_DestinationPath,
                Path.GetFileNameWithoutExtension(baseFileName));

	    if (m_Settings.m_AppendSuffix)
		outputPath += "_" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss");

	    return outputPath + "." + m_Settings.m_OutputFormat.ToString().ToLower();
        }

        // https://stackoverflow.com/questions/26643695/converting-decimal-to-fraction-c
        static private long GreatestCommonDivisor(long a, long b)
        {
            if (a == 0)
                return b;

            if (b == 0)
                return a;
        
            return (a < b) ? GreatestCommonDivisor(a, b % a) : GreatestCommonDivisor(b, a % b);
        }

        static private MediaRational RationalFromDouble(double value)
        {
            double integral = Math.Floor(value);
            double frac = value - integral;

            const long precision = 1000000000;

            long gcd = GreatestCommonDivisor((long)Math.Round(frac * precision), precision);
            long denom = precision / gcd;

            return new MediaRational() 
            {
                numerator = (int)((long)integral * denom + ((long)Math.Round(frac * (double)precision)) / gcd),
                denominator = (int)denom
            };
        }
    }
}
