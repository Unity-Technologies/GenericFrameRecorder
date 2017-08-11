using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.FrameRecorder.Core.Engine
{
    public class FileNameGenerator
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

        public static string AddTag(ETags t, string pattern)
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

        public static string BuildFileName(string pattern, int frame, int width, int height, string ext)
        {
            pattern  = pattern.Replace(tags[(int)ETags.Extension], ext)
                .Replace(tags[(int)ETags.Resolution], string.Format("{0}x{1}", width, height))
                .Replace(tags[(int)ETags.Frame], frame.ToString("00000"))
                .Replace(tags[(int)ETags.Scene], "(scene-NA)")
                .Replace(tags[(int)ETags.Project], "(prj-NA)")
                .Replace(tags[(int)ETags.Time], "(time-NA)")
                .Replace(tags[(int)ETags.Date], "(date-NA)");


            return pattern;
        }

    }
}
