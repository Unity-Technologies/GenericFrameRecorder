using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.FrameRecorder.Core.Engine
{
    [Serializable]
    public struct FileNameGenerator
    {
        public static string[] tagLabels { get; private set; }
        public static string[] tags { get; private set; }

        public enum ETags
        {
            Time,
            Date,
            Project,
            Scene,
            Resolution,
            Frame,
            Extension
        }

        [SerializeField]
        string m_Pattern;

        public string pattern {
            get { return m_Pattern;}
            set { m_Pattern = value;  }
        }

        static FileNameGenerator()
        {
            tags = new[]
            {
                "<ts>",  
                "<dt>",
                "<prj>",
                "<scn>",
                "<res>",
                "<00000>",
                "<ext>"
            };

            tagLabels = new[]
            {
                "<ts>  - Time",  
                "<dt>  - Date",
                "<prj> - Project name",
                "<scn> - Scene name",
                "<res> - Resolution",
                "<00000> - Frame number",
                "<ext> - Default extension"
            };
        }

        public static string AddTag(string pattern, ETags t)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                switch (t)
                {
                    case ETags.Frame:
                    case ETags.Extension:
                    {
                        pattern += ".";
                        break;
                    }
                    default:
                    {
                        pattern += "-";
                        break;
                    }
                }
            }

            pattern += tags[(int)t];

            return pattern;
        }

        public string BuildFileName( int frame, int width, int height, string ext )
        {
            var fileName  = pattern.Replace(tags[(int)ETags.Extension], ext)
                .Replace(tags[(int)ETags.Resolution], string.Format("{0}x{1}", width, height))
                .Replace(tags[(int)ETags.Frame], frame.ToString("00000"))
                .Replace(tags[(int)ETags.Scene], "(scene-NA)")
                .Replace(tags[(int)ETags.Project], "(prj-NA)")
                .Replace(tags[(int)ETags.Time], "(time-NA)")
                .Replace(tags[(int)ETags.Date], "(date-NA)");

            return fileName;
        }

    }
}
