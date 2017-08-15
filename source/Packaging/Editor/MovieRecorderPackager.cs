using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{
    class MovieRecorderPackager : ScriptableObject
    {
        public static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();

            File.WriteAllText( Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/TS.txt" ), DateTime.Now.ToString() );

            var files = new []
            {
                Path.Combine(rootPath, "Extensions/MovieRecorder/Recorder" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Audio" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Recorder" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/Editor" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/TS.txt" ),
            };
            var destFile = Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/MovieRecorder.unitypackage");
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);            
        }        
    }

    [InitializeOnLoad]
    class MovieRecorderPackagerInternal : ScriptableObject
    {
        const string k_PackageName = "MovieRecorder.unitypackage";

        static bool AutoExtractAllowed
        {
            get { return !AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "FRPackager" && y.Namespace == "UnityEditor.FrameRecorder")); }
        }

        static bool AudioRecordingAvailable
        {
            get { return AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "AudioRecorder" && y.Namespace == "UnityEngine")); }
        }

        static bool MovieRecordingAvailable
        {
            get { return AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "MediaEncoder" && y.Namespace == "UnityEditor.Media")); }
        }

        static MovieRecorderPackagerInternal() // auto extracts
        {
            if(AutoExtractAllowed && AudioRecordingAvailable && MovieRecordingAvailable )
            {
                var pkgFile = Path.Combine(FRPackagerPaths.GetFrameRecorderRootPath(), "Extensions/MovieRecorder/Packaging/" + k_PackageName);
                var tsFile = Path.Combine(FRPackagerPaths.GetFrameRecorderRootPath(), "Extensions/MovieRecorder/Packaging/TS.txt");
                var recDir = Path.Combine(FRPackagerPaths.GetFrameRecorderRootPath(), "Extensions/MovieRecorder/Recorder");
                if ( File.Exists(pkgFile) && 
                    (!Directory.Exists(recDir) || File.GetLastWriteTime(pkgFile) > File.GetLastWriteTime(tsFile).AddMinutes(5))) // extra 5min to compensate for package write duration
                {
                    Debug.Log("Importing MovieRecorder...");
                    AssetDatabase.ImportPackage(pkgFile, false);
                }
            }
        }

      
    }
}